using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_OneiroiDarknessEater : S_Foe
{
    public Foe_OneiroiDarknessEater() : base
    (
        "Foe_OneiroiDarknessEater",
        "����� �Դ� �����̷���",
        "���ĵ� �� ���ÿ��� ���� ���� ������ ī�带 ��� �����մϴ�.",
        S_FoeTypeEnum.Clotho_Elite,
        S_FoeAbilityConditionEnum.Stand,
        S_FoePassiveEnum.None
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = true;
        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        S_EffectChecker.Instance.GetLeastSuitCardsInStack(out S_CardSuitEnum suit);

        await eA.CurseRandomCards(this, 999, suit, -1, false, true);
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
        return new Foe_OneiroiDarknessEater();
    }
}
