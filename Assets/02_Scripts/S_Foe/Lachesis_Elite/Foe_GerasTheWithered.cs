using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_GerasTheWithered : S_Foe
{
    public Foe_GerasTheWithered() : base
    (
        "Foe_GerasTheWithered",
        "����� �Զ�",
        "�÷� ���� �� ������ ���� ���� ������ ī�带 ��� �����մϴ�.",
        S_FoeTypeEnum.Lachesis_Elite,
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
        S_EffectChecker.Instance.GetLeastSuitCardsInDeck(out S_CardSuitEnum suit);

        await eA.CurseRandomCards(this, 999, suit, -1, true, false);
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
        return new Foe_GerasTheWithered();
    }
}
