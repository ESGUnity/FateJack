using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_GraveKeeper : S_Foe
{
    public Foe_GraveKeeper() : base
    (
        "Foe_GraveKeeper",
        "묘지기",
        "카드를 3장 히트할 때마다 덱에서 무작위 카드 1장을 제외합니다.",
        S_FoeTypeEnum.Atropos,
        S_FoeAbilityConditionEnum.Reverb,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 3;

        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            if (S_PlayerCard.Instance.GetPreDeckCards().Count > 0)
            {
                await eA.ExclusionRandomCard(this, 1);
            }

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
        return new Foe_GraveKeeper();
    }
}
