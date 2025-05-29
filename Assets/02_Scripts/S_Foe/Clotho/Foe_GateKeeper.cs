using System.Threading.Tasks;
using UnityEngine;

public class Foe_GateKeeper : S_Foe
{
    public Foe_GateKeeper() : base
    (
        "Foe_GateKeeper",
        "������",
        "�÷� ���� �� ���ڰ� 1�� ī�带 ��� �����մϴ�.",
        S_FoeTypeEnum.Clotho,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        return base.IsMeetCondition(card);
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.CurseRandomCards(this, 999, S_CardSuitEnum.None, 1);
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        base.ActivateCount(card, isTwist);
    }
    public override string GetDescription()
    {
        return base.GetDescription();
    }
    public override S_Foe Clone()
    {
        return new Foe_GateKeeper();
    }
}
