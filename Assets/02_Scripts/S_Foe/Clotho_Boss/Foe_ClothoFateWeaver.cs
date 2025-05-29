using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_ClothoFateWeaver : S_Foe
{
    public Foe_ClothoFateWeaver() : base
    (
        "Foe_ClothoFateWeaver",
        "����� ���� �� Ŭ����",
        "���ÿ� �� ������ ī�尡 1�� �̻� ���ٸ� �÷��̾ ���� �� ��� óġ�մϴ�.",
        S_FoeTypeEnum.Clotho_Boss,
        S_FoeAbilityConditionEnum.DeathAttack,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount < 4;
        return CanActivateEffect;
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1);
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n���ÿ� �����ϴ� ���� ���� : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_ClothoFateWeaver();
    }
}
