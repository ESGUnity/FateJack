using System.Threading.Tasks;
using UnityEngine;

public class Foe_TaboosWatcher : S_Foe
{
    public Foe_TaboosWatcher() : base
    (
        "Foe_TaboosWatcher",
        "금기 파수꾼",
        "시련 시작 시 숫자가 10인 카드를 모두 저주합니다.",
        S_FoeTypeEnum.Clotho,
        S_FoeAbilityConditionEnum.StartTrial,
        S_FoePassiveEnum.None
    ) { }

    public override async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await eA.CurseRandomCards(this, 999, S_CardSuitEnum.None, 10);
    }
    public override void CheckMeetCondition(S_Card card = null)
    {
        IsMeetCondition = false;
    }
    public override string GetDescription()
    {
        return AbilityDescription;
    }
    public override S_Foe Clone()
    {
        return new Foe_TaboosWatcher();
    }
}
