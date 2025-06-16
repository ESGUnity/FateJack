using UnityEngine;

public class Foe_ClothoFateWeaver : S_Foe
{
    public Foe_ClothoFateWeaver() : base
    (
        "Foe_ClothoFateWeaver",
        "운명을 짓는 자 클로토",
        "스택에 없는 문양이 있다면 플레이어를 공격 시 즉시 처치합니다.",
        S_FoeTypeEnum.Clotho_Boss,
        S_FoeAbilityConditionEnum.DeathAttack,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_EffectChecker.Instance.GetGrandChaosInPreStack(1);
        IsMeetCondition = ActivatedCount < 4;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n스택에 있는 문양 개수 : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_ClothoFateWeaver();
    }
}
