using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_CardManager : MonoBehaviour
{
    [Header("카드 생성 관련")]
    int idCount = 0;

    // 싱글턴
    static S_CardManager instance;
    public static S_CardManager Instance { get { return instance; } }

    void Awake()
    {
        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
public enum S_StatEnum
{
    None,
    Str, Mind, Luck,
    Str_Mind, Str_Luck, Mind_Luck,
    AllStat
}
public enum S_CardTypeEnum
{
    None,
    Str,
    Mind,
    Luck,
    Common,
    Foe,
}
public enum S_UnleashEnum
{
    None,
    // 스탯
    Stat, Stat_SubAllHighestStatAndStr3Multi, Stat_StrLuck5SubAndMind2Multi, Stat_Mind10SubAndStrLuck10Add, Stat_Per1State, 
    Stat_Per1MindCard, Stat_Per1LuckCard, Stat_Per1CommonCard,
    // 피해
    Harm, Harm_Per1CursedCard, Harm_Per4CursedCard, Harm_Per1StrCard, Harm_Per3StrCard,
    Harm_Per1MindCard, Harm_Per3MindCard, Harm_Per1LuckCard, Harm_Per3LuckCard, Harm_Per1State,
    Harm_Per4State,
    // 체력
    Health,
    // 상태
    Delusion, Expansion, First, ColdBlood, 
    State_SubAllStates, State_AddRandomStatesPer10Luck, 
    // 무게
    Weight, Weight_AddIfWeightLowerLimit, Weight_MakePerfectAndAddMind, Weight_1Or11,
    // 한계
    Limit, Limit_Per10Mind, Limit1_Weight2,
    // 저주 해제
    Dispel_AllCard, Dispel_Per10Str, DispelAndCurse_AllCard,
    // 저주
    Curse_AllRightCards, Curse_AllCardsIfBurst,

    // 적
    // 피해
    Damaged, Damaged_DiffLimitWeight, Damaged_CriticalBurst,
}
public enum S_PersistEnum
{
    None,
    // 카드를 낼 때 (ActivateByHit)
    ReverbCard_CurseAndAddStat, ReverbCard_Weight, ReverbCard_First, ReverbCard_Expansion, ReverbCard_Stat, ReverbCard_Only2WeightWhenHitCurseCard,
    // 상태를 얻을 때 (AddOrSubState)
    ReverbState_Dispel, ReverbState_CantAddState, ReverbState_MultiState,
    // 카드가 저주받을 때 (CurseCard)
    Cursed_Stat, Cursed_AddRandomState, Cursed_Weight, Cursed_LeftCardsImmunity,
    // 스탯을 얻을 때 (AddOrSubStat)
    Stat_MultiStat, Stat_AddStatPer1Rebound, Stat_CantStat,
    // 피해를 줄 때 (HarmFoe)
    Harm_CantHarm, Harm_MultiDamage, Harm_MultiDamagePer1HitCount, Harm_AddDamagePer1Rebound, Harm_NoBurstPerfect,
    // 무게 계산 시 (AddOrSubWeight, AddOrSubLimit)
    Weight_PerfectAddStat, Weight_PerfectAddColdBlood,
    // 버스트 및 완벽 체크 시 적용할 지속 효과(S_PlayerStat에서 계산)
    CheckBurstAndPerfect_CanPerfectDiff1,
    // 스탠드 시 (ActivateByStand)
    Stand_RightCardReboundPer10Luck, Stand_LeftLuckCardsRebound1, Stand_AllRightCardsRebound2, Stand_FieldCardLowerThan3Rebound2, Stand_FirstCardRebound2, Stand_AllCardsRebound1, Stand_Overload_AllRightCards,
    // 발현할 때마다 (ActivateUnleash)
    Unleash_StatPer1Rebound, Unleash_HarmPer1Rebound,
    // 고정 처리하는 곳(S_PlayerCard -> UpdateCardsByStand)
    Fix_FixCursedCard,
    // 타입 (IsSameType)
    IsSameType_StrCommon, IsSameType_MindLuck,
}

public enum S_PersistModifyEnum
{
    None,
    Any,
    Str, Mind, Luck, Common,
}
public enum S_EngravingEnum
{
    None,
    Overload, Overload_Burst, Overload_NotPerfect, // (ActivateByStand)
    Immunity, // (CurseCard)
    QuickAction, // (S_PlayerCard -> UpdateCardsByStartTrial)
    Rebound, // (ActivateByStand)
    Fix, // (S_PlayerCard -> UpdateCardsByStand)
    Flexible, Leap, // (ActivateByStand)
    Dismantle, // (ActivateByHit)
    Mask,
}
