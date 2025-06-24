using UnityEngine;

public class S_Abracadabra : S_Trinket
{
    public S_Abracadabra() : base
    (
        "Abracadabra",
        "아브라카다브라",
        "한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 각각 2장 이상 냈다면 모든 능력치를 곱한만큼 피해를 줍니다.",
        1,
        0,
        S_TrinketConditionEnum.GrandChaos_Two,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Harm_AllStat,
        S_BattleStatEnum.AllStat,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Abracadabra();
    }
}