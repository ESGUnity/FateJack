using System.Threading.Tasks;
using UnityEngine;

public class Foe_DeterminationEater : S_Foe
{
    public Foe_DeterminationEater() : base
    (
        "Foe_DeterminationEater",
        "의지를 먹는 자",
        "시련 시작 시 의지를 1 잃습니다.",
        S_FoeTypeEnum.Lachesis,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }


    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.AddOrSubtractDetermination(this, null, -1);
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
        return new Foe_DeterminationEater();
    }
}
