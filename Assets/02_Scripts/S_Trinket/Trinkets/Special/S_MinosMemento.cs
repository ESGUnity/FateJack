using UnityEngine;

public class S_MinosMemento : S_Trinket
{
    public S_MinosMemento() : base
    (
        "MinosMemento",
        "미노스의 유품",
        "상품 개수를 5개로 늘립니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.AddProductCount,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_MinosMemento();
    }
}