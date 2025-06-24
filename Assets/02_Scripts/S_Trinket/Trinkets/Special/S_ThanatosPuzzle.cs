using UnityEngine;

public class S_ThanatosPuzzle : S_Trinket
{
    public S_ThanatosPuzzle() : base
    (
        "ThanatosPuzzle",
        "타나토스의 퍼즐",
        "정신력 카드와 행운 카드를 같은 카드로 취급합니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.SameMindLuck,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_ThanatosPuzzle();
    }
}