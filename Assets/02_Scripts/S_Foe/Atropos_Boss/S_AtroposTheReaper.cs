using UnityEngine;

public class S_AtroposTheReaper : S_Foe
{
    public S_AtroposTheReaper() : base
    (
        "AtroposTheReaper",
        "거두는 자 아트로포스",
        "한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 1장 이상 내지 못했다면 플레이어 공격 시 즉시 처치합니다.",
        S_FoeTypeEnum.Atropos_Boss,
        0,
        0,
        S_TrinketConditionEnum.GrandChaos_One_Flip,
        S_TrinketModifyEnum.None,
        S_TrinketPassiveEnum.None,
        S_TrinketEffectEnum.DeathAttack,
        S_BattleStatEnum.None,
        true
    )
    { }

    public override S_Foe Clone()
    {
        return new S_AtroposTheReaper();
    }
}
