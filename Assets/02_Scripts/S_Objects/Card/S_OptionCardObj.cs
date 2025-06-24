using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_OptionCardObj : S_CardObj, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] SpriteRenderer sprite_BlurEffect;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.StoreBuying };
    }

    public override void SetOrder(int order)
    {
        base.SetOrder(order);

        sprite_BlurEffect.sortingLayerName = "WorldObject";
        sprite_BlurEffect.sortingOrder = order + 8;
    }
    public override void SetAlphaValue(float value, float duration)
    {
        base.SetAlphaValue(value, duration);

        sprite_BlurEffect.DOKill();
        sprite_BlurEffect.material.DOFloat(value, "_AlphaValue", duration);
    }
    public override Sequence SetAlphaValueAsync(float value, float duration)
    {
        sprite_BlurEffect.DOKill();

        return base.SetAlphaValueAsync(value, duration).Join(sprite_BlurEffect.material.DOFloat(value, "_AlphaValue", duration));
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            sprite_BlurEffect.gameObject.SetActive(false);
        }
    }
    public override void ForceExit()
    {
        base.ForceExit();

        sprite_BlurEffect.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        S_StoreInfoSystem.Instance.DecideSelectCard(CardInfo);
    }
}
