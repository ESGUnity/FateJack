using System.Threading.Tasks;
using UnityEngine;

public class Foe_AtroposTheReaper : S_Foe
{
    public Foe_AtroposTheReaper() : base
    (
        "Foe_AtroposTheReaper",
        "거두는 자 아트로포스",
        "카드를 2장 히트할 때마다 망상을 얻습니다.",
        S_FoeTypeEnum.Atropos_Boss,
        S_FoeAbilityConditionEnum.Reverb,
        S_FoePassiveEnum.NeedActivatedCount
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        if (IsMeetCondition)
        {
            await eA.GetDelusion(this, null);

            ActivatedCount = 0;
            IsMeetCondition = false;
        }
    }
    public override void CheckMeetConditionByActivatedCount(S_Card card = null)
    {
        int count = S_PlayerCard.Instance.GetPreStackCards().Count % 2;

        if (S_PlayerCard.Instance.GetPreStackCards().Count == 0)
        {
            ActivatedCount = 0;
        }
        else
        {
            if (count == 0)
            {
                ActivatedCount = 2;
            }
            else
            {
                ActivatedCount = count;
            }
        }

        IsMeetCondition = ActivatedCount >= 2;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}\n히트한 카드 : {ActivatedCount}장 째";
    }
    public override S_Foe Clone()
    {
        return new Foe_AtroposTheReaper();
    }
}
