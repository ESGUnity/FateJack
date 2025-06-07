using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_Devourer : S_Foe
{
    public Foe_Devourer() : base
    (
        "Foe_Devourer",
        "탐식자",
        "시련 시작 시 무작위 카드 4장을 제외합니다.",
        S_FoeTypeEnum.Lachesis,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.ExclusionRandomCard(this, 4);
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
        return new Foe_Devourer();
    }
}
