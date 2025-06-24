using UnityEngine;

public class S_LachesisTheDecider : S_Foe
{
    public S_LachesisTheDecider() : base
    (
        "LachesisTheDecider",
        "정하는 자 라케시스",
        "공용 카드를 낼 때마다 저주합니다.",
        S_FoeTypeEnum.Lachesis_Boss,
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.CurseCommon,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false
    )
    { }

    public override S_Foe Clone()
    {
        return new S_LachesisTheDecider();
    }
}
