using UnityEngine;

public class S_Soda : S_Trinket
{
    public S_Soda() : base
    (
        "Soda",
        "탄산 음료",
        "힘 카드를 3장 낼 때마다 냉혈을 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.ColdBlood,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Soda();
    }
}