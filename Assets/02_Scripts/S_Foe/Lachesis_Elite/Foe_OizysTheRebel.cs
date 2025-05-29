using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_OizysTheRebel : S_Foe
{
    public Foe_OizysTheRebel() : base
    (
        "Foe_OizysTheRebel",
        "�ݵ��ϴ� ��������",
        "���ĵ� �� ������ ��� �ҽ��ϴ�.",
        S_FoeTypeEnum.Lachesis_Elite,
        S_FoeAbilityConditionEnum.Stand,
        S_FoePassiveEnum.None
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = true;
        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.AddOrSubtractDetermination(this, null, -3);
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
        return new Foe_OizysTheRebel();
    }
}
