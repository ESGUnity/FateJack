using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_UnstableFusion : S_Skill
{
    public Skill_UnstableFusion() : base
    (
        "Skill_UnstableFusion",
        "�Ҿ����� ����",
        "�����̵�� Ŭ�ι��� ���� �������� ����մϴ�.",
        S_SkillConditionEnum.None,
        S_SkillPassiveEnum.SameBlack,
        false
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = false;

        return CanActivateEffect;
    }
    public override string GetDescription()
    {
        return $"{Description}";
    }
    public override S_Skill Clone()
    {
        return new Skill_UnstableFusion();
    }
}
