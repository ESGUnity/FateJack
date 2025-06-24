using UnityEngine;

public class S_Spirits : S_Trinket
{
    public S_Spirits() : base
    (
        "Spirits",
        "증류주",
        "정신력 카드를 3장 낼 때마다 모든 능력치를 1 얻고 누적됩니다.",
        1,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Mind,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat,
        S_BattleStatEnum.AllStat,
        true,
        true
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Spirits();
    }
}
