using UnityEngine;

public class S_DarkHold : S_Trinket
{
    public S_DarkHold() : base
    (
        "DarkHold",
        "다크 홀드",
        "한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 각각 1장 이상 냈다면 한계를 4 얻습니다.",
        4,
        0,
        S_TrinketConditionEnum.GrandChaos_One,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Limit,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_DarkHold();
    }
}