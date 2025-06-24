using UnityEngine;

public class S_CoffeeFlavoredGum : S_Trinket
{
    public S_CoffeeFlavoredGum() : base
    (
        "CoffeeFlavoredGum",
        "커피맛 껌",
        "힘 카드를 낼 때마다 전개를 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Expansion,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_CoffeeFlavoredGum();
    }
}