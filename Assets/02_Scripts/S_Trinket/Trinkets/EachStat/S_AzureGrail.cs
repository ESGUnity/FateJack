using UnityEngine;

public class S_AzureGrail : S_Trinket
{
    public S_AzureGrail() : base
    (
        "AzureGrail",
        "푸른 성배",
        "완벽을 얻을 때마다 정신력을 15 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.Perfect15Mind,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_AzureGrail();
    }
}
