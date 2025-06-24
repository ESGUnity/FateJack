using UnityEngine;

public class S_Lotto : S_Trinket
{
    public S_Lotto() : base
    (
        "Lotto",
        "복권",
        "한 턴에 행운 카드만 냈다면 힘이 1.5배 증가합니다.",
        0,
        1.5f,
        S_TrinketConditionEnum.Only,
        S_TrinketModifyEnum.Luck,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat_Multi,
        S_BattleStatEnum.Luck,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Lotto();
    }
}