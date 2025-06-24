using UnityEngine;

public class S_ErisPuzzle : S_Trinket
{
    public S_ErisPuzzle() : base
    (
        "ErisPuzzle",
        "에리스의 퍼즐",
        "힘 카드와 공용 카드를 같은 카드로 취급합니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.SameStrCommon,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_ErisPuzzle();
    }
}