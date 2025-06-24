using UnityEngine;

public class S_ThanatosTheRest : S_Foe
{
    public S_ThanatosTheRest() : base
    (
        "ThanatosTheRest",
        "안식의 타나토스",
        "최대 체력의 30%보다 작은 피해에는 체력을 잃지 않습니다.",
        S_FoeTypeEnum.Atropos_Elite,
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.Immunity30PerHarm,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false
    )
    { }

    public override S_Foe Clone()
    {
        return new S_ThanatosTheRest();
    }
}
