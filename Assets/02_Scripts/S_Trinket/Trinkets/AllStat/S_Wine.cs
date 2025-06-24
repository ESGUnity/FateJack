using UnityEngine;

public class S_Wine : S_Trinket
{
    public S_Wine() : base
    (
        "Wine",
        "와인",
        "힘 카드를 3장 낼 때마다 모든 능력치를 1 얻고 누적됩니다.",
        1,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat,
        S_BattleStatEnum.AllStat,
        true,
        true
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Wine();
    }
}
