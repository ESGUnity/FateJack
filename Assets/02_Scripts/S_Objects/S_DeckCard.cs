using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.DebugUI;

public class S_DeckCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // 카드 정보
    [HideInInspector] public S_Card CardInfo;

    // 카드 애님 관련
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;
    [HideInInspector] public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;

    // 카드 표시 관련
    [SerializeField] SpriteRenderer sprite_CardBase;
    [SerializeField] TMP_Text text_CardNumber;
    [SerializeField] SpriteRenderer sprite_CardSuit;
    [SerializeField] SpriteRenderer sprite_CardEffect;
    [SerializeField] SpriteRenderer sprite_CursedEffect;
    [SerializeField] SpriteRenderer sprite_CardFrame;

    // 의지 히트나 상점에서 사용
    [HideInInspector] public bool IsSelectedByDetermination;
    [HideInInspector] public bool IsSelectedByRemoveCard; // 제외할 카드 선택 시. 상점 전용

    public void SetCardInfo(S_Card card)
    {
        // 카드 정보 설정
        CardInfo = card;

        if (CardInfo.Equals(default)) return;

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

        UpdateDeckCardState();
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
        sprite_CursedEffect.sortingLayerName = "WorldObject";
        sprite_CursedEffect.sortingOrder = order + 3;
        sprite_CardFrame.sortingLayerName = "WorldObject";
        sprite_CardFrame.sortingOrder = order + 5;

        // 카드 숫자
        text_CardNumber.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_CardNumber.GetComponent<MeshRenderer>().sortingOrder = order + 2;

        // 카드 문양
        sprite_CardSuit.sortingLayerName = "WorldObject";
        sprite_CardSuit.sortingOrder = order + 2;
    }
    #region 버튼 함수
    bool isEnter = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.OnDeckInfo || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.StoreByRemove)
        {
            SetOrder(1000);
            GetComponent<Transform>().DOLocalMove(OriginPRS.Pos + new Vector3(0, 0.05f, 0), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            GetComponent<Transform>().DOScale(OriginPRS.Scale + new Vector3(0.12f, 0.12f, 0.12f), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverCardSystem.Instance.ActivePanelByDeckCard(CardInfo, transform.position);

            isEnter = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnter)
        {
            SetOrder(OriginOrder);
            GetComponent<Transform>().DOLocalMove(OriginPRS.Pos, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            GetComponent<Transform>().DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverCardSystem.Instance.DisablePanelOnCard();

            isEnter = false;
        }
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.StoreByRemove)
        {
            if (IsSelectedByRemoveCard)
            {
                S_StoreInfoSystem.Instance.CancelSelectCardByStore(CardInfo);
                IsSelectedByRemoveCard = false;
            }
            else
            {
                S_StoreInfoSystem.Instance.SelectCardByRemoveCard(CardInfo);
                IsSelectedByRemoveCard = true;
            }
        }
        else if (CardInfo.IsInDeck)
        {
            S_DeckInfoSystem.Instance.SelectCardByDeterminationHit(CardInfo);

            if (IsSelectedByDetermination)
            {
                S_DeckInfoSystem.Instance.CancelSelectCardByDeterminationHit();
            }
        }
    }
    #endregion
    #region 카드의 상테에 따른 효과
    public void UpdateDeckCardState()
    {
        OnIsInDeckAlphaValue();
        OnCursedEffect();
    }
    public void OnIsInDeckAlphaValue()
    {
        if (CardInfo.IsInDeck)
        {
            SetAlphaValue(1, 0);
        }
        else
        {
            SetAlphaValue(0.25f, 0);
        }
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
    public void SetAlphaValue(float value, float duration)
    {
        sprite_CardBase.DOFade(value, duration);
        text_CardNumber.DOFade(value, duration);
        sprite_CardSuit.DOFade(value, duration);
        sprite_CardEffect.DOFade(value, duration);
        sprite_CursedEffect.DOFade(value, duration);
        sprite_CardFrame.DOFade(value, duration);
    }
    #endregion
}
