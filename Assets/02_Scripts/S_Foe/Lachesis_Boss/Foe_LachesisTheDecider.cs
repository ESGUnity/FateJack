using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_LachesisTheDecider : S_Foe
{
    public Foe_LachesisTheDecider() : base
    (
        "Foe_LachesisTheDecider",
        "정하는 자 라케시스",
        "한 턴에 카드를 8장 이상 히트했다면 스탠드 시 덱에 있는 카드 절반을 저주합니다.",
        S_FoeTypeEnum.Lachesis_Boss,
        S_FoeAbilityConditionEnum.Stand,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = ActivatedCount >= 8;
        return CanActivateEffect;
    }
    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (CanActivateEffect)
        {
            await eA.CurseRandomCards(this, S_PlayerCard.Instance.GetImmediateDeckCards().Count / 2, S_CardSuitEnum.None, -1, true, false);
        }
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
        return new Foe_LachesisTheDecider();
    }
}
