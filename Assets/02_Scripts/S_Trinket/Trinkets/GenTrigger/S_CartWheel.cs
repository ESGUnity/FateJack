using UnityEngine;

public class S_CartWheel : S_Trinket
{
    public S_CartWheel() : base
    (
        "CartWheel",
        "수레바퀴",
        "이번 턴에 처음 낸 카드는 효과를 2번 더 발동시킵니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.FirstCard2Trig,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_CartWheel();
    }
}