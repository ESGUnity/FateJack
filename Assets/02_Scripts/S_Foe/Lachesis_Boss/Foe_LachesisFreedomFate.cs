using UnityEngine;

public class Foe_LachesisFreedomFate : S_Foe
{
    public Foe_LachesisFreedomFate() : base
    (
        "Foe_LachesisFreedomFate",
        "자유로운 운명의 라케시스",
        "스택에 카드가 13장 이상이라면 플레이어를 공격 시 즉시 처치합니다.",
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
        return $"{AbilityDescription}\n스택에 있는 카드 개수 : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_LachesisFreedomFate();
    }
}
