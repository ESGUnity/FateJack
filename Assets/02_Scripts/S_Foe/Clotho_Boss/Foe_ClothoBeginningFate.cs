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

    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_PlayerCard.Instance.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Count();
        IsMeetCondition = ActivatedCount <= 4;
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
