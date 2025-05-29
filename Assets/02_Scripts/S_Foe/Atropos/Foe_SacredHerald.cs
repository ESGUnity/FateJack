using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_SacredHerald : S_Foe
{
    public Foe_SacredHerald() : base
    (
        "Foe_SacredHerald",
        "�ż��� ����",
        "�÷� ���� �� ��尡 10 �̻��̶�� ü���� 1 �ҽ��ϴ�.",
        S_FoeTypeEnum.Atropos,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = false;
        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (S_PlayerStat.Instance.CurrentGold >= 10)
        {
            await eA.AddOrSubtractHealth(this, null, -1);
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {

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
