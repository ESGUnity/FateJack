using UnityEngine;

public class S_BrokenBowl : S_Trinket
{
    public S_BrokenBowl() : base
    (
        "BrokenBowl",
        "깨진 그릇",
        "카드가 저주받을 때마다 힘을 10 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.CurseGet10Str,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_BrokenBowl();
    }
}
