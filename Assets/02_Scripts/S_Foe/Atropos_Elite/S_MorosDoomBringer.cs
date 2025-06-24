using UnityEngine;

public class S_MorosDoomBringer : S_Foe
{
    public S_MorosDoomBringer() : base
    (
        "MorosDoomBringer",
        "파멸을 부르는 모로스",
        "최대 체력의 25%를 초과한 피해를 무시합니다.",
        S_FoeTypeEnum.Atropos_Elite,
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.Ignore25PerHarm,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false
    )
    { }

    public override S_Foe Clone()
    {
        return new S_MorosDoomBringer();
    }
}
