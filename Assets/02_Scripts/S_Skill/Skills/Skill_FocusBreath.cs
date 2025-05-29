using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_FocusBreath : S_Skill
{
    public Skill_FocusBreath() : base
    (
        "Skill_FocusBreath",
        "전집중 호흡",
        "카드를 3장 히트할 때마다 전개를 얻습니다.",
        S_SkillConditionEnum.Reverb,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 3;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.GetExpansion(this, new List<S_Card>() { hitCard });

            ActivatedCount = 0;
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        if (isTwist)
        {
            ActivatedCount--;
            if (ActivatedCount <= 0)
            {
                ActivatedCount = 3;
            }
        }
        else
        {
            ActivatedCount++;
        }
    }
    public override string GetDescription()
    {
        return $"{Description}\n{ActivatedCount}장 째";
    }
    public override S_Skill Clone()
    {
        return new Skill_FocusBreath();
    }
}
