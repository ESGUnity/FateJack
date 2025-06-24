using UnityEngine;

public class S_SomeoneDiary : S_Trinket
{
    public S_SomeoneDiary() : base
    (
        "SomeoneDiary",
        "누군가의 일기",
        "시련 시작 시 덱에서 카드 1장을 복사하여 2장 생성합니다.",
        2,
        0,
        S_TrinketConditionEnum.StartTrial,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Gen_Deck,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_SomeoneDiary();
    }
}