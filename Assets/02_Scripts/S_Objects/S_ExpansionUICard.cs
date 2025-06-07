using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_ExpansionUICard : S_UICard, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    const float POINTER_ENTER_SCALE_AMOUNT = 1.25f;
    const float POINTER_ENTER_ANIMATION_TIME = 0.1f;

    public override void SetAlphaValue(float value, float duration)
    {
        base.SetAlphaValue(value, duration);

        if (value == 0)
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
        }
        else
        {
            image_CardBase.raycastTarget = true;
            image_CardSuit.raycastTarget = true;
            text_CardNumber.raycastTarget = true;

            image_BasicCondition.raycastTarget = true;
            image_BasicEffect.raycastTarget = true;
            image_AdditiveCondition.raycastTarget = true;
            image_Debuff.raycastTarget = true;
            image_AdditiveEffect.raycastTarget = true;

            image_CursedEffect.raycastTarget = true;

            image_CardFrame.raycastTarget = true;
        }
    }

    bool isEnter;
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverInfoSystem.Instance.ActivateHoverInfo(CardInfo, GetComponent<RectTransform>());

        isEnter = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnter)
        {
            transform.DOKill();

            transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverInfoSystem.Instance.DeactiveHoverInfo();

            isEnter = false;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // 전개 카드 효과 숨기기
        S_UICardEffecter.Instance.EndExpansionCards();

        // 카드 내기
        S_HitBtnSystem.Instance.SelectHitCardByExpansion(CardInfo);
    }
}
