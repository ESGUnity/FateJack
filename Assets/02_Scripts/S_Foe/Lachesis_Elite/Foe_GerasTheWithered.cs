using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_GerasTheWithered : S_Foe
{
    public Foe_GerasTheWithered() : base
    (
        "Foe_GerasTheWithered",
        "노쇠한 게라스",
        "시련 시작 시 덱에서 가장 적은 문양의 카드를 모두 저주합니다.",
        S_FoeTypeEnum.Lachesis_Elite,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        S_EffectChecker.Instance.GetLeastSuitCardsInDeck(out S_CardSuitEnum suit);

        await eA.CurseRandomCards(this, 999, suit, -1, true, false);
    }
    public override void CheckMeetCondition(S_Card card = null)
    {
        IsMeetCondition = false;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}";
    }
    public override S_Foe Clone()
    {
        return new Foe_GerasTheWithered();
    }
}
