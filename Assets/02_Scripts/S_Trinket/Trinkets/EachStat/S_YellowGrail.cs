using UnityEngine;

public class S_YellowGrail : S_Trinket
{
    public S_YellowGrail() : base
    (
        "YellowGrail",
        "노란 성배",
        "완벽을 얻을 때마다 힘과 행운을 8 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.Perfect8StrLuck,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_YellowGrail();
    }
}
