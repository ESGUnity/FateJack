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

    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            await eA.AddBattleStats(this, null, S_BattleStatEnum.Strength, S_PlayerStat.Instance.CurrentStrength);
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSameSuitCardsInStack(S_CardSuitEnum.Spade).Count;

        IsMeetCondition = ActivatedCount > 0 && ActivatedCount % 6 == 0;
    }
    public override string GetDescription()
    {
        return $"{Description}\n스페이드 카드 : {ActivatedCount}장";
    }
    public override S_Skill Clone()
    {
        return new Skill_Overwhelm();
    }
}
