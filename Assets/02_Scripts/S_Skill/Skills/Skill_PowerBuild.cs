using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_PowerBuild : S_Skill
{
    public Skill_PowerBuild() : base
    (
        "Skill_PowerBuild",
        "�ٷ� �Ʒ�",
        "�����̵� ī�带 3�� ��Ʈ�� ������ ���� 2 ��� �����˴ϴ�.",
        S_SkillConditionEnum.Reverb,
        S_SkillPassiveEnum.NeedActivatedCount,
        true
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 3;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            AccumulateValue += 2;
            await eA.AddBattleStats(this, new List<S_Card>() { hitCard }, S_BattleStatEnum.Strength, 2);

            ActivatedCount = 0;
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        if (S_EffectChecker.Instance.IsSameSuit(card.Suit, S_CardSuitEnum.Spade))
        {
            if (isTwist)
            {
                ActivatedCount--;
                if (ActivatedCount <= 0)
                {
                    ActivatedCount = 3;

                    AccumulateValue -= 2;
                }
            }
            else
            {
                ActivatedCount++;
            }
        }
    }
    public override void StartNewTurn(int currentTrial)
    {
        if (currentTrial == 1)
        {
            S_PlayerStat.Instance.AddStrength(AccumulateValue); // �����̱⿡ �÷� ���� �� ������ ���س���
        }
    }
    public override string GetDescription()
    {
        return $"{Description}\n{ActivatedCount}�� °\n������ : {AccumulateValue}";
    }
    public override S_Skill Clone()
    {
        return new Skill_PowerBuild();
    }
}
