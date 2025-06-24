using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_OptionTrinketObj : S_TrinketObj, IPointerEnterHandler, IPointerClickHandler
{
    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.StoreBuying };
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            S_HoverInfoSystem.Instance.ActivateHoverInfo(TrinketInfo, sprite_Trinket.gameObject, false);

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
        S_StoreInfoSystem.Instance.DecideTrinketOption(TrinketInfo);
    }
}
