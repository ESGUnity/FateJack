using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_QuadBlade : S_Skill
{
    public Skill_QuadBlade() : base
    (
        "Skill_QuadBlade",
        "�絵��",
        "�� �Ͽ� 1�� �����̵� ī�带 4�� �̻� ��Ʈ�� �� ���� 1.5�� ������ŵ�ϴ�.",
        S_SkillConditionEnum.Reverb,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    bool WasActivatedThisTurn = false;

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 4;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect && !WasActivatedThisTurn)
        {
            int amount = (int)System.Math.Round(S_PlayerStat.Instance.CurrentStrength * 0.5f, System.MidpointRounding.AwayFromZero);

            await eA.AddBattleStats(this, new List<S_Card>() { hitCard }, S_BattleStatEnum.Strength, amount);

            WasActivatedThisTurn = true;
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
            ActivatedCount = S_EffectChecker.Instance.GetSameSuitCardsInStackInCurrentTurn(S_CardSuitEnum.Spade).Count;
        }
    }
    public override void StartNewTurn(int currentTrial)
    {
        ActivatedCount = 0;
        WasActivatedThisTurn = false;
    }
    public override string GetDescription()
    {
        return $"{Description}\n{ActivatedCount}�� °";
    }
    public override S_Skill Clone()
    {
        return new Skill_QuadBlade();
    }
}
