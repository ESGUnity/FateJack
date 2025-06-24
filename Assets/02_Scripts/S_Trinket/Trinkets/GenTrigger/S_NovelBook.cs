using UnityEngine;

public class S_NovelBook : S_Trinket
{
    public S_NovelBook() : base
    (
        "NovelBook",
        "소설책",
        "시련 시작 시 무작위 카드를 4장 생성합니다.",
        4,
        0,
        S_TrinketConditionEnum.StartTrial,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Gen,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_NovelBook();
    }
}