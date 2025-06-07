using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_AtroposTwillightFate : S_Foe
{
    public Foe_AtroposTwillightFate() : base
    (
        "Foe_AtroposTwillightFate",
        "저무는 운명의 아트로포스",
        "시련 시작 시 무작위 카드 절반을 제외합니다.",
        S_FoeTypeEnum.Atropos_Boss,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.ExclusionRandomCard(this, S_PlayerCard.Instance.GetImmediateDeckCards().Count / 2);
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
