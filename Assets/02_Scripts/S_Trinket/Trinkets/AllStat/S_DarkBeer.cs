using UnityEngine;

public class S_DarkBeer : S_Trinket
{
    public S_DarkBeer() : base
    (
        "DarkBeer",
        "흑맥주",
        "공용 카드를 3장 낼 때마다 모든 능력치를 1 얻고 누적됩니다.",
        1,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Common,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat,
        S_BattleStatEnum.AllStat,
        true,
        true
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_DarkBeer();
    }
}