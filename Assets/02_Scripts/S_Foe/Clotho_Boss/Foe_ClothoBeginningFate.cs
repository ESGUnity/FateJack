using System.Linq;
using UnityEngine;

public class Foe_ClothoBeginningFate : S_Foe
{
    public Foe_ClothoBeginningFate() : base
    (
        "Foe_ClothoBeginningFate",
        "�����ϴ� ����� Ŭ����",
        "�� �Ͽ� ī�带 4�� ���ϸ� ��Ʈ�ߴٸ� �÷��̾ ���� �� ��� óġ�մϴ�.",
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
        ActivatedCount = S_PlayerCard.Instance.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Count();
    }
    public override void StartNewTurn(int currentTrial)
    {
        ActivatedCount = 0;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n�̹� �Ͽ� ��Ʈ�� ī�� ���� : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_ClothoBeginningFate();
    }
}
