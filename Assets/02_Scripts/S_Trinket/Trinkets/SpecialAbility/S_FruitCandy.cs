using UnityEngine;

public class S_FruitCandy : S_Trinket
{
    public S_FruitCandy() : base
    (
        "FruitCandy",
        "과일 사탕",
        "카드를 3장 낼 때마다 전개를 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Any,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Expansion,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_FruitCandy();
    }
}