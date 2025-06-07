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

    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_PlayerCard.Instance.GetPreStackCards().Count;
        IsMeetCondition = ActivatedCount >= 13;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n히트한 카드 : {ActivatedCount}장";
    }
    public override S_Foe Clone()
    {
        return new Foe_LachesisFreedomFate();
    }
}
