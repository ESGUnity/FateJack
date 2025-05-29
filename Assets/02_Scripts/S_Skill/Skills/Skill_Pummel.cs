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
        "한 턴에 정확히 카드를 3장만 히트하고 스탠드 시 무작위 능력치 2개를 곱한만큼 피해를 3번 줍니다.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount == 3;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            List<S_BattleStatEnum> list = new() { S_BattleStatEnum.Strength_Mind, S_BattleStatEnum.Strength_Luck, S_BattleStatEnum.Mind_Luck };
            S_BattleStatEnum stat = list[Random.Range(0, list.Count)];

            for (int i = 0; i < 3; i++)
            {
                stat = list[Random.Range(0, list.Count)];

                switch (stat)
                {
                    case S_BattleStatEnum.Strength_Mind:
                        await eA.HarmCreature(this, new List<S_Card>(), stat, S_PlayerStat.Instance.CurrentStrength * S_PlayerStat.Instance.CurrentMind);
                        break;
                    case S_BattleStatEnum.Strength_Luck:
                        await eA.HarmCreature(this, new List<S_Card>(), stat, S_PlayerStat.Instance.CurrentStrength * S_PlayerStat.Instance.CurrentLuck);
                        break;
                    case S_BattleStatEnum.Mind_Luck:
                        await eA.HarmCreature(this, new List<S_Card>(), stat, S_PlayerStat.Instance.CurrentMind * S_PlayerStat.Instance.CurrentLuck);
                        break;
                }
            }
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        if (isTwist)
        {
            ActivatedCount = 0;
        }
        else
        {
            int currentHitCount = S_PlayerCard.Instance.GetPreStackCards()
                .Where(x => x.IsCurrentTurnHit)
                .Count();

            ActivatedCount = currentHitCount;
        }
    }
    public override void StartNewTurn(int currentTrial)
    {
        ActivatedCount = 0;
    }
    public override string GetDescription()
    {
        return $"{Description}\n{ActivatedCount}장 째";
    }
    public override S_Skill Clone()
    {
        return new Skill_Pummel();
    }
}
