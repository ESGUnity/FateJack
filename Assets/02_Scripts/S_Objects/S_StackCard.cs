using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_StackCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 카드 정보
    [HideInInspector] public S_Card CardInfo;

    // 카드 애님 관련
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;

    const float POINTER_ENTER_ANIMATION_TIME = 0.15f;
    const float POINTER_ENTER_SCALE_AMOUNT = 1.15f;
    Vector3 POINTER_ENTER_POS = new Vector3(0, 0.05f, 0);

    const float BOUNCING_SCALE_AMOUNT = 1.25f;

    // 카드 표시 관련
    [SerializeField] SpriteRenderer sprite_MeetConditionEffect;
    [SerializeField] SpriteRenderer sprite_CardBase;
    [SerializeField] TMP_Text text_CardNumber;
    [SerializeField] SpriteRenderer sprite_CardSuit;
    [SerializeField] SpriteRenderer sprite_CardEffect;
    [SerializeField] SpriteRenderer sprite_CursedEffect;
    [SerializeField] SpriteRenderer sprite_CardFrame;
    [SerializeField] SpriteRenderer sprite_IllusionEffect;
    [SerializeField] SpriteRenderer sprite_CurrentTurnHitEffect;

    #region 초기 카드 정보 설정 메서드
    public void SetCardInfo(S_Card card)
    {
        // 카드 정보 설정
        CardInfo = card;

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

        UpdateCardState();
    }
    void OnCardBaseLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_CardBase.sprite = opHandle.Result;
        }
    }
    void OnCardSuitLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_CardSuit.sprite = opHandle.Result;
        }
    }
    void OnCardEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_CardEffect.sprite = opHandle.Result;
        }
    }
    public void SetOrder(int order) // 카드 요소의 소팅 오더를 정렬
    {
        // 카드 베이스
        sprite_CardBase.sortingLayerName = "WorldObject";
        sprite_CardBase.sortingOrder = order; 

        // 카드 효과
        sprite_CardEffect.sortingLayerName = "WorldObject";
        sprite_CardEffect.sortingOrder = order + 1;

        sprite_IllusionEffect.sortingLayerName = "WorldObject";
        sprite_IllusionEffect.sortingOrder = order + 3;
        sprite_CurrentTurnHitEffect.sortingLayerName = "WorldObject";
        sprite_CurrentTurnHitEffect.sortingOrder = order + 4;
        sprite_CursedEffect.sortingLayerName = "WorldObject";
        sprite_CursedEffect.sortingOrder = order + 5;

        sprite_CardFrame.sortingLayerName = "WorldObject";
        sprite_CardFrame.sortingOrder = order + 6;

        // 카드 숫자
        text_CardNumber.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_CardNumber.GetComponent<MeshRenderer>().sortingOrder = order + 2;

        // 카드 문양
        sprite_CardSuit.sortingLayerName = "WorldObject";
        sprite_CardSuit.sortingOrder = order + 2;
    }
    #endregion
    #region 버튼 함수
    bool isEnter = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.HittingCard)
        {
            transform.DOKill();

            SetOrder(1000);
            transform.DOLocalMove(OriginPRS.Pos + POINTER_ENTER_POS, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            transform.DORotate(Vector3.zero, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverCardSystem.Instance.ActivePanelByStackCard(CardInfo, transform.position);

            isEnter = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnter)
        {
            transform.DOKill();

            SetOrder(OriginOrder);
            transform.DOLocalMove(OriginPRS.Pos, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            transform.DORotate(OriginPRS.Rot, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverCardSystem.Instance.DisablePanelOnCard();

            isEnter = false;
        }
    }
    #endregion
    #region VFX 관련
    public void BouncingVFX() // 바운싱 VFX
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(OriginPRS.Scale * BOUNCING_SCALE_AMOUNT, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad))
            .Join(transform.DORotate(Vector3.zero, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(OriginPRS.Scale, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad))
            .Join(transform.DORotate(OriginPRS.Rot, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad));
    }
    public void SetAlphaValue(float value, float duration)
    {
        sprite_CardBase.DOFade(value, duration).SetEase(Ease.OutQuart);
        text_CardNumber.DOFade(value, duration).SetEase(Ease.OutQuart);
        sprite_CardSuit.DOFade(value, duration).SetEase(Ease.OutQuart);
        sprite_CardEffect.DOFade(value, duration).SetEase(Ease.OutQuart);
        sprite_CursedEffect.DOFade(value, duration).SetEase(Ease.OutQuart);
        sprite_IllusionEffect.DOFade(value, duration).SetEase(Ease.OutQuart);
        sprite_CurrentTurnHitEffect.DOFade(value, duration).SetEase(Ease.OutQuart);
        sprite_CardFrame.DOFade(value, duration).SetEase(Ease.OutQuart);
    }
    public void UpdateCardState()
    {
        OnCursedEffect();
        OnIllusionEffect();
        OnCurrentTurnHitEffect();
        OnMeetConditionEffect();
    }
    public void OnCursedEffect()
    {
        if (CardInfo.IsCursed)
        {
            sprite_CursedEffect.gameObject.SetActive(true);
        }
        else
        {
            sprite_CursedEffect.gameObject.SetActive(false);
        }
    }
    public void OnIllusionEffect()
    {
        if (CardInfo.IsIllusion)
        {
            sprite_IllusionEffect.gameObject.SetActive(true);
        }
        else
        {
            sprite_IllusionEffect.gameObject.SetActive(false);
        }
    }
    public void OnCurrentTurnHitEffect()
    {
        if (CardInfo.IsCurrentTurnHit)
        {
            sprite_CurrentTurnHitEffect.gameObject.SetActive(true);
        }
        else
        {
            sprite_CurrentTurnHitEffect.gameObject.SetActive(false);
        }
    }
    public void OnMeetConditionEffect()
    {
        if (S_EffectActivator.Instance.IsMeetAdditiveCondition(CardInfo))
        {
            sprite_MeetConditionEffect.gameObject.SetActive(true);
        }
        else
        {
            sprite_MeetConditionEffect.gameObject.SetActive(false);
        }
    }
    #endregion
}
