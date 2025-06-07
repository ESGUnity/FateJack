using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_Pummel : S_Skill
{
    public Skill_Pummel() : base
    (
        "Skill_Pummel",
        "난타",
        "한 턴에 카드를 정확히 3장만 히트하고 스탠드 시 무작위 능력치 2개를 곱한만큼 피해를 3번 줍니다.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            List<S_BattleStatEnum> list = new() { S_BattleStatEnum.Strength_Mind, S_BattleStatEnum.Strength_Luck, S_BattleStatEnum.Mind_Luck };
            S_BattleStatEnum stat;

            for (int i = 0; i < 3; i++)
            {
                stat = list[Random.Range(0, list.Count)];

                switch (stat)
                {
                    case S_BattleStatEnum.Strength_Mind:
                        await eA.HarmFoe(this, null, stat, S_PlayerStat.Instance.CurrentStrength * S_PlayerStat.Instance.CurrentMind);
                        break;
                    case S_BattleStatEnum.Strength_Luck:
                        await eA.HarmFoe(this, null, stat, S_PlayerStat.Instance.CurrentStrength * S_PlayerStat.Instance.CurrentLuck);
                        break;
                    case S_BattleStatEnum.Mind_Luck:
                        await eA.HarmFoe(this, null, stat, S_PlayerStat.Instance.CurrentMind * S_PlayerStat.Instance.CurrentLuck);
                        break;
                }
            }
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_PlayerCard.Instance.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Count();

        IsMeetCondition = ActivatedCount == 3;

    }
    public override void StartNewTurn(int currentTrial)
    {
        ActivatedCount = 0;
    }
    public override string GetDescription()
    {
        return $"{Description}\n히트한 카드 : {ActivatedCount}장";
    }
    public override S_Skill Clone()
    {
        return new Skill_Pummel();
    }
}
