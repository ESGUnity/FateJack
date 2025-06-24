using UnityEngine;

public class S_ClothoFateWeaver : S_Foe
{
    public S_ClothoFateWeaver() : base
    (
        "ClothoFateWeaver",
        "운명을 짓는 자 클로토",
        "한 턴에 카드를 3장 이하만 냈다면 이번 턴에 낸 카드를 모두 저주합니다.",
        S_FoeTypeEnum.Clotho_Boss,
        0,
        0,
        S_TrinketConditionEnum.Resection_Three,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.DeathAttack,
        S_BattleStatEnum.None,
        true
    )
    { }

    public override S_Foe Clone()
    {
        return new S_ClothoFateWeaver();
    }
}
