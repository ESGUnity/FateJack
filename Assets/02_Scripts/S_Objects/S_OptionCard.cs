using UnityEngine;
using UnityEngine.EventSystems;

public class S_OptionCard : S_DeckCard
{
    [HideInInspector] public bool IsSelectedOptionCard; // 구매할 카드 선택 시

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.StoreByRemove)
        {
            if (IsSelectedByRemoveCard)
            {
                S_StoreInfoSystem.Instance.CancelSelectCardByStore(CardInfo);
                IsSelectedByRemoveCard = false;
            }
            else
            {
                S_StoreInfoSystem.Instance.SelectCardByRemoveCard(CardInfo);
                IsSelectedByRemoveCard = true;
            }
        }
        else if (IsSelectedOptionCard)
        {
            S_StoreInfoSystem.Instance.CancelSelectCardByStore(CardInfo);
            IsSelectedOptionCard = false;
        }
        else
        {
            S_StoreInfoSystem.Instance.SelectCardByOption(CardInfo);
            IsSelectedOptionCard = true;
        }
    }
}
