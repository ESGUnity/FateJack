using UnityEngine;

public class S_ThornGripBornSaw : S_Trinket
{
    public S_ThornGripBornSaw() : base
    (
        "ThornGripBornSaw",
        "가시손잡이 뼈톱",
        "시련 시작 시 체력을 2 잃지만 모든 피해에 20을 곱합니다.",
        -2,
        0,
        S_TrinketConditionEnum.StartTrial,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.Multi20Harm,
        S_TrinketEffectEnum.Health,
        S_BattleStatEnum.None,
        false,
        false
    ) { }

    public override S_Trinket Clone()
    {
        return new S_ThornGripBornSaw();
    }
}