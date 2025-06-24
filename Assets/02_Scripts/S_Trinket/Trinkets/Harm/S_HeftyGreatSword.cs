using UnityEngine;

public class S_HeftyGreatSword : S_Trinket
{
    public S_HeftyGreatSword() : base
    (
        "HeftyGreatSword",
        "중후한 대검",
        "스택에 카드 무게 합 20 당 능력치 2개를 곱한만큼 피해를 2번 줍니다. 매 턴마다 능력치가 변경됩니다.",
        2,
        0,
        S_TrinketConditionEnum.Legion_Twenty,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Harm_TwoStat_Random,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_HeftyGreatSword();
    }
}