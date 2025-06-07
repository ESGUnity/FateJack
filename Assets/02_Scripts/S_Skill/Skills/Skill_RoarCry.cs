using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_RoarCry : S_Skill
{
    public Skill_RoarCry() : base
    (
        "Skill_RoarCry",
        "우렁찬 함성",
        "스페이드 카드를 낼 때마다 힘을 5 얻습니다.",
        S_SkillConditionEnum.Reverb,
        S_SkillPassiveEnum.None,
        false
    ) { }

    public override void CheckMeetConditionByBasic(S_Card card = null)
    {
        if (card == null)
        {
            IsMeetCondition = false;
        }
        else
        {
            IsMeetCondition = S_EffectChecker.Instance.IsSameSuit(card.Suit, S_CardSuitEnum.Spade);
        }
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            await eA.AddBattleStats(this, hitCard, S_BattleStatEnum.Strength, 5);
        }
    }
    public override S_Skill Clone()
    {
        return new Skill_RoarCry();
    }
}
