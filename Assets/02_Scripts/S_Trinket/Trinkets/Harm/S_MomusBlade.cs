using UnityEngine;

public class S_MomusBlade : S_Trinket
{
    public S_MomusBlade() : base
    (
        "MomusBlade",
        "모무스의 검",
        "능력치 2개를 곱한만큼 피해를 줍니다. 매 턴마다 능력치가 변경됩니다.",
        1,
        0,
        S_TrinketConditionEnum.Always,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Harm_TwoStat_Random,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_MomusBlade();
    }
}