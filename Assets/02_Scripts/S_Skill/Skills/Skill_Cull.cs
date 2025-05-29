using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_Cull : S_Skill
{
    public Skill_Cull() : base
    (
        "Skill_Cull",
        "����",
        "���ĵ� �� ������ �ɷ�ġ 2���� ���Ѹ�ŭ ���ظ� �ݴϴ�.\n�� �ϸ��� �ɷ�ġ�� ����˴ϴ�.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.None,
        false
    ) { }

    S_BattleStatEnum stat = S_BattleStatEnum.Strength_Mind;

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = true;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
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
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {

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
                str = $"���ĵ� �� ���� ���ŷ��� ���Ѹ�ŭ ���ظ� �ݴϴ�.\n�� �ϸ��� �ɷ�ġ�� ����˴ϴ�.";
                break;
            case S_BattleStatEnum.Strength_Luck:
                str = $"���ĵ� �� ���� ����� ���Ѹ�ŭ ���ظ� �ݴϴ�.\n�� �ϸ��� �ɷ�ġ�� ����˴ϴ�.";
                break;
            case S_BattleStatEnum.Mind_Luck:
                str = $"���ĵ� �� ���ŷ°� ��� ���Ѹ�ŭ ���ظ� �ݴϴ�.\n�� �ϸ��� �ɷ�ġ�� ����˴ϴ�.";
                break;
        }

        return str;
    }
    public override S_Skill Clone()
    {
        return new Skill_Cull();
    }
}
