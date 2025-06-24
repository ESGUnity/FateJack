using UnityEngine;

public class S_GamblerDice : S_Trinket
{
    public S_GamblerDice() : base
    (
        "GamblerDice",
        "도박사의 주사위",
        "모든 행운 카드는 행운 1 당 1%의 확률로 효과를 1번 더 발동합니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.LuckPer1LuckTrig,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_GamblerDice();
    }
}
