using UnityEngine;

public class Foe_ThanatosTheRest : S_Foe
{
    public Foe_ThanatosTheRest() : base
    (
        "Foe_ThanatosTheRest",
        "�Ƚ��� Ÿ���佺",
        "���ÿ� ������ 2�� ���϶�� �÷��̾� ���� �� ��� óġ�մϴ�.",
        S_FoeTypeEnum.Atropos_Elite,
        S_FoeAbilityConditionEnum.DeathAttack,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount <= 2;
        return CanActivateEffect;
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1);
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n���ÿ� �����ϴ� ���� ���� : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_ThanatosTheRest();
    }
}
