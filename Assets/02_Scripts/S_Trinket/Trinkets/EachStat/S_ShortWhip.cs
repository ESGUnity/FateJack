using UnityEngine;

public class S_ShortWhip : S_Trinket
{
    public S_ShortWhip() : base
    (
        "ShortWhip",
        "짧은 채찍",
        "생성된 카드는 효과를 1번 더 발동합니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.Gen1Trig,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_ShortWhip();
    }
}
