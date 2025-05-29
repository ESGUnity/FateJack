using System.Threading.Tasks;
using UnityEngine;

public class Foe_AtroposTheReaper : S_Foe
{
    public Foe_AtroposTheReaper() : base
    (
        "Foe_AtroposTheReaper",
        "거두는 자 아트로포스",
        "카드를 2장 히트할 때마다 망상을 얻습니다.",
        S_FoeTypeEnum.Atropos_Boss,
        S_FoeAbilityConditionEnum.Reverb,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 2;

        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.GetDelusion(this, null);

            ActivatedCount = 0;
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        if (isTwist)
        {
            ActivatedCount--;
            if (ActivatedCount <= 0)
            {
                ActivatedCount = 3;
            }
        }
        else
        {
            ActivatedCount++;
        }
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n{ActivatedCount}장 째";
    }
    public override S_Foe Clone()
    {
        return new Foe_AtroposTheReaper();
    }
}
