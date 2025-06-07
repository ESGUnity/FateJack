using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_MorosDoomBringer : S_Foe
{
    public Foe_MorosDoomBringer() : base
    (
        "Foe_MorosDoomBringer",
        "파멸을 부르는 모로스",
        "카드를 3장 히트할 때마다 골드를 4 잃습니다.",
        S_FoeTypeEnum.Atropos_Elite,
        S_FoeAbilityConditionEnum.Reverb,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            await eA.AddOrSubtractGold(this, null, -4);

            ActivatedCount = 0;
            IsMeetCondition = false;
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        int count = S_PlayerCard.Instance.GetPreStackCards().Count % 3;

        if (S_PlayerCard.Instance.GetPreStackCards().Count == 0)
        {
            ActivatedCount = 0;
        }
        else
        {
            if (count == 0)
            {
                ActivatedCount = 3;
            }
            else
            {
                ActivatedCount = count;
            }
        }

        IsMeetCondition = ActivatedCount >= 3;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n히트한 카드 : {ActivatedCount}장 째";
    }
    public override S_Foe Clone()
    {
        return new Foe_MorosDoomBringer();
    }
}
