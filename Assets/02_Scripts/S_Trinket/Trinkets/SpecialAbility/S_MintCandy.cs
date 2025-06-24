using UnityEngine;

public class S_MintCandy : S_Trinket
{
    public S_MintCandy() : base
    (
        "MintCandy",
        "민트 사탕",
        "공용 카드를 낼 때마다 전개를 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Common,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Expansion,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_MintCandy();
    }
}