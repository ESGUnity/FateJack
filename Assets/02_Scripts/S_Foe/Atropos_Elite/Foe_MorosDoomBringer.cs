using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_MorosDoomBringer : S_Foe
{
    public Foe_MorosDoomBringer() : base
    (
        "Foe_MorosDoomBringer",
        "�ĸ��� �θ��� ��ν�",
        "ī�带 3�� ��Ʈ�� ������ ��带 4 �ҽ��ϴ�.",
        S_FoeTypeEnum.Atropos_Elite,
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
            await eA.AddOrSubtractGold(this, null, -4);

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
        return $"{AbilityDescription}\n{ActivatedCount}�� °";
    }
    public override S_Foe Clone()
    {
        return new Foe_MorosDoomBringer();
    }
}
