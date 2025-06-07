using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Skill_PowerBuild : S_Skill
{
    public Skill_PowerBuild() : base
    (
        "Skill_PowerBuild",
        "근력 훈련",
        "스페이드 카드를 3장 히트할 때마다 힘을 2 얻고 누적됩니다.",
        S_SkillConditionEnum.Reverb,
        S_SkillPassiveEnum.NeedActivatedCount,
        true
    ) { }

    public override async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            await eA.AddBattleStats(this, hitCard, S_BattleStatEnum.Strength, 2);

            ActivatedCount = 0;
            IsMeetCondition = false;
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        int spadeCount = S_EffectChecker.Instance.GetSameSuitCardsInStack(S_CardSuitEnum.Spade).Count;

        if (spadeCount == 0)
        {
            ActivatedCount = 0;
            IsMeetCondition = false;
            return;
        }

        int mod = spadeCount % 3;
        ActivatedCount = mod == 0 ? 3 : mod;
        IsMeetCondition = mod == 0;
        CurrentAccumulateValue = (spadeCount / 3) * 2 + TrialAccumulateValue;
    }
    public override void StartNewTurn(int currentTrial)
    {
        if (currentTrial == 1)
        {
            S_PlayerStat.Instance.AddStrength(CurrentAccumulateValue); // 누적이기에 시련 시작 시 누적량 더해놓기
        }
    }
    public override string GetDescription()
    {
        return $"{Description}\n스페이드 카드 : {ActivatedCount}장 째\n누적 힘 : {CurrentAccumulateValue}";
    }
    public override S_Skill Clone()
    {
        return new Skill_PowerBuild();
    }
}
