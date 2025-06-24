using UnityEngine;

public class S_GraveKeeper : S_Foe
{
    public S_GraveKeeper() : base
    (
        "GraveKeeper",
        "묘지기",
        "힘 카드를 3장 낼 때마다 망상을 얻습니다.",
        S_FoeTypeEnum.Atropos,
        0,
        0,
        S_TrinketConditionEnum.Reverb_Three,
        S_TrinketModifyEnum.Str,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Delusion,
        S_BattleStatEnum.None,
        true
    )
    { }

    public override S_Foe Clone()
    {
        return new S_GraveKeeper();
    }
}
