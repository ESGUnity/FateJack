using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_HypnosSleepWaker : S_Foe
{
    public Foe_HypnosSleepWaker() : base
    (
        "Foe_HypnosSleepWaker",
        "���� ���� �����뽺",
        "���ĵ� �� ���ÿ� ���� ������ �ִٸ� ������ ������ ī�� 6���� �����մϴ�.",
        S_FoeTypeEnum.Clotho_Elite,
        S_FoeAbilityConditionEnum.Stand,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount < 4;
        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.CurseRandomCards(this, 6, S_CardSuitEnum.None, -1, true, false);
        }
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
        return new Foe_HypnosSleepWaker();
    }
}
