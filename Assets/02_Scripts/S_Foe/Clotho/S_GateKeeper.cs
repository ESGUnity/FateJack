using UnityEngine;

public class S_GateKeeper : S_Foe
{
    public S_GateKeeper() : base
    (
        "GateKeeper",
        "문지기",
        "한 턴에 카드를 3장 이하만 냈다면 망상을 얻습니다.",
        S_FoeTypeEnum.Clotho,
        0,
        0,
        S_TrinketConditionEnum.Resection_Three,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.Delusion,
        S_BattleStatEnum.None,
        true
    ) { }

    public override S_Foe Clone()
    {
        return new S_GateKeeper();
    }
}
