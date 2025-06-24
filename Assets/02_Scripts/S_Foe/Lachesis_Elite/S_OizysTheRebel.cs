using UnityEngine;

public class S_OizysTheRebel : S_Foe
{
    public S_OizysTheRebel() : base
    (
        "OizysTheRebel",
        "격동하는 오이지스",
        "의지가 0이라면 플레이어 공격 시 즉시 처치합니다.",
        S_FoeTypeEnum.Lachesis_Elite,
        0,
        0,
        S_TrinketConditionEnum.None,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.NoDeterminationDeathAttack,
        S_TrinketEffectEnum.None,
        S_BattleStatEnum.None,
        false
    )
    { }

    public override S_Foe Clone()
    {
        return new S_OizysTheRebel();
    }
}
