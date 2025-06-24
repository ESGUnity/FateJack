using UnityEngine;

public class S_PlateArmor : S_Trinket
{
    public S_PlateArmor() : base
    (
        "PlateArmor",
        "판금 갑옷",
        "힘을 얻을 수 없습니다. 대신 정신력 카드와 행운 카드는 효과를 1번 더 발동합니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.NoStrMindLuck1Trig,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_PlateArmor();
    }
}