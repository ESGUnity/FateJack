using UnityEngine;

public class S_Mitten : S_Trinket
{
    public S_Mitten() : base
    (
        "Mitten",
        "벙어리 장갑",
        "행운을 얻을 수 없습니다. 대신 힘 카드와 정신력 카드는 효과를 1번 더 발동합니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.NoLuckStrMind1Trig,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Mitten();
    }
}