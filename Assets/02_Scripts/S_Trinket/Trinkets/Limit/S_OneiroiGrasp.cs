using UnityEngine;

public class S_OneiroiGrasp : S_Trinket
{
    public S_OneiroiGrasp() : base
    (
        "OneiroiGrasp",
        "오네이로이의 손아귀",
        "한 턴에 카드를 6장 이상 냈다면 한계를 3 얻습니다.",
        3,
        0,
        S_TrinketConditionEnum.Overflow_Six,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Limit,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_OneiroiGrasp();
    }
}