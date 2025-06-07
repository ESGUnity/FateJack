using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_QuadBlade : S_Skill
{
    public Skill_QuadBlade() : base
    (
        "Skill_QuadBlade",
        "사도류",
        "한 턴에 1번 스페이드 카드를 4장 이상 히트할 때 힘을 1.5배 증가시킵니다.",
        S_SkillConditionEnum.Reverb,
        S_SkillPassiveEnum.NeedActivatedCount,
        false
    ) { }

    bool WasActivatedThisTurn = false;

    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition && !WasActivatedThisTurn)
        {
            int amount = (int)System.Math.Round(S_PlayerStat.Instance.CurrentStrength * 0.5f, System.MidpointRounding.AwayFromZero);

            await eA.AddBattleStats(this, hitCard, S_BattleStatEnum.Strength, amount);

            WasActivatedThisTurn = true;
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        ActivatedCount = S_EffectChecker.Instance.GetSameSuitCardsInStackInCurrentTurn(S_CardSuitEnum.Spade).Count;

        IsMeetCondition = ActivatedCount >= 4;
    }
    public override void StartNewTurn(int currentTrial)
    {
        ActivatedCount = 0;
        WasActivatedThisTurn = false;
    }
    public override string GetDescription()
    {
        return $"{Description}\n스페이드 카드 : {ActivatedCount}장 째";
    }
    public override S_Skill Clone()
    {
        return new Skill_QuadBlade();
    }
}
