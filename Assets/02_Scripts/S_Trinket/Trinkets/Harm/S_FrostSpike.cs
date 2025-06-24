using UnityEngine;

public class S_FrostSpike : S_Trinket
{
    public S_FrostSpike() : base
    (
        "FrostSpike",
        "얼음송곳",
        "한 턴에 카드를 3장 이하만 냈다면 능력치 2개를 곱한만큼 피해를 3번 줍니다. 매 턴마다 능력치가 변경됩니다.",
        3,
        0,
        S_TrinketConditionEnum.Resection_Three,
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
        return new S_FrostSpike();
    }
}