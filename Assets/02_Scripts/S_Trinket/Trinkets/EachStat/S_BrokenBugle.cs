using UnityEngine;

public class S_BrokenBugle : S_Trinket
{
    public S_BrokenBugle() : base
    (
        "BrokenBugle",
        "고장난 나팔",
        "모든 카드는 행운 1 당 1%의 확률로 카드를 낼 때 효과를 1번 발동합니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.LuckPerUnleashTrig,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_BrokenBugle();
    }
}
