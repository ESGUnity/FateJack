using UnityEngine;

public class S_Hatchet : S_Trinket
{
    public S_Hatchet() : base
    (
        "Hatchet", 
        "손도끼",
        "힘 카드를 낼 때마다 힘만큼 피해를 줍니다.",
        1,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Harm,
        S_BattleStatEnum.Str,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_Hatchet();
    }
}
