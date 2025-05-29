using UnityEngine;

public class Foe_LachesisFreedomFate : S_Foe
{
    public Foe_LachesisFreedomFate() : base
    (
        "Foe_LachesisFreedomFate",
        "�����ο� ����� ���ɽý�",
        "���ÿ� ī�尡 13�� �̻��̶�� �÷��̾ ���� �� ��� óġ�մϴ�.",
        S_FoeTypeEnum.Lachesis_Boss,
        S_FoeAbilityConditionEnum.DeathAttack,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 13;
        return CanActivateEffect;
    }
    public override void ActivateCount(S_Card card, bool isTwist = false)
    {
        ActivatedCount = S_PlayerCard.Instance.GetPreStackCards().Count;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n���ÿ� �ִ� ī�� ���� : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_LachesisFreedomFate();
    }
}
