using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_AtroposTwillightFate : S_Foe
{
    public Foe_AtroposTwillightFate() : base
    (
        "Foe_AtroposTwillightFate",
        "������ ����� ��Ʈ������",
        "�÷� ���� �� ������ ī�� ������ �����մϴ�.",
        S_FoeTypeEnum.Atropos_Boss,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = false;
        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.ExclusionRandomCard(this, S_PlayerCard.Instance.GetImmediateDeckCards().Count / 2);
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {

    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}";
    }
    public override S_Foe Clone()
    {
        return new Foe_AtroposTwillightFate();
    }
}
