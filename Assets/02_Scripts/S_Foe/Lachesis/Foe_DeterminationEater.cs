using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_DeterminationEater : S_Foe
{
    public Foe_DeterminationEater() : base
    (
        "Foe_DeterminationEater",
        "������ �Դ� ��",
        "�÷� ���� �� ������ 1 �ҽ��ϴ�.",
        S_FoeTypeEnum.Lachesis,
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
        await eA.AddOrSubtractDetermination(this, null, -1);
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
        return new Foe_DeterminationEater();
    }
}
