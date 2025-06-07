using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_OizysTheRebel : S_Foe
{
    public Foe_OizysTheRebel() : base
    (
        "Foe_OizysTheRebel",
        "격동하는 오이지스",
        "스탠드 시 의지를 모두 잃습니다.",
        S_FoeTypeEnum.Lachesis_Elite,
        S_FoeAbilityConditionEnum.Stand,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.AddOrSubtractDetermination(this, null, -3);
    }
    public override void CheckMeetCondition(S_Card card = null)
    {
        IsMeetCondition = true;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}";
    }
    public override S_Foe Clone()
    {
        return new Foe_OizysTheRebel();
    }
}
