using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_Cull : S_Skill
{
    public Skill_Cull() : base
    (
        "Skill_Cull",
        "도륙",
        "스탠드 시 무작위 능력치 2개를 곱한만큼 피해를 줍니다.\n매 턴마다 능력치가 변경됩니다.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.None,
        false
    ) { }

    S_BattleStatEnum stat = S_BattleStatEnum.Strength_Mind;


    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
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
    public override void CheckMeetConditionByBasic(S_Card card = null)
    {
        IsMeetCondition = true;
    }
    public override void StartNewTurn(int currentTrial)
    {
        List<S_BattleStatEnum> list = new() { S_BattleStatEnum.Strength_Mind, S_BattleStatEnum.Strength_Luck, S_BattleStatEnum.Mind_Luck };
        stat = list[Random.Range(0, list.Count)];
    }
    public override string GetDescription()
    {
        string str = "";

        switch (stat)
        {
            case S_BattleStatEnum.Strength_Mind:
                str = $"스탠드 시 힘과 정신력을 곱한만큼 피해를 줍니다.\n매 턴마다 능력치가 변경됩니다.";
                break;
            case S_BattleStatEnum.Strength_Luck:
                str = $"스탠드 시 힘과 행운을 곱한만큼 피해를 줍니다.\n매 턴마다 능력치가 변경됩니다.";
                break;
            case S_BattleStatEnum.Mind_Luck:
                str = $"스탠드 시 정신력과 행운 곱한만큼 피해를 줍니다.\n매 턴마다 능력치가 변경됩니다.";
                break;
        }

        return str;
    }
    public override S_Skill Clone()
    {
        return new Skill_Cull();
    }
}
