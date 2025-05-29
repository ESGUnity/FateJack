using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_Abracadabra : S_Skill
{
    public Skill_Abracadabra() : base
    (
        "Skill_Abracadabra",
        "아브라카다브라",
        "스택에 각 문양의 카드가 4장 이상 있다면 스탠드 시 모든 능력치를 곱한만큼 피해를 줍니다.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 4;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.HarmCreature(this, new List<S_Card>(), S_BattleStatEnum.AllStat, S_PlayerStat.Instance.CurrentStrength * S_PlayerStat.Instance.CurrentMind * S_PlayerStat.Instance.CurrentLuck);
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(4);
    }
    public override void StartNewTurn(int currentTrial)
    {

    }
    public override string GetDescription()
    {
        return $"{Description}\n충족한 문양 개수 : {ActivatedCount}";
    }
    public override S_Skill Clone()
    {
        return new Skill_Abracadabra();
    }
}
