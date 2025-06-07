using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_SacredHerald : S_Foe
{
    public Foe_SacredHerald() : base
    (
        "Foe_SacredHerald",
        "신성한 사자",
        "시련 시작 시 골드가 10 이상이라면 체력을 1 잃습니다.",
        S_FoeTypeEnum.Atropos,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (S_PlayerStat.Instance.CurrentGold >= 10)
        {
            await eA.AddOrSubtractHealth(this, null, -1);
        }
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
        return new Foe_SacredHerald();
    }
}
