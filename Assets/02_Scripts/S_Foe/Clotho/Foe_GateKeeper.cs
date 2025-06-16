using System.Threading.Tasks;
using UnityEngine;

public class Foe_GateKeeper : S_Foe
{
    public Foe_GateKeeper() : base
    (
        "Foe_GateKeeper",
        "문지기",
        "시련 시작 시 숫자가 1인 카드를 모두 저주합니다.",
        S_FoeTypeEnum.Clotho,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }


    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.CurseRandomCards(this, 999, S_EngravingEnum.None, 1);
    }
    public override void CheckMeetCondition(S_Card card = null)
    {
        IsMeetCondition = false;
    }
    public override string GetDescription()
    {
        return AbilityDescription;
    }
    public override S_Foe Clone()
    {
        return new Foe_GateKeeper();
    }
}
