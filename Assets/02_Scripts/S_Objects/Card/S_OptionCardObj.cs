using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_OptionCardObj : S_CardObj, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] SpriteRenderer sprite_BlurCard;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.StoreBuying };
    }

    public override void SetOrder(int order)
    {
        base.SetOrder(order);

        sprite_BlurCard.sortingLayerName = "WorldObject";
        sprite_BlurCard.sortingOrder = order + 8;
    }
    public override void SetAlphaValue(float value, float duration)
    {
        base.SetAlphaValue(value, duration);

        sprite_BlurCard.DOKill();
        sprite_BlurCard.DOFade(value, duration);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            sprite_BlurCard.gameObject.SetActive(false);
        }
    }
    public override void ForceExit()
    {
        base.ForceExit();

        sprite_BlurCard.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        S_StoreInfoSystem.Instance.DecideSelectCard(CardInfo);
    }
}
