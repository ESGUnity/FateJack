using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_Abracadabra : S_Skill
{
    public Skill_Abracadabra() : base
    (
        "Skill_Abracadabra",
        "�ƺ��ī�ٺ��",
        "���ÿ� �� ������ ī�尡 4�� �̻� �ִٸ� ���ĵ� �� ��� �ɷ�ġ�� ���Ѹ�ŭ ���ظ� �ݴϴ�.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 4;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.HarmCreature(this, new List<S_Card>(), S_BattleStatEnum.AllStat, S_PlayerStat.Instance.CurrentStrength * S_PlayerStat.Instance.CurrentMind * S_PlayerStat.Instance.CurrentLuck);
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(4);
    }
    public override void StartNewTurn(int currentTrial)
    {

    }
    public override string GetDescription()
    {
        return $"{Description}\n������ ���� ���� : {ActivatedCount}";
    }
    public override S_Skill Clone()
    {
        return new Skill_Abracadabra();
    }
}
