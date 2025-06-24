using UnityEngine;

public class S_Devourer : S_Foe
{
    public S_Devourer() : base
    (
        "Devourer",
        "탐식자",
        "공용 카드를 낼 때마다 모든 능력치를 2 잃습니다.",
        S_FoeTypeEnum.Lachesis,
        -2,
        0,
        S_TrinketConditionEnum.Reverb_One,
        S_TrinketModifyEnum.Common,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Stat,
        S_BattleStatEnum.AllStat,
        false
    )
    { }

    public override S_Foe Clone()
    {
        return new S_Devourer();
    }
}
