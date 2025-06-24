using UnityEngine;

public class S_ZeroSugaSoda : S_Trinket
{
    public S_ZeroSugaSoda() : base
    (
        "ZeroSugaSoda",
        "루이보스 차",
        "공용 카드를 3장 낼 때마다 냉혈을 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Common,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.ColdBlood,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_ZeroSugaSoda();
    }
}