using UnityEngine;

public class S_Eraser : S_Trinket
{
    public S_Eraser() : base
    (
        "Eraser",
        "지우개",
        "버스트에 의한 데미지 감소와 완벽에 의한 데미지 증가를 없앱니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.NoBurstPerfect,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_Eraser();
    }
}