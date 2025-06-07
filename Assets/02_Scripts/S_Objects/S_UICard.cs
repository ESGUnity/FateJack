using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_UICard : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public S_Card CardInfo;

    [Header("연출")]
    [HideInInspector] public PRS OriginPRS;
    Vector2 hideCardPos = new Vector2(0, -800);
    Vector2 originCardPos = new Vector2(0, 300);
    const float APPEAR_TIME = 0.5f;
    const float SCALE_AMOUNT = 1.2f;

    [Header("씬 오브젝트")]
    [SerializeField] protected Image image_CardBase;
    [SerializeField] protected Image image_CardSuit;
    [SerializeField] protected TMP_Text text_CardNumber;

    [SerializeField] protected Image image_BasicCondition;
    [SerializeField] protected Image image_BasicEffect;
    [SerializeField] protected Image image_AdditiveCondition;
    [SerializeField] protected Image image_Debuff;
    [SerializeField] protected Image image_AdditiveEffect;

    [SerializeField] protected Image image_CursedEffect;

    [SerializeField] protected Image image_CardFrame;

    void Awake()
    {
        image_CardBase.raycastTarget = false;
        image_CardSuit.raycastTarget = false;
        text_CardNumber.raycastTarget = false;

        image_BasicCondition.raycastTarget = false;
        image_BasicEffect.raycastTarget = false;
        image_AdditiveCondition.raycastTarget = false;
        image_Debuff.raycastTarget = false;
        image_AdditiveEffect.raycastTarget = false;

        image_CursedEffect.raycastTarget = false;

        image_CardFrame.raycastTarget = false;

        OriginPRS.Rot = transform.eulerAngles;
        OriginPRS.Scale = transform.localScale;
    }

    public void SetCardInfo(S_Card card)
    {
        // 카드 정보 설정
        CardInfo = card;

        if (CardInfo == null) return;

        // 카드 베이스 설정
        string cardBaseAddress = "";
        if (card.IsIllusion) cardBaseAddress = "Sprite_IllusionCardBase";
        else cardBaseAddress = "Sprite_OriginCardBase";
        var cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>(cardBaseAddress);
        cardBaseOpHandle.Completed += OnCardBaseLoadComplete;

        // 카드 문양 설정
        var cardSuitOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Card_{card.Suit}Suit");
        cardSuitOpHandle.Completed += OnCardSuitLoadComplete;

        // 카드 숫자 설정
        text_CardNumber.text = card.Number.ToString();

        // 카드 효과 설정
        if (card.BasicCondition == S_CardBasicConditionEnum.None)
        {
            image_BasicCondition.gameObject.SetActive(false);
        }
        else
        {
            image_BasicCondition.gameObject.SetActive(true);
            var basicConditionOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_BasicCondition_{card.BasicCondition}");
            basicConditionOpHandle.Completed += OnBasicConditionLoadComplete;
        }

        if (card.BasicEffect == S_CardBasicEffectEnum.None)
        {
            image_BasicEffect.gameObject.SetActive(false);
        }
        else
        {
            image_BasicEffect.gameObject.SetActive(true);
            var basicEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_BasicEffect_{card.BasicEffect}");
            basicEffectOpHandle.Completed += OnBasicEffectLoadComplete;
        }

        if (card.AdditiveCondition == S_CardAdditiveConditionEnum.None)
        {
            image_AdditiveCondition.gameObject.SetActive(false);
        }
        else
        {
            image_AdditiveCondition.gameObject.SetActive(true);
            var additiveConditionOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_AdditiveCondition_{card.AdditiveCondition}");
            additiveConditionOpHandle.Completed += OnAdditiveConditionLoadComplete;
        }

        if (card.Debuff == S_CardDebuffConditionEnum.None)
        {
            image_Debuff.gameObject.SetActive(false);
        }
        else
        {
            image_Debuff.gameObject.SetActive(true);
            var debuffOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Debuff_{card.Debuff}");
            debuffOpHandle.Completed += OnDebuffLoadComplete;
        }

        if (card.AdditiveEffect == S_CardAdditiveEffectEnum.None)
        {
            image_AdditiveEffect.gameObject.SetActive(false);
        }
        else
        {
            image_AdditiveEffect.gameObject.SetActive(true);
            var additiveEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_AdditiveEffect_{card.AdditiveEffect}");
            additiveEffectOpHandle.Completed += OnAdditiveEffectLoadComplete;
        }

        // 저주 체크
        SetCursedEffect(card.IsCursed);
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
    void OnBasicConditionLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_BasicCondition.sprite = opHandle.Result;
        }
    }
    void OnBasicEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_BasicEffect.sprite = opHandle.Result;
        }
    }
    void OnAdditiveConditionLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_AdditiveCondition.sprite = opHandle.Result;
        }
    }
    void OnDebuffLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_Debuff.sprite = opHandle.Result;
        }
    }
    void OnAdditiveEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_AdditiveEffect.sprite = opHandle.Result;
        }
    }

    public void ChangeCardVFXByStore(S_Card card)
    {
        // 카드 크기를 작게
        transform.localScale = Vector3.zero;  // 처음에 스케일 0

        // 불투명도 처리
        SetAlphaValue(0, 0);

        // 카드 위치 설정
        Vector2 generateCardPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
        generateCardPos = Vector2.zero;
        GetComponent<RectTransform>().anchoredPosition = generateCardPos;

        // VFX
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;
        Vector2 end = this.hideCardPos;

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

        // 카드 등장 부분
        seq.AppendCallback(() => SetAlphaValue(1f, APPEAR_TIME));
        seq.Append(transform.DOScale(Vector3.one, APPEAR_TIME).SetEase(Ease.OutQuart));
        seq.AppendInterval(0.5f);

        // 카드 변하는 부분
        seq.Append(transform.DOScale(OriginPRS.Scale * SCALE_AMOUNT, APPEAR_TIME / 3).SetEase(Ease.OutQuart))
            .Join(transform.DORotate(OriginPRS.Rot + new Vector3(0, 20, 0), APPEAR_TIME / 3)).SetEase(Ease.OutQuart);
        seq.AppendCallback(() => SetCardInfo(card));
        seq.Append(transform.DOScale(OriginPRS.Scale, APPEAR_TIME / 3).SetEase(Ease.OutQuart))
            .Join(transform.DORotate(OriginPRS.Rot, APPEAR_TIME / 3)).SetEase(Ease.OutQuart);

        seq.AppendInterval(0.8f);
        seq.Append(rt.DOLocalPath(path, APPEAR_TIME, PathType.CatmullRom, PathMode.Ignore).SetEase(Ease.OutQuart))
            .Join(transform.DOScale(Vector3.zero, APPEAR_TIME).SetEase(Ease.OutQuart))
            .OnComplete(() => Destroy(gameObject));
    }

    public virtual void SetAlphaValue(float value, float duration)
    {
        Sequence seq = DOTween.Sequence();
        image_CardBase.DOKill();
        image_CardSuit.DOKill();
        text_CardNumber.DOKill();
        image_BasicCondition.DOKill();
        image_BasicEffect.DOKill();
        image_AdditiveCondition.DOKill();
        image_Debuff.DOKill();
        image_AdditiveEffect.DOKill();
        image_CursedEffect.DOKill();
        image_CardFrame.DOKill();

        seq.Append(image_CardBase.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_BasicCondition.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_BasicEffect.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_AdditiveCondition.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_Debuff.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_AdditiveEffect.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_CursedEffect.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_CardFrame.DOFade(value, duration).SetEase(Ease.OutQuart));

    }
    public void SetCursedEffect(bool isCursed)
    {
        image_CursedEffect.gameObject.SetActive(isCursed);
    }
}














//public async Task ExclusionCardVFX()
//{
//    // 카드 크기를 작게
//    transform.localScale = Vector3.zero;  // 처음에 스케일 0
//    GetComponent<RectTransform>().DOAnchorPos(addCardPos, 0f); // 카드 위치도 밑으로

//    // 불투명도 처리
//    SetAlphaValue(0, 0);

//    // 카드 위치 설정
//    Vector2 generateCardPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
//    GetComponent<RectTransform>().anchoredPosition = generateCardPos;

//    Sequence seq = DOTween.Sequence();
//    seq.Append(GetComponent<RectTransform>().DOAnchorPos(generateCardPos, 1.6f))
//        .Join(transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutQuart))
//        .Join(image_CardBase.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
//        .Join(text_CardNumber.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
//        .Join(image_CardSuit.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
//        .Join(image_CardEffect.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
//        .Append(GetComponent<RectTransform>().DOAnchorPos(exclusionCardPos, 1f))
//        .AppendInterval(0.2f)
//        .Join(image_CardBase.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
//        .Join(text_CardNumber.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
//        .Join(image_CardSuit.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
//        .Join(image_CardEffect.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
//        .OnComplete(() => Destroy(gameObject));

//    await Task.Delay(1200);
//}