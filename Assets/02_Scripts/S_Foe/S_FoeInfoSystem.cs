using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_FoeInfoSystem : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public int CurrentHealth;
    [HideInInspector] public List<S_CardBase> FoeCardList;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject pos_FoeCardsBase;

    [Header("프리팹")]
    [SerializeField] GameObject prefab_FieldCard;

    [Header("컴포넌트")]
    GameObject panel_FoeInfoBase;
    Image image_HealthBar;
    TMP_Text text_HealthValue;

    [Header("카드 오브젝트 리스트")]
    List<GameObject> foeCardObjs = new();

    [Header("UI")]
    Vector2 hidePos = new Vector2(0, 300);
    Vector2 originPos = new Vector2(0, 0);
    Vector3 ATTACK_READY_POS = new Vector3(0, 0, -1);
    Vector3 ATTACK_TARGET_POS = new Vector3(0, -5.5f, -1.15f);

    [Header("연출")]
    Vector3 CARD_SPAWN_POS = new Vector3(0, 0, 10f);
    Vector3 FOE_CARDS_START_POS = new Vector3(-5.5f, 0, 0);
    Vector3 FOE_CARDS_END_POS = new Vector3(5.5f, 0, 0);
    Vector3 FOE_CARDS_ORIGIN_SCALE = new Vector3(1.7f, 1.7f, 1.7f);
    const float STACK_Z_VALUE = -0.02f;
    const float CARD_POS_OFFSET = 0.02f;
    const int MAX_CARD_COUNT = 4; // 고정 격차로 배치되는 최대 카드 개수

    // 싱글턴
    static S_FoeInfoSystem instance;
    public static S_FoeInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // 컴포넌트 할당
        panel_FoeInfoBase = System.Array.Find(transforms, c => c.gameObject.name.Equals("Panel_FoeInfoBase")).gameObject;
        image_HealthBar = System.Array.Find(images, c => c.gameObject.name.Equals("Image_HealthBar"));
        text_HealthValue = System.Array.Find(texts, c => c.gameObject.name.Equals("Text_HealthValue"));

        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 초기화

    #endregion
    #region VFX

    #endregion
    #region 시련 진행에 따른 메서드
    public async Task UpdateCardsByRewardTime() // 카드 등장, 적 체력 설정
    {
        // 주요 정보 할당
        S_FoeStruct foe = S_FoeList.FOES.First(x => x.Trial == S_GameFlowManager.Instance.CurrentTrial + 1);
        MaxHealth = foe.Health;
        CurrentHealth = MaxHealth;
        FoeCardList = foe.EssentialFoeCards;
        FoeCardList.AddRange(foe.OptionalFoeCards.OrderBy(x => Random.value).Take(foe.OptionalCount).ToList());

        // 체력바 세팅
        ChangeHealthValueVFX();

        // 카드 세팅
        await SetFoeCardsByRewardTime(FoeCardList);
    }
    public async Task UpdateFoeByStartTrial()
    {
        await S_DialogInfoSystem.Instance.StartQueueDialog(S_DialogMetaData.GetDialogsByPrefix($"FoeDialog_StartTrial_{S_GameFlowManager.Instance.CurrentTrial}"));
    }
    public async Task ResetFoeCardsByEndTrial() // 시련 종료 시 적 카드를 제거하는 메서드
    {
        List<Task> animationTasks = new();

        foreach (GameObject go in foeCardObjs)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(go.transform.DOMove(CARD_SPAWN_POS, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart)).OnComplete(() => Destroy(go));

            animationTasks.Add(seq.AsyncWaitForCompletion());
        }
        await Task.WhenAll(animationTasks);

        foeCardObjs.Clear();
    }
    public void DamagedByHarm(int harmValue) // 피해로 인한 데미지
    {
        CurrentHealth -= harmValue;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        ChangeHealthValueVFX();
    }
    #endregion
    #region 보조
    public void ChangeHealthValueVFX() // 체력 트윈
    {
        image_HealthBar.DOFillAmount((float)CurrentHealth / MaxHealth, S_EffectActivator.Instance.GetEffectLifeTime()).SetEase(Ease.OutQuart);
        ChangeHealthValueVFXTween(int.Parse(text_HealthValue.text.Split('/')[0].Trim()), CurrentHealth, text_HealthValue);
    }
    void ChangeHealthValueVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = $"{currentNumber} / {MaxHealth}"; },
                newValue,
                S_EffectActivator.Instance.GetEffectLifeTime()
            ).SetEase(Ease.OutQuart);
    }

    public async Task SetFoeCardsByRewardTime(List<S_CardBase> cards) // 카드 등록 메서드
    {
        // 스택 카드 생성 및 설정
        foreach (S_CardBase card in cards)
        {
            GameObject go = Instantiate(prefab_FieldCard);
            go.transform.position = CARD_SPAWN_POS;
            go.transform.SetParent(pos_FoeCardsBase.transform, true);
            go.GetComponent<S_FieldCardObj>().SetCardInfo(card);
            foeCardObjs.Add(go);
        }

        await AlignmentFoeCards();
    }
    async Task AlignmentFoeCards() // 적 카드 정렬
    {
        List<GameObject> allStackCards = foeCardObjs.ToList();
        List<PRS> originCardPRS = SetFoeCardPos(allStackCards.Count);
        List<Task> tweenTask = new();

        for (int i = 0; i < allStackCards.Count; i++)
        {
            // 카드 위치 설정
            allStackCards[i].GetComponent<S_FieldCardObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            allStackCards[i].GetComponent<S_FieldCardObj>().OriginOrder = (i + 1) * 10;
            allStackCards[i].GetComponent<S_FieldCardObj>().SetOrder(allStackCards[i].GetComponent<S_FieldCardObj>().OriginOrder);

            // 두트윈
            Task move = allStackCards[i].transform.DOLocalMove(allStackCards[i].GetComponent<S_FieldCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task rotate = allStackCards[i].transform.DOLocalRotate(allStackCards[i].GetComponent<S_FieldCardObj>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task scale = allStackCards[i].transform.DOScale(allStackCards[i].GetComponent<S_FieldCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            tweenTask.Add(Task.WhenAll(move, rotate, scale));
        }

        await Task.WhenAll(tweenTask);
    }
    List<PRS> SetFoeCardPos(int cardCount) // 적 카드 위치 설정
    {
        List<PRS> results = new List<PRS>(cardCount);

        for (int i = 0; i < cardCount; i++)
        {
            float t;
            if (cardCount == 1)
            {
                t = 0.5f; // 중앙
            }
            else if (cardCount == 2)
            {
                t = (i == 0) ? 0.34f : 0.66f; // 약간 가운데로 몰리게
            }
            else
            {
                float interval = cardCount <= MAX_CARD_COUNT ? 1f / (MAX_CARD_COUNT - 1) : 1f / (cardCount - 1);
                t = interval * i;
            }

            Vector3 pos = Vector3.Lerp(FOE_CARDS_START_POS, FOE_CARDS_END_POS, t);
            pos = new Vector3(
                pos.x + Random.Range(-CARD_POS_OFFSET, CARD_POS_OFFSET),
                pos.y + Random.Range(-CARD_POS_OFFSET, CARD_POS_OFFSET),
                i * STACK_Z_VALUE
            );

            Vector3 rot = new Vector3(0, 0, 0);
            Vector3 scale = FOE_CARDS_ORIGIN_SCALE;

            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }

    public GameObject GetFoeCardObj(S_CardBase card)
    {
        foreach (GameObject go in foeCardObjs)
        {
            if (go.GetComponent<S_FieldCardObj>().CardInfo == card)
            {
                return go;
            }
        }

        Debug.Log("필드에 없는 걸 왜 찾나");
        return null;
    }
    #endregion
}