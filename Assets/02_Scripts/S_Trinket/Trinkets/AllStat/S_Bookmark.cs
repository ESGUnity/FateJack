using UnityEngine;

public class S_Bookmark : S_Trinket
{
    public S_Bookmark() : base
    (
        "Bookmark",
        "책갈피",
        "한 턴에 정신력 카드만 냈다면 힘이 1.5배 증가합니다.",
        0,
        1.5f,
        S_TrinketConditionEnum.Only,
        S_TrinketModifyEnum.Mind,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat_Multi,
        S_BattleStatEnum.Mind,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Bookmark();
    }
}