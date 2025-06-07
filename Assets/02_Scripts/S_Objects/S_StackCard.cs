using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_StackCard : S_CardObject, IPointerEnterHandler, IPointerExitHandler
{
    // 생성됨은 어차피 낼 때만 확인하면 되니까 SetCardInfo에서 처리
    #region 포인터 함수
    bool isEnter = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.HittingCard)
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
    #endregion
    #region VFX 관련
    public override void UpdateCardState()
    {
        base.UpdateCardState();

        OnCurrentTurnHitEffect();
        OnMeetConditionEffect();
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
        if (CardInfo.IsMeetCondition)
        {
            sprite_MeetConditionEffect.gameObject.SetActive(true);
        }
        else
        {
            sprite_MeetConditionEffect.gameObject.SetActive(false);
        }
    }
    public void BouncingVFX() // 바운싱 VFX
    {
        S_TweenHelper.Instance.BouncingVFX(transform, OriginPRS.Scale, OriginPRS.Rot);
    }
    #endregion
}
