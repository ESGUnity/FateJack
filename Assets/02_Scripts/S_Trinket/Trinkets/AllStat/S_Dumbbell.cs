using UnityEngine;

public class S_Dumbbell : S_Trinket
{
    public S_Dumbbell() : base
    (
        "Dumbbell",
        "덤벨",
        "한 턴에 힘 카드만 냈다면 힘이 1.5배 증가합니다.",
        0,
        1.5f,
        S_TrinketConditionEnum.Only,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat_Multi,
        S_BattleStatEnum.Str,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_Dumbbell();
    }
}