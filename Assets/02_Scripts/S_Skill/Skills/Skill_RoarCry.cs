using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_RoarCry : S_Skill
{
    public Skill_RoarCry() : base
    (
        "Skill_RoarCry",
        "�췷�� �Լ�",
        "�����̵� ī�带 �� ������ ���� 5 ����ϴ�.",
        S_SkillConditionEnum.Reverb,
        S_SkillPassiveEnum.None,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        if (card == null)
        {
            CanActivateEffect = false;
        }
        else
        {
            CanActivateEffect = S_EffectChecker.Instance.IsSameSuit(card.Suit, S_CardSuitEnum.Spade);
        }

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.AddBattleStats(this, new List<S_Card>() { hitCard }, S_BattleStatEnum.Strength, 5);
        }
    }
    public override S_Skill Clone()
    {
        return new Skill_RoarCry();
    }
}
