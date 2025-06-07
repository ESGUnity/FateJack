using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_SavageStrike : S_Skill
{
    public Skill_SavageStrike() : base
    (
        "Skill_SavageStrike",
        "야만적인 타격",
        "스탠드 시 스택의 스페이드 카드 숫자 합 10 당 힘만큼 피해를 줍니다.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            int count = ActivatedCount / 10;

            for (int i = 0; i < count; i++)
            {
                await eA.HarmFoe(this, null, S_BattleStatEnum.Strength, S_PlayerStat.Instance.CurrentStrength);
            }
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSameSuitSumInStack(S_CardSuitEnum.Spade);

        IsMeetCondition = ActivatedCount >= 10;
    }
    public override string GetDescription()
    {
        return $"{Description}\n스페이드 카드 숫자 합 : {ActivatedCount}";
    }
    public override S_Skill Clone()
    {
        return new Skill_SavageStrike();
    }
}
