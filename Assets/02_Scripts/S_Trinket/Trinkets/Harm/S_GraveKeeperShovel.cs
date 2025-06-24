using UnityEngine;

public class S_GraveKeeperShovel : S_Trinket
{
    public S_GraveKeeperShovel() : base
    (
        "GraveKeeperShovel",
        "묘지기의 삽",
        "스택에 카드가 정확히 6의 배수만큼 있다면 능력치 2개를 곱한만큼 피해를 3번 줍니다. 매 턴마다 능력치가 변경됩니다.",
        3,
        0,
        S_TrinketConditionEnum.Precision_Six,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Harm_TwoStat_Random,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_GraveKeeperShovel();
    }
}