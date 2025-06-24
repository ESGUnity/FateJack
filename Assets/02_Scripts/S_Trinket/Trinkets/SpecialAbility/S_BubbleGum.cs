using UnityEngine;

public class S_BubbleGum : S_Trinket
{
    public S_BubbleGum() : base
    (
        "BubbleGum",
        "풍선껌",
        "정신력 카드를 낼 때마다 전개를 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Mind,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Expansion,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_BubbleGum();
    }
}