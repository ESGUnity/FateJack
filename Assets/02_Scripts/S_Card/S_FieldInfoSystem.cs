using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_FieldInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_FieldCard;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject pos_FieldBase;

    [Header("카드 오브젝트 리스트")]
    List<GameObject> fieldCardObjs = new();

    [Header("연출")]
    Vector3 CARD_SPAWN_POS = new Vector3(0, 0, -9f);
    Vector3 effectLogEndPoint = new Vector3(0, 50, 0);
    const float GEN_CARD_SPAWN_Z_AMOUNT = -10;

    Vector3 STACK_BASE_START_POS = new Vector3(-5.5f, 0, 0);
    Vector3 STACK_BASE_END_POS = new Vector3(5.5f, 0, 0);
    Vector3 STACK_CARD_ORIGIN_SCALE = new Vector3(1.7f, 1.7f, 1.7f);
    const float STACK_Z_VALUE = -0.02f;
    const float CARD_POS_OFFSET = 0.02f;
    const int MAX_CARD_COUNT = 8; // 고정 격차로 배치되는 최대 카드 개수

    public const float EXCLUSION_SCALE_AMOUNT = 1.2f;
    public const float EXCLUSION_TIME = 0.4f;
    public const float EXCLUSION_MOVE_Y_AMOUNT = 10f;

    // 싱글턴
    static S_FieldInfoSystem instance;
    public static S_FieldInfoSystem Instance { get { return instance; } }

    void Awake()
    {
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

    #region 히트 관련
    public async Task HitFieldCard(S_CardBase card) // 카드를 낼 때 오브제 메서드
    {
        // 스택 카드 생성 및 설정
        GameObject go = Instantiate(prefab_FieldCard);
        go.transform.position = CARD_SPAWN_POS;
        go.transform.SetParent(pos_FieldBase.transform, true);
        go.GetComponent<S_FieldCardObj>().SetCardInfo(card);
        fieldCardObjs.Add(go);

        await AlignmentFieldCards();
    }
    public async Task AlignmentFieldCards() // 스택에 있는 카드를 정렬하기
    {
        List<GameObject> allStackCards = fieldCardObjs.ToList();
        List<PRS> originCardPRS = SetFieldCardPos(allStackCards.Count);
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
    List<PRS> SetFieldCardPos(int cardCount) // 카드 위치 설정하는 메서드
    {
        List<PRS> results = new List<PRS>(cardCount);
        float interval = cardCount <= MAX_CARD_COUNT ? 1f / (MAX_CARD_COUNT - 1) : 1f / (cardCount - 1);

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(STACK_BASE_START_POS, STACK_BASE_END_POS, interval * i);
            pos = new Vector3(pos.x + Random.Range(-CARD_POS_OFFSET, CARD_POS_OFFSET), pos.y + Random.Range(-CARD_POS_OFFSET, CARD_POS_OFFSET), i * STACK_Z_VALUE);

            Vector3 rot = new Vector3(0, 0, 0);

            Vector3 scale = STACK_CARD_ORIGIN_SCALE;

            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void BouncingFieldCard(S_CardBase card) // 카드 바운스 효과 실행
    {
        foreach (GameObject cardObj in fieldCardObjs)
        {
            if (cardObj.GetComponent<S_FieldCardObj>().CardInfo == card)
            {
                cardObj.GetComponent<S_FieldCardObj>().BouncingVFX();
                break;
            }
        }
    }
    public void RemoveFieldCard(S_CardBase card)
    {
        GameObject go = GetFieldCardObj(card);
        fieldCardObjs.Remove(go);
        Destroy(go);
    }
    #endregion
    #region 각종 효과 함수
    public async Task FlexibleCard(S_CardBase card)
    {
        GameObject go = GetFieldCardObj(card);

        int index = fieldCardObjs.IndexOf(go);
        if (index == -1 || index == fieldCardObjs.Count - 1) return; // 없거나 이미 마지막이면 할 필요 없음

        fieldCardObjs.RemoveAt(index);
        fieldCardObjs.Add(go);

        // 정렬하기
        await AlignmentFieldCards();
    }
    public async Task LeapCard(S_CardBase card)
    {
        GameObject go = GetFieldCardObj(card);

        int index = fieldCardObjs.IndexOf(go);
        if (index <= 0) return; // 없거나 이미 첫 번째면 무시

        fieldCardObjs.RemoveAt(index);
        fieldCardObjs.Insert(0, go);

        // 정렬하기
        await AlignmentFieldCards();
    }
    public async Task ResetCardsByEndTrialAsync() // 시련 종료 시 카드를 덱으로 다시 가져오는 메서드
    {
        List<Task> animationTasks = new();

        foreach (GameObject go in fieldCardObjs)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(go.transform.DOMove(CARD_SPAWN_POS, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart)).OnComplete(() => Destroy(go));

            animationTasks.Add(seq.AsyncWaitForCompletion());
        }
        await Task.WhenAll(animationTasks);

        fieldCardObjs.Clear();
    }
    public void UpdateCardState() // 카드의 저주, 이번 턴에 히트했는가 여부, 환상, 각인 조건 충족의 상태를 모두 업데이트 하여 VFX를 OnOff 합니다.
    {
        foreach (GameObject go in fieldCardObjs)
        {
            go.GetComponent<S_FieldCardObj>().UpdateCardState();
        }
    }
    #endregion
    #region 보조 함수 && 비틀기 호버링 시 사용하는 것들
    public void AlignmentFieldCardsOnlyOrder() // 바뀌었던 Order만 다시 원위치하는 메서드
    {
        List<GameObject> allStackCards = fieldCardObjs.ToList();

        for (int i = 0; i < allStackCards.Count; i++)
        {
            // 소팅오더 설정
            allStackCards[i].GetComponent<S_FieldCardObj>().OriginOrder = (i + 1) * 10;
            allStackCards[i].GetComponent<S_FieldCardObj>().SetOrder(allStackCards[i].GetComponent<S_FieldCardObj>().OriginOrder);
        }
    }
    public List<GameObject> GetCurrentTurnCardObjs()
    {
        List<GameObject> list = new();

        foreach (GameObject go in fieldCardObjs)
        {
            if (go != null && go.GetComponent<S_FieldCardObj>().CardInfo.IsCurrentTurn)
            {
                list.Add(go);
            }
        }

        return list;
    }
    public GameObject GetFieldCardObj(S_CardBase card)
    {
        foreach (GameObject go in fieldCardObjs)
        {
            if (go.GetComponent<S_FieldCardObj>().CardInfo == card)
            {
                return go;
            }
        }

        Debug.Log("스택에 없는 걸 왜 찾으려 하는가");
        return null;
    }
    public void RemoveFieldCardObj(S_CardBase card)
    {
        foreach (GameObject go in fieldCardObjs)
        {
            if (go.GetComponent<S_FieldCardObj>().CardInfo == card)
            {
                fieldCardObjs.Remove(go);
                Destroy(go);
                return;
            }
        }

        Debug.Log("스택에 없는 걸 왜 찾으려 하는가");
    }
    public async Task SwapCardObjIndex(S_CardBase card, bool isMoveRight) // 오브젝트 인덱스 교환하는 메서드
    {
        if (isMoveRight)
        {
            int index = fieldCardObjs.IndexOf(GetFieldCardObj(card));

            Debug.Log(index);
            if (index == fieldCardObjs.Count - 1) // 마지막 인덱스라면
            {
                GameObject lastCard = fieldCardObjs[index];

                // 리스트의 요소들을 뒤에서부터 한 칸씩 뒤로 밀기
                for (int i = index; i > 0; i--)
                {
                    fieldCardObjs[i] = fieldCardObjs[i - 1];
                }

                // 맨 앞에 마지막 카드를 넣기
                fieldCardObjs[0] = lastCard;
            }
            else // 아니라면
            {
                // 오른쪽 요소와 자리 바꾸기
                GameObject tempGo = fieldCardObjs[index + 1];
                fieldCardObjs[index + 1] = fieldCardObjs[index];
                fieldCardObjs[index] = tempGo;
            }
        }
        else
        {
            int index = fieldCardObjs.IndexOf(GetFieldCardObj(card));
            Debug.Log(index);
            if (index == 0)
            {
                GameObject firstCard = fieldCardObjs[0];

                // 요소들을 왼쪽으로 한 칸씩 이동
                for (int i = 0; i < fieldCardObjs.Count - 1; i++)
                {
                    fieldCardObjs[i] = fieldCardObjs[i + 1];
                }

                // 마지막 자리에 처음 요소 넣기
                fieldCardObjs[fieldCardObjs.Count - 1] = firstCard;
            }
            else
            {
                // 일반적인 왼쪽 교체
                GameObject tempGo = fieldCardObjs[index - 1];
                fieldCardObjs[index - 1] = fieldCardObjs[index];
                fieldCardObjs[index] = tempGo;
            }
        }

        // 위치 바꿨으니 정렬
        await AlignmentFieldCards();
    }
    #endregion
}
