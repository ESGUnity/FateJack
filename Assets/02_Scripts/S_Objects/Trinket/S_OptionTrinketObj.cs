using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_OptionTrinketObj : S_TrinketObj, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] Material m_Option;
    [SerializeField] Material m_Origin;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.StoreBuying };
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            sprite_Trinket.material = m_Option;
        }
    }
    public override void ForceExit()
    {
        base.ForceExit();

        sprite_Trinket.material = m_Origin;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        S_StoreInfoSystem.Instance.DecideTrinketOption(TrinketInfo);
    }
}
