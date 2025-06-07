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

    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1);

        IsMeetCondition = ActivatedCount <= 2;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n스택에 있는 문양 개수 : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_ThanatosTheRest();
    }
}
