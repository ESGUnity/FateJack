using UnityEngine;

public class S_Incinerator : S_Trinket
{
    public S_Incinerator() : base
    (
        "Incinerator",
        "소각로",
        "스택에 카드 무게 합 20 당 한계를 2 얻습니다.",
        2,
        0,
        S_TrinketConditionEnum.Legion_Twenty,
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
        return new S_Incinerator();
    }
}