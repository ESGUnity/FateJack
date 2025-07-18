using UnityEngine.EventSystems;

public class S_ExpansionCardObj : S_CardObj, IPointerClickHandler
{
    bool isClicked = false;
    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Expansion };
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        if (!isClicked)
        {
            isClicked = true;

            // 전개 종료
            S_HitBtnSystem.Instance.EndExpansion();

            // 카드 내기
            await S_HitBtnSystem.Instance.SelectHitCard(CardInfo);
        }
    }
}
