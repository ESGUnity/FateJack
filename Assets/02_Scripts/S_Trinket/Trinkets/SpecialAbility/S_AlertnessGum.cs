using UnityEngine;

public class S_AlertnessGum : S_Trinket
{
    public S_AlertnessGum() : base
    (
        "AlertnessGum",
        "졸음방지 껌",
        "정신력 카드를 3장 낼 때마다 냉혈을 얻습니다.",
        0,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Mind,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.ColdBlood,
        S_BattleStatEnum.None,
        true,
        false
    )
    { }

    public override S_Trinket Clone()
    {
        return new S_AlertnessGum();
    }
}