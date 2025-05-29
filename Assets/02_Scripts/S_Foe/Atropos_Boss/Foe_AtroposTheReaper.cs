using System.Threading.Tasks;
using UnityEngine;

public class Foe_AtroposTheReaper : S_Foe
{
    public Foe_AtroposTheReaper() : base
    (
        "Foe_AtroposTheReaper",
        "�ŵδ� �� ��Ʈ������",
        "ī�带 2�� ��Ʈ�� ������ ������ ����ϴ�.",
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
        return $"{AbilityDescription}\n{ActivatedCount}�� °";
    }
    public override S_Foe Clone()
    {
        return new Foe_AtroposTheReaper();
    }
}
