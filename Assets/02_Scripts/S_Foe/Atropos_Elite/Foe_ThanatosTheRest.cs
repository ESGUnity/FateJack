using UnityEngine;

public class Foe_ThanatosTheRest : S_Foe
{
    public Foe_ThanatosTheRest() : base
    (
        "Foe_ThanatosTheRest",
        "안식의 타나토스",
        "스택에 문양이 2개 이하라면 플레이어 공격 시 즉시 처치합니다.",
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
        return $"{AbilityDescription}\n스택에 존재하는 문양 개수 : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_ThanatosTheRest();
    }
}
