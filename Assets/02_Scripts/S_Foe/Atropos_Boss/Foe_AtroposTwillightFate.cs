using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_AtroposTwillightFate : S_Foe
{
    public Foe_AtroposTwillightFate() : base
    (
        "Foe_AtroposTwillightFate",
        "저무는 운명의 아트로포스",
        "50%의 확률로 카드를 낼 때 저주받습니다.",
        S_FoeTypeEnum.Atropos_Boss,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {

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
        return new Foe_AtroposTwillightFate();
    }
}
