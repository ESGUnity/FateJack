using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_UICard : MonoBehaviour
{
    // 카드 정보
    [HideInInspector] public S_Card CardInfo;

    // 카드 표시 관련
    [SerializeField] protected Image image_CardBase;
    [SerializeField] protected TMP_Text text_CardNumber;
    [SerializeField] protected Image image_CardSuit;
    [SerializeField] protected Image image_CardEffect;
    [SerializeField] protected Image image_CursedEffect;
    [SerializeField] protected Image image_CardFrame;

    Vector2 addCardPos = new Vector2(0, -800);
    Vector2 exclusionCardPos = new Vector2(0, 300);
    const float APPEAR_TIME = 0.5f;

    void Awake()
    {
        image_CardBase.raycastTarget = false;
        text_CardNumber.raycastTarget = false;
        image_CardSuit.raycastTarget = false;
        image_CardEffect.raycastTarget = false;
        image_CursedEffect.raycastTarget = false;
        image_CardFrame.raycastTarget = false;
    }
    public void SetCardInfo(S_Card card)
    {
        // 카드 정보 설정
        CardInfo = card;

        if (CardInfo == null) return;

        // 카드 베이스 설정
        string cardBaseAddress = "";
        switch (card.CardEffect.Grade)
        {
            case S_CardEffectGradeEnum.Normal:
                cardBaseAddress = "Sprite_NormalCardBase";
                break;
            case S_CardEffectGradeEnum.Superior:
                cardBaseAddress = "Sprite_SuperiorCardBase";
                break;
            case S_CardEffectGradeEnum.Rare:
                cardBaseAddress = "Sprite_RareCardBase";
                break;
            case S_CardEffectGradeEnum.Mythic:
                cardBaseAddress = "Sprite_MythicCardBase";
                break;
        }
        var cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>(cardBaseAddress);
        cardBaseOpHandle.Completed += OnCardBaseLoadComplete;

        // 카드 숫자 설정
        text_CardNumber.text = card.Number.ToString();

        // 카드 문양 설정
        string cardSuitAddress = "";
        switch (card.Suit)
        {
            case S_CardSuitEnum.Spade:
                cardSuitAddress = "Sprite_SpadeSuit";
                break;
            case S_CardSuitEnum.Heart:
                cardSuitAddress = "Sprite_HeartSuit";
                break;
            case S_CardSuitEnum.Diamond:
                cardSuitAddress = "Sprite_DiamondSuit";
                break;
            case S_CardSuitEnum.Clover:
                cardSuitAddress = "Sprite_CloverSuit";
                break;
        }
        var cardSuitOpHandle = Addressables.LoadAssetAsync<Sprite>(cardSuitAddress);
        cardSuitOpHandle.Completed += OnCardSuitLoadComplete;

        // 카드 효과 설정
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{card.CardEffect.Key}");
        cardEffectOpHandle.Completed += OnCardEffectLoadComplete;
    }
    void OnCardBaseLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_CardBase.sprite = opHandle.Result;
        }
    }
    void OnCardSuitLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_CardSuit.sprite = opHandle.Result;
        }
    }
    void OnCardEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_CardEffect.sprite = opHandle.Result;
        }
    }

    public void GetCardVFX()
    {
        // 카드 크기를 작게
        transform.localScale = Vector3.zero;  // 처음에 스케일 0

        // 불투명도 처리
        image_CardBase.DOFade(0f, 0f);
        text_CardNumber.DOFade(0f, 0f);
        image_CardSuit.DOFade(0f, 0f);
        image_CardEffect.DOFade(0f, 0f);

        // 카드 위치 설정
        Vector2 generateCardPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
        GetComponent<RectTransform>().anchoredPosition = generateCardPos;

        // VFX
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;
        Vector2 end = addCardPos;

        // 곡선의 휘어짐 정도 (y로는 위로 띄우고, x로는 좌/우 무작위 휘어짐)
        float curveHeight = 250f;
        float curveHorizontal = Random.Range(-350f, 350f); // 왼쪽 또는 오른쪽으로 휘게

        // 중간 지점 (휘어짐 제어)
        Vector2 control = new Vector2(
            Mathf.Lerp(start.x, end.x, 0.5f) + curveHorizontal,
            Mathf.Lerp(start.y, end.y, 0.5f) + curveHeight
        );

        // 경로 구성 (시작점은 생략 가능)
        Vector3[] path = new Vector3[] { control, end };

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.one, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(image_CardBase.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .AppendInterval(0.8f)
            .Append(rt.DOLocalPath(path, APPEAR_TIME, PathType.CatmullRom, PathMode.Ignore).SetEase(Ease.OutQuart))
            .Join(transform.DOScale(Vector3.zero, APPEAR_TIME).SetEase(Ease.OutQuart))
            .OnComplete(() => Destroy(gameObject));
    }
    public async Task ExclusionCardVFX()
    {
        // 카드 크기를 작게
        transform.localScale = Vector3.zero;  // 처음에 스케일 0
        GetComponent<RectTransform>().DOAnchorPos(addCardPos, 0f); // 카드 위치도 밑으로

        // 불투명도 처리
        SetAlphaValue(0, 0);

        // 카드 위치 설정
        Vector2 generateCardPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
        GetComponent<RectTransform>().anchoredPosition = generateCardPos;

        Sequence seq = DOTween.Sequence();
        seq.Append(GetComponent<RectTransform>().DOAnchorPos(generateCardPos, 1.6f))
            .Join(transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardBase.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Append(GetComponent<RectTransform>().DOAnchorPos(exclusionCardPos, 1f))
            .AppendInterval(0.2f)
            .Join(image_CardBase.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .OnComplete(() => Destroy(gameObject));

        await Task.Delay(1200);
    }
    public void SetAlphaValue(float value, float duration)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(image_CardBase.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(value, duration).SetEase(Ease.OutQuart));
    }
    public void SetCursedEffect(bool isCursed)
    {
        image_CursedEffect.gameObject.SetActive(isCursed);
    }
}
