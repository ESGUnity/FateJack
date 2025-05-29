using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_ExpansionUICard : S_UICard, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Vector3 originScale;
    const float POINTER_ENTER_SCALE_AMOUNT = 1.25f;
    const float POINTER_ENTER_ANIMATION_TIME = 0.1f;

    void Awake()
    {
        originScale = transform.localScale;

        image_CardBase.raycastTarget = true;
        text_CardNumber.raycastTarget = true;
        image_CardSuit.raycastTarget = true;
        image_CardEffect.raycastTarget = true;
        image_CursedEffect.raycastTarget = true;
        image_CardFrame.raycastTarget = true;

        SetAlphaValue(0, 0);
    }

    public void SetAlphaValue(float value, float duration)
    {
        image_CardBase.DOFade(value, duration);
        text_CardNumber.DOFade(value, duration);
        image_CardSuit.DOFade(value, duration);
        image_CardEffect.DOFade(value, duration);
        image_CursedEffect.DOFade(value, duration);
        image_CardFrame.DOFade(value, duration);

        if (value == 0)
        {
            image_CardBase.raycastTarget = false;
            text_CardNumber.raycastTarget = false;
            image_CardSuit.raycastTarget = false;
            image_CardEffect.raycastTarget = false;
            image_CursedEffect.raycastTarget = false;
            image_CardFrame.raycastTarget = false;
        }
        else
        {
            image_CardBase.raycastTarget = true;
            text_CardNumber.raycastTarget = true;
            image_CardSuit.raycastTarget = true;
            image_CardEffect.raycastTarget = true;
            image_CursedEffect.raycastTarget = true;
            image_CardFrame.raycastTarget = true;
        }
    }

    bool isEnter;
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(originScale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverCardSystem.Instance.ActivePanelByStackCard(CardInfo, transform.position);

        isEnter = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnter)
        {
            transform.DOKill();

            transform.DOScale(originScale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverCardSystem.Instance.DisablePanelOnCard();

            isEnter = false;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // 전개 카드 효과 숨기기
        S_UICardEffecter.Instance.DeshowExpansionCards();

        // 카드 내기
        S_HitBtnSystem.Instance.SelectHitCardByExpansion(CardInfo);
    }
}
