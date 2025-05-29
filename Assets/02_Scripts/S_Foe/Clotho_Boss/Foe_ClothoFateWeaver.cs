using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_ClothoFateWeaver : S_Foe
{
    public Foe_ClothoFateWeaver() : base
    (
        "Foe_ClothoFateWeaver",
        "운명을 짓는 자 클로토",
        "스택에 각 문양의 카드가 1장 이상 없다면 플레이어를 공격 시 즉시 처치합니다.",
        S_FoeTypeEnum.Clotho_Boss,
        S_FoeAbilityConditionEnum.DeathAttack,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount < 4;
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
        return new Foe_ClothoFateWeaver();
    }
}
