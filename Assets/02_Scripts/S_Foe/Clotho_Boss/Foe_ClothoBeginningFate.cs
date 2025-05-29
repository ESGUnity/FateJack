using System.Linq;
using UnityEngine;

public class Foe_ClothoBeginningFate : S_Foe
{
    public Foe_ClothoBeginningFate() : base
    (
        "Foe_ClothoBeginningFate",
        "시작하는 운명의 클로토",
        "한 턴에 카드를 4장 이하만 히트했다면 플레이어를 공격 시 즉시 처치합니다.",
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
        return $"{AbilityDescription}\n이번 턴에 히트한 카드 개수 : {ActivatedCount}";
    }
    public override S_Foe Clone()
    {
        return new Foe_ClothoBeginningFate();
    }
}
