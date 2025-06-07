using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_DeckCard : S_CardObject, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region 버튼 함수
    bool isEnter = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.OnDeckInfo || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.StoreByDeckInfo)
        {
            transform.DOKill();

            SetOrder(1000);
            transform.DOLocalMove(OriginPRS.Pos + POINTER_ENTER_POS, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            transform.DOLocalRotate(Vector3.zero, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverInfoSystem.Instance.ActivateHoverInfo(CardInfo, gameObject);

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
            transform.DOLocalRotate(OriginPRS.Rot, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverInfoSystem.Instance.DeactiveHoverInfo();

            isEnter = false;
        }
    }
    public void BackToOriginPRSByDeterminationHit()
    {
        transform.DOLocalMove(OriginPRS.Pos, 0);
        transform.DOScale(OriginPRS.Scale, 0);
        transform.DOLocalRotate(OriginPRS.Rot, 0);

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.StoreByDeckInfo) // 상품 중 카드 선택하는 상품의 카드 선택 시
        {
            S_StoreInfoSystem.Instance.SelectCardInDeck(CardInfo, transform.position);
        }
        else if (CardInfo.IsInDeck) // 의지 히트 시
        {
            S_DeckInfoSystem.Instance.SelectCardByDeterminationHit(CardInfo);
        }
    }
    #endregion
    #region 카드의 상태에 따른 효과
    public override void UpdateCardState()
    {
        base.UpdateCardState();

        OnIsInDeckAlphaValue();
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
    #endregion
}
