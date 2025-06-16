using UnityEngine;

public class S_RedAmulet : S_Trinket
{
    public S_RedAmulet() : base
    (
        "RedAmulet",
        "붉은 부적",
        "힘 카드를 낼 때마다 그 카드를 저주하고 정신력과 행운을 15 얻습니다.",
        15,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.CurseStr,
        S_TrinketEffectEnum.Stat,
        S_BattleStatEnum.Mind_Luck,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_RedAmulet();
    }
}
