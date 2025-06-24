using UnityEngine;

public class S_SacredFeather : S_Trinket
{
    public S_SacredFeather() : base
    (
        "SacredFeather",
        "신성한 깃털",
        "정신력 카드를 낼 때마다 무게가 1 감소합니다.",
        1,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Mind,
        S_TrinketPassiveEnum.CurseStr,
        S_TrinketEffectEnum.Weight,
        S_BattleStatEnum.None,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_SacredFeather();
    }
}
