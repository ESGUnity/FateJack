using UnityEngine;

public class S_Booze : S_Trinket
{
    public S_Booze() : base
    (
        "Booze",
        "독한 술",
        "힘 카드를 낼 때마다 모든 능력치를 2 얻습니다.",
        2,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat,
        S_BattleStatEnum.AllStat,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_Booze();
    }
}
