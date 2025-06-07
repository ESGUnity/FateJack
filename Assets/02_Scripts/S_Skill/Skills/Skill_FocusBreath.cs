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

    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            await eA.GetExpansion(this, hitCard);

            ActivatedCount = 0;
            IsMeetCondition = false;
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        int count  = S_PlayerCard.Instance.GetPreStackCards().Count % 3;

        if (S_PlayerCard.Instance.GetPreStackCards().Count == 0)
        {
            ActivatedCount = 0;
        }
        else
        {
            if (count == 0)
            {
                ActivatedCount = 3;
            }
            else
            {
                ActivatedCount = count;
            }
        }

        IsMeetCondition = ActivatedCount >= 3;
    }
    public override string GetDescription()
    {
        return $"{Description}\n히트한 카드 : {ActivatedCount}장 째";
    }
    public override S_Skill Clone()
    {
        return new Skill_FocusBreath();
    }
}
