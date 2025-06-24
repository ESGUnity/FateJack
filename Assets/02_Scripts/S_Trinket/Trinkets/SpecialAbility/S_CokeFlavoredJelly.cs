using UnityEngine;

public class S_CokeFlavoredJelly : S_Trinket
{
    public S_CokeFlavoredJelly() : base
    (
        "CokeFlavoredJelly",
        "콜라맛 젤리",
        "카드를 2장 낼 때마다 우선을 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_Two,
        S_TrinketModifyEnum.Any,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.First,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_CokeFlavoredJelly();
    }
}