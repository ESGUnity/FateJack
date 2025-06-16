using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Foe_OneiroiDarknessEater : S_Foe
{
    public Foe_OneiroiDarknessEater() : base
    (
        "Foe_OneiroiDarknessEater",
        "어둠을 먹는 오네이로이",
        "스탠드 시 스택에서 가장 적은 문양의 카드를 모두 저주합니다.",
        S_FoeTypeEnum.Clotho_Elite,
        S_FoeAbilityConditionEnum.Stand,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        S_EffectChecker.Instance.GetLeastTypeCardsInStack(out S_CardTypeEnum type);

        await eA.CurseRandomCards(this, 999, type, -1, false, true);
    }
    public override void CheckMeetCondition(S_Card card = null)
    {
        IsMeetCondition = true;
    }
    public override string GetDescription()
    {
        return $"{AbilityDescription}";
    }
    public override S_Foe Clone()
    {
        return new Foe_OneiroiDarknessEater();
    }
}
