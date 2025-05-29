using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_Overwhelm : S_Skill
{
    public Skill_Overwhelm() : base
    (
        "Skill_Overwhelm",
        "압도",
        "스택에 스페이드 카드가 정확히 6의 배수만큼 있다면 스탠드 시 힘을 2배 증가시킵니다.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount % 6 == 0 ? true : false;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.AddBattleStats(this, new List<S_Card>(), S_BattleStatEnum.Strength, 2);
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSameSuitCardsInStack(S_CardSuitEnum.Spade).Count;
    }
    public override void StartNewTurn(int currentTrial)
    {

    }
    public override string GetDescription()
    {
        return $"{Description}\n{ActivatedCount}장 째";
    }
    public override S_Skill Clone()
    {
        return new Skill_Overwhelm();
    }
}
