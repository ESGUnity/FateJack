using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_Overwhelm : S_Skill
{
    public Skill_Overwhelm() : base
    (
        "Skill_Overwhelm",
        "�е�",
        "���ÿ� �����̵� ī�尡 ��Ȯ�� 6�� �����ŭ �ִٸ� ���ĵ� �� ���� 2�� ������ŵ�ϴ�.",
        S_SkillConditionEnum.Stand,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount % 6 == 0 ? true : false;

        return CanActivateEffect;
    }
    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.AddBattleStats(this, new List<S_Card>(), S_BattleStatEnum.Strength, 2);
        }
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSameSuitCardsInStack(S_CardSuitEnum.Spade).Count;
    }
    public override void StartNewTurn(int currentTrial)
    {

    }
    public override string GetDescription()
    {
        return $"{Description}\n{ActivatedCount}�� °";
    }
    public override S_Skill Clone()
    {
        return new Skill_Overwhelm();
    }
}
