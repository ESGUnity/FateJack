using UnityEngine;

public class S_Bitters : S_Trinket
{
    public S_Bitters() : base
    (
        "Bitters",
        "약용주",
        "행운 카드를 낼 때마다 모든 능력치를 2 얻습니다.",
        2,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Luck,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat,
        S_BattleStatEnum.AllStat,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Bitters();
    }
}
