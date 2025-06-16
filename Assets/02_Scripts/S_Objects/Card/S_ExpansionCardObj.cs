using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_ExpansionCardObj : S_CardObj, IPointerClickHandler
{

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.HittingCard };
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        // 전개 종료
        await S_HitBtnSystem.Instance.EndExpansion();

        // 카드 내기
        await S_HitBtnSystem.Instance.SelectHitCard(CardInfo);
    }
}
