using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class S_EffectActivator : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_EffectLog;

    [Header("컴포넌트")]
    S_EffectChecker checker;
    S_PlayerCard pCard;
    S_PlayerStat pStat;

    [Header("애님 관련")]
    float EFFECT_SPEED = 1.2f;
    const float HIT_AND_SORT_STACK_TIME = 0.35f;
    const float EFFECT_LIFE_TIME = 0.35f;
    const float EFFECT_LOG_LIFE_TIME = 0.60f; // 로그는 효과보다 조금 더 오래 살아남는다.
    int logStartPosX = 80;
    int logStartPosY = 40;
    int logStartPosYOffset = 310;
    int logMoveAmount = 30;
    Vector3 HARM_READY_POS = new Vector3(0, 0, -1);
    Vector3 ATTACK_FOE_POS = new Vector3(0, 8.5f, -1.2f);
    Vector3 ATTACK_PLAYER_POS = new Vector3(0, -10f, -1.2f);

    // 싱글턴
    static S_EffectActivator instance;
    public static S_EffectActivator Instance { get { return instance; } }

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
    void Start()
    {
        // 컴포넌트 할당
        checker = GetComponent<S_EffectChecker>();
        pCard = S_PlayerCard.Instance;
        pStat = S_PlayerStat.Instance;
    }

    #region 효과 발동
    public async Task ActivateByHit(S_CardBase hitCard)
    {
        #region 저주 영역
        // 망상일 경우 저주하기
        if (pStat.IsDelusion > 0)
        {
            // 망상 손실
            pStat.IsDelusion--;
            S_StatInfoSystem.Instance.UpdateSpecialAbility();

            // 저주 처리
            await CurseCard(hitCard, null);

            // 로그 생성
            GenerateEffectLog("망상 -1");
            await WaitEffectLifeTimeAsync();
        }
        // 지속 효과 : 특정 타입 카드를 낼 때 저주하지만 능력치 얻음
        foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.ReverbCard_CurseAndAddStat)) 
        {
            if (hitCard == card) continue;
            S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.ReverbCard_CurseAndAddStat).First();
            switch (persist.PersistModify)
            {
                case S_PersistModifyEnum.Any: 
                    await CurseCard(hitCard, null);
                    await AddOrSubStat(card, persist.Stat, persist.Value);
                    break;
                case S_PersistModifyEnum.Str:
                    if (checker.IsSameType(S_CardTypeEnum.Str, hitCard.CardType))
                    {
                        await CurseCard(hitCard, null);
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Mind:
                    if (checker.IsSameType(S_CardTypeEnum.Mind, hitCard.CardType))
                    {
                        await CurseCard(hitCard, null);
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Luck:
                    if (checker.IsSameType(S_CardTypeEnum.Luck, hitCard.CardType))
                    {
                        await CurseCard(hitCard, null);
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Common:
                    if (checker.IsSameType(S_CardTypeEnum.Common, hitCard.CardType))
                    {
                        await CurseCard(hitCard, null);
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
            }
        }
        #endregion
        #region 무게 영역
        int weight = hitCard.Weight;
        // 분해 각인 여부
        if (hitCard.OriginEngraving.Contains(S_EngravingEnum.Dismantle) || hitCard.Engraving.Contains(S_EngravingEnum.Dismantle)) weight = 0;
        // 지속 효과 : 저주받은 카드를 낼 때 무게를 2만 취급하는 효과
        if (pCard.GetPersistCardsInField(S_PersistEnum.ReverbCard_Only2WeightWhenHitCurseCard).Count > 0 && hitCard.IsCursed)
        {
            weight = 2;
        }
        // 냉혈
        if (pStat.IsColdBlood > 0) // 냉혈일 경우 무게 0
        {
            weight = 0;

            // 냉혈 초기화
            pStat.IsColdBlood--;
            S_StatInfoSystem.Instance.UpdateSpecialAbility();

            // 바운싱 카드
            if (hitCard != null)
            {
                S_FieldInfoSystem.Instance.BouncingFieldCard(hitCard);
            }
            // 로그 생성
            GenerateEffectLog("냉혈 -1");
            await WaitEffectLifeTimeAsync();
        }
        // 무게 계산
        await AddOrSubWeight(hitCard, weight);
        // 지속 효과 : 특정 타입 카드를 낼 때 무게 조정
        foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.ReverbCard_Weight))
        {
            if (hitCard == card) continue;

            S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.ReverbCard_Weight).First();
            switch (persist.PersistModify)
            {
                case S_PersistModifyEnum.Any:
                    await AddOrSubWeight(card, persist.Value);
                    break;
                case S_PersistModifyEnum.Str:
                    if (checker.IsSameType(S_CardTypeEnum.Str, hitCard.CardType))
                    {
                        await AddOrSubWeight(card, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Mind:
                    if (checker.IsSameType(S_CardTypeEnum.Mind, hitCard.CardType))
                    {
                        await AddOrSubWeight(card, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Luck:
                    if (checker.IsSameType(S_CardTypeEnum.Luck, hitCard.CardType))
                    {
                        await AddOrSubWeight(card, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Common:
                    if (checker.IsSameType(S_CardTypeEnum.Common, hitCard.CardType))
                    {
                        await AddOrSubWeight(card, persist.Value);
                    }
                    break;
            }
        }
        #endregion
        // 지속 효과 : 카드를 낼 때 능력치 조정
        foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.ReverbCard_Stat))
        {
            if (hitCard == card) continue;

            S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.ReverbCard_Stat).First();
            switch (persist.PersistModify)
            {
                case S_PersistModifyEnum.Any:
                    await AddOrSubStat(card, persist.Stat, persist.Value);
                    break;
                case S_PersistModifyEnum.Str:
                    if (checker.IsSameType(S_CardTypeEnum.Str, hitCard.CardType))
                    {
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Mind:
                    if (checker.IsSameType(S_CardTypeEnum.Mind, hitCard.CardType))
                    {
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Luck:
                    if (checker.IsSameType(S_CardTypeEnum.Luck, hitCard.CardType))
                    {
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Common:
                    if (checker.IsSameType(S_CardTypeEnum.Common, hitCard.CardType))
                    {
                        await AddOrSubStat(card, persist.Stat, persist.Value);
                    }
                    break;
            }
        }
        // 지속 효과 : 특정 타입 카드를 낼 때 우선을 얻음
        foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.ReverbCard_First))
        {
            if (hitCard == card) continue;

            S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.ReverbCard_First).First();
            switch (persist.PersistModify)
            {
                case S_PersistModifyEnum.Any:
                    await AddOrSubState(card, S_UnleashEnum.First, persist.Value);
                    break;
                case S_PersistModifyEnum.Str:
                    if (checker.IsSameType(S_CardTypeEnum.Str, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.First, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Mind:
                    if (checker.IsSameType(S_CardTypeEnum.Mind, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.First, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Luck:
                    if (checker.IsSameType(S_CardTypeEnum.Luck, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.First, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Common:
                    if (checker.IsSameType(S_CardTypeEnum.Common, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.First, persist.Value);
                    }
                    break;
            }
        }
        // 지속 효과 : 특정 타입 카드를 낼 때 전개를 얻음
        foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.ReverbCard_Expansion))
        {
            if (hitCard == card) continue;

            S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.ReverbCard_Expansion).First();
            switch (persist.PersistModify)
            {
                case S_PersistModifyEnum.Any:
                    await AddOrSubState(card, S_UnleashEnum.Expansion, persist.Value);
                    break;
                case S_PersistModifyEnum.Str:
                    if (checker.IsSameType(S_CardTypeEnum.Str, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.Expansion, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Mind:
                    if (checker.IsSameType(S_CardTypeEnum.Mind, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.Expansion, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Luck:
                    if (checker.IsSameType(S_CardTypeEnum.Luck, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.Expansion, persist.Value);
                    }
                    break;
                case S_PersistModifyEnum.Common:
                    if (checker.IsSameType(S_CardTypeEnum.Common, hitCard.CardType))
                    {
                        await AddOrSubState(card, S_UnleashEnum.Expansion, persist.Value);
                    }
                    break;
            }
        }
    }
    public async Task ActivateFieldCardsByStand()
    {
        // 발현 효과 전 유연과 도약 먼저 계산
        foreach (S_CardBase c in pCard.GetFieldCards())
        {
            if (c.OriginEngraving.Contains(S_EngravingEnum.Flexible) || c.Engraving.Contains(S_EngravingEnum.Flexible))
            {
                // 사운드
                S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);

                pCard.FlexibleCard(c);
                await S_FieldInfoSystem.Instance.FlexibleCard(c);
            }
            if (c.OriginEngraving.Contains(S_EngravingEnum.Leap) || c.Engraving.Contains(S_EngravingEnum.Leap))
            {
                // 사운드
                S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);

                pCard.LeapCard(c);
                await S_FieldInfoSystem.Instance.LeapCard(c);
            }
        }

        // 내 카드 계산
        foreach (S_CardBase card in pCard.GetFieldCards())
        {
            Debug.Log($"{pCard.GetFieldCards().Count} 필드 카드 개수");
            // 효과 발동
            if (card.IsCursed) continue;
            foreach (S_UnleashStruct unleash in card.Unleash)
            {
                Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(제일 기본 발현 단계)");
                await ActivateUnleash(card, unleash);
            }
            card.ReboundCount++;
            // 카드의 메아리 각인 체크
            if (card.IsCursed) continue;
            if (card.OriginEngraving.Contains(S_EngravingEnum.Rebound) || card.Engraving.Contains(S_EngravingEnum.Rebound))
            {
                foreach (S_UnleashStruct unleash in card.Unleash)
                {
                    Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(메아리 각인)");
                    await ActivateUnleash(card, unleash);
                }
                card.ReboundCount++;
            }
            // 지속 효과 : 행운 10 당 오른쪽 카드 메아리 1번 적용
            if (card.IsCursed) continue;
            if (pCard.GetPersistCardInLeft(card, S_PersistEnum.Stand_RightCardReboundPer10Luck) != null)
            {
                for (int i = 0; i < pStat.CurrentLuck / 10; i++)
                {
                    foreach (S_UnleashStruct unleash in card.Unleash)
                    {
                        Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(행운 10당)");
                        await ActivateUnleash(card, unleash);
                    }
                    card.ReboundCount++;
                }
            }
            // 지속 효과 : 왼쪽에 있는 행운 카드에 메아리 1번 적용
            if (card.IsCursed) continue;
            foreach (S_CardBase c in pCard.GetPersistCardsInRight(card, S_PersistEnum.Stand_LeftLuckCardsRebound1))
            {
                if (checker.IsSameType(card.CardType, S_CardTypeEnum.Luck))
                {
                    foreach (S_UnleashStruct unleash in card.Unleash)
                    {
                        Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(왼쪽 행운에 메아리)");
                        await ActivateUnleash(card, unleash);
                    }
                    card.ReboundCount++;
                }
            }
            // 지속 효과 : 오른쪽에 있는 모든 카드에 메아리 2번 적용
            if (card.IsCursed) continue;
            for (int i = 0; i < pCard.GetPersistCardsInLeft(card, S_PersistEnum.Stand_AllRightCardsRebound2).Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    foreach (S_UnleashStruct unleash in card.Unleash)
                    {
                        Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(오른쪽 메아리 2번)");
                        await ActivateUnleash(card, unleash);
                    }
                    card.ReboundCount++;
                }
            }
            // 지속 효과 : 필드에 카드가 3장 이하라면 모든 카드에 메아리 2번 적용
            if (card.IsCursed) continue;
            if (pCard.GetFieldCards().Count <= 3)
            {
                for (int i = 0; i < pCard.GetPersistCardsInField(S_PersistEnum.Stand_FieldCardLowerThan3Rebound2).Count; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        foreach (S_UnleashStruct unleash in card.Unleash)
                        {
                            Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(3장 이하)");
                            await ActivateUnleash(card, unleash);
                        }
                        card.ReboundCount++;
                    }
                }
            }
            // 지속 효과 : 이번 턴에 처음 낸 카드라면 메아리를 2번 적용
            if (card.IsCursed) continue;
            if (pCard.GetFieldCards().Where(x => x.IsCurrentTurn).ToList().FirstOrDefault() == card)
            {
                for (int i = 0; i < pCard.GetPersistCardsInField(S_PersistEnum.Stand_FirstCardRebound2).Count; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        foreach (S_UnleashStruct unleash in card.Unleash)
                        {
                            Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(2번 적용 ㅌ이번턴처음낸)");
                            await ActivateUnleash(card, unleash);
                        }
                        card.ReboundCount++;
                    }
                }
            }
            // 지속 효과 : 필드에 있는 모든 카드에 메아리를 1번 적용
            if (card.IsCursed) continue;
            for (int i = 0; i < pCard.GetPersistCardsInField(S_PersistEnum.Stand_AllCardsRebound1).Count; i++)
            {
                foreach (S_UnleashStruct unleash in card.Unleash)
                {
                    Debug.Log($"{card.Key}의 발현 카운트 : {card.Unleash.Count}(모든 카드 메아리 1)");
                    await ActivateUnleash(card, unleash);
                }
                card.ReboundCount++;
            }

            // 적 사망 시 루프 종료
            if (S_FoeInfoSystem.Instance.CurrentHealth <= 0) break;
        }

        // 과부하 처리
        foreach (S_CardBase c in pCard.GetFieldCards())
        {
            if (c.IsCursed) continue;
            bool isCursed = false;
            if (c.OriginEngraving.Contains(S_EngravingEnum.Overload) || c.Engraving.Contains(S_EngravingEnum.Overload))
            {
                isCursed = true;
            }
            else if (pStat.IsBurst && (c.OriginEngraving.Contains(S_EngravingEnum.Overload_Burst) || c.Engraving.Contains(S_EngravingEnum.Overload_Burst)))
            {
                isCursed = true;
            }
            else if(!pStat.IsPerfect && (c.OriginEngraving.Contains(S_EngravingEnum.Overload_NotPerfect) || c.Engraving.Contains(S_EngravingEnum.Overload_NotPerfect)))
            {
                isCursed = true;
            }
            else if(pCard.GetPersistCardsInLeft(c, S_PersistEnum.Stand_Overload_AllRightCards).Count > 0)
            {
                isCursed = true;
            }

            if (isCursed)
            {
                // 저주
                await CurseCard(c, null);
            }
        }
    }
    public async Task ActivateFoeCardsByStand()
    {
        // 적 카드 계산
        // 발현 효과 전 유연과 도약 먼저 계산
        foreach (S_CardBase card in S_FoeInfoSystem.Instance.FoeCardList)
        {
            foreach (S_UnleashStruct unleash in card.Unleash)
            {
                await ActivateUnleashByFoe(card, unleash);
            }
        }
    }
    #endregion
    #region 카드 효과 발동 관련
    async Task ActivateUnleash(S_CardBase card, S_UnleashStruct unleash) // 발현 효과 발동
    {
        if (card.IsCursed) return;

        // value 캐싱
        int value;
        switch (unleash.Unleash)
        {
            case S_UnleashEnum.Stat: await AddOrSubStat(card, unleash.Stat, unleash.Value); break;
            case S_UnleashEnum.Stat_SubAllHighestStatAndStr3Multi:
                S_StatEnum highestStat = checker.GetHighestStats(out value);
                await AddOrSubStat(card, highestStat, -value);
                await AddOrSubStat(card, S_StatEnum.Str, pStat.CurrentStr * 2); break;
            case S_UnleashEnum.Stat_StrLuck5SubAndMind2Multi:
                value = 0;
                if (pStat.CurrentStr >= 5)
                {
                    value += 5;
                    await AddOrSubStat(card, S_StatEnum.Str, -5);
                }
                else
                {
                    value += pStat.CurrentStr;
                    await AddOrSubStat(card, S_StatEnum.Str, -pStat.CurrentStr);
                }
                if (pStat.CurrentLuck >= 5)
                {
                    value += 5;
                    await AddOrSubStat(card, S_StatEnum.Luck, -5);
                }
                else
                {
                    value += pStat.CurrentLuck;
                    await AddOrSubStat(card, S_StatEnum.Luck, -pStat.CurrentLuck);
                }
                await AddOrSubStat(card, S_StatEnum.Mind, value * 2); break;
            case S_UnleashEnum.Stat_Mind10SubAndStrLuck10Add:
                value = 0;
                if (pStat.CurrentMind >= 10)
                {
                    value += 10;
                    await AddOrSubStat(card, S_StatEnum.Mind, -10);
                }
                else
                {
                    value += pStat.CurrentMind;
                    await AddOrSubStat(card, S_StatEnum.Mind, -pStat.CurrentMind);
                }
                await AddOrSubStat(card, S_StatEnum.Str, value);
                await AddOrSubStat(card, S_StatEnum.Luck, value); break;
            case S_UnleashEnum.Stat_Per1State: await AddOrSubStat(card, unleash.Stat, unleash.Value * (pStat.IsDelusion + pStat.IsExpansion + pStat.IsFirst + pStat.IsColdBlood)); break;
            case S_UnleashEnum.Stat_Per1MindCard: await AddOrSubStat(card, unleash.Stat, unleash.Value * checker.GetSameTypeCardsInField(S_CardTypeEnum.Mind).Count); break;
            case S_UnleashEnum.Stat_Per1LuckCard: await AddOrSubStat(card, unleash.Stat, unleash.Value * checker.GetSameTypeCardsInField(S_CardTypeEnum.Luck).Count); break;
            case S_UnleashEnum.Stat_Per1CommonCard: await AddOrSubStat(card, unleash.Stat, unleash.Value * checker.GetSameTypeCardsInField(S_CardTypeEnum.Common).Count); break;

            case S_UnleashEnum.Harm: await HarmFoe(card, unleash.Stat, unleash.Value); break;
            case S_UnleashEnum.Harm_Per1CursedCard: await HarmFoe(card, unleash.Stat, unleash.Value * checker.GetAllCursedCards().Count); break;
            case S_UnleashEnum.Harm_Per3CursedCard: await HarmFoe(card, unleash.Stat, unleash.Value * (checker.GetAllCursedCards().Count / 3)); break;
            case S_UnleashEnum.Harm_Per1StrCard: await HarmFoe(card, unleash.Stat, unleash.Value * checker.GetSameTypeCardsInField(S_CardTypeEnum.Str).Count); break;
            case S_UnleashEnum.Harm_Per3StrCard: await HarmFoe(card, unleash.Stat, unleash.Value * (checker.GetSameTypeCardsInField(S_CardTypeEnum.Str).Count / 3)); break;
            case S_UnleashEnum.Harm_Per1MindCard: await HarmFoe(card, unleash.Stat, unleash.Value * checker.GetSameTypeCardsInField(S_CardTypeEnum.Mind).Count); break;
            case S_UnleashEnum.Harm_Per3MindCard: await HarmFoe(card, unleash.Stat, unleash.Value * (checker.GetSameTypeCardsInField(S_CardTypeEnum.Mind).Count / 3)); break;
            case S_UnleashEnum.Harm_Per1LuckCard: await HarmFoe(card, unleash.Stat, unleash.Value * checker.GetSameTypeCardsInField(S_CardTypeEnum.Luck).Count); break;
            case S_UnleashEnum.Harm_Per3LuckCard: await HarmFoe(card, unleash.Stat, unleash.Value * (checker.GetSameTypeCardsInField(S_CardTypeEnum.Luck).Count / 3)); break;
            case S_UnleashEnum.Harm_Per1State: await HarmFoe(card, unleash.Stat, unleash.Value * (pStat.IsDelusion + pStat.IsExpansion + pStat.IsFirst + pStat.IsColdBlood)); break;
            case S_UnleashEnum.Harm_Per2State: await HarmFoe(card, unleash.Stat, unleash.Value * ((pStat.IsDelusion + pStat.IsExpansion + pStat.IsFirst + pStat.IsColdBlood) / 2)); break;

            case S_UnleashEnum.Health: await AddOrSubHealth(card, unleash.Value); break;

            case S_UnleashEnum.Delusion:
            case S_UnleashEnum.Expansion:
            case S_UnleashEnum.First:
            case S_UnleashEnum.ColdBlood: await AddOrSubState(card, unleash.Unleash, unleash.Value); break;
            case S_UnleashEnum.State_SubAllStates:
                if (pStat.IsDelusion > 0) await AddOrSubState(card, S_UnleashEnum.Delusion, -pStat.IsDelusion);
                if (pStat.IsExpansion > 0) await AddOrSubState(card, S_UnleashEnum.Expansion, -pStat.IsExpansion);
                if (pStat.IsFirst > 0) await AddOrSubState(card, S_UnleashEnum.First, -pStat.IsFirst);
                if (pStat.IsColdBlood > 0) await AddOrSubState(card, S_UnleashEnum.ColdBlood, -pStat.IsColdBlood); break;
            case S_UnleashEnum.State_AddRandomStatesPer10Luck:
                value = pStat.CurrentLuck / 10;
                List<S_UnleashEnum> states = new() { S_UnleashEnum.Expansion, S_UnleashEnum.First, S_UnleashEnum.ColdBlood };
                for (int i = 0; i < value; i++)
                {
                    await AddOrSubState(card, states.OrderBy(x => Random.value).First(), 1);
                } break;

            case S_UnleashEnum.Weight: await AddOrSubWeight(card, unleash.Value); break;
            case S_UnleashEnum.Weight_AddIfWeightLowerLimit:
                if (pStat.CurrentLimit > pStat.CurrentWeight)
                {
                    await AddOrSubWeight(card, unleash.Value); break;
                } break;
            case S_UnleashEnum.Weight_MakePerfectAndAddMind:
                int diff = pStat.CurrentLimit - pStat.CurrentWeight;
                await AddOrSubWeight(card, diff);
                await AddOrSubStat(card, S_StatEnum.Mind, Mathf.Abs(diff)); break;
            case S_UnleashEnum.Weight_1Or11:
                int currentWeight = pStat.CurrentWeight;
                int limit = pStat.CurrentLimit;

                int weight1 = currentWeight + 1;
                int weight11 = currentWeight + 11;

                bool canUse1 = weight1 <= limit;
                bool canUse11 = weight11 <= limit;

                int chosenWeight;

                if (canUse1 && canUse11)
                {
                    // 둘 다 가능하면 한계에 더 가까운 쪽 선택
                    int diff1 = Mathf.Abs(limit - weight1);
                    int diff11 = Mathf.Abs(limit - weight11);

                    chosenWeight = (diff1 <= diff11) ? 1 : 11;
                }
                else if (canUse11)
                {
                    chosenWeight = 11;
                }
                else if (canUse1)
                {
                    chosenWeight = 1;
                }
                else
                {
                    // 둘 다 버스트면 그냥 1을 선택 (어차피 터짐)
                    chosenWeight = 1;
                }
                await AddOrSubWeight(card, chosenWeight);
                break;

            case S_UnleashEnum.Limit: await AddOrSubLimit(card, unleash.Value); break;
            case S_UnleashEnum.Limit_Per10Mind: await AddOrSubLimit(card, unleash.Value * (pStat.CurrentMind / 10)); break;
            case S_UnleashEnum.Limit1_Weight2:
                List<S_CardBase> limitWeightCards = pCard.GetFieldCards();
                int centerIndex = limitWeightCards.IndexOf(card);
                int centerLeft = 0;
                for (int i = centerIndex - 1; i >= 0; i--)
                {
                    centerLeft++;
                }
                int centerRight = 0;
                for (int i = centerIndex + 1; i < limitWeightCards.Count; i++)
                {
                    centerRight++;
                }
                await AddOrSubLimit(card, centerLeft);
                await AddOrSubWeight(card, centerRight * 2); break;

            case S_UnleashEnum.Dispel_AllCard:
                foreach (S_CardBase c in checker.GetCursedCardsInField())
                {
                    await DispelCard(c, card);
                } break;
            case S_UnleashEnum.Dispel_Per10Str:
                foreach (S_CardBase c in checker.GetCursedCardsInField().OrderBy(x => Random.value).Take(pStat.CurrentStr / 10))
                {
                    await DispelCard(c, card);
                } break;
            case S_UnleashEnum.DispelAndCurse_AllCard:
                foreach (S_CardBase c in pCard.GetFieldCards())
                {
                    if (c.IsCursed) await DispelCard(c, card);
                    else await CurseCard(c, card);
                } break;

            case S_UnleashEnum.Curse_AllRightCards:
                foreach (S_CardBase c in checker.GetRightCardsInField(card))
                {
                    if (!c.IsCursed) await CurseCard(c, card);
                } break;
            case S_UnleashEnum.Curse_AllCardsIfBurst:
                if (pStat.IsBurst)
                {
                    foreach (S_CardBase c in pCard.GetFieldCards())
                    {
                        if (!c.IsCursed) await CurseCard(c, card);
                    }
                } break;
        }
        
        // 지속 효과 : 발현 효과 발동할 때마다 능력치 획득
        foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.Unleash_StatPer1Rebound))
        {
            S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.Unleash_StatPer1Rebound).First();
            await AddOrSubStat(c, persist.Stat, persist.Value);
        }
        // 지속 효과 : 발현 효과 발동할 때마다 피해 주기
        foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.Unleash_HarmPer1Rebound))
        {
            S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.Unleash_HarmPer1Rebound).First();
            await HarmFoe(c, persist.Stat, persist.Value);
        }
    }
    async Task ActivateUnleashByFoe(S_CardBase card, S_UnleashStruct unleash) // 적 카드 효과 발동
    {
        switch (unleash.Unleash)
        {
            case S_UnleashEnum.Damaged: await HarmedByFoe(card, unleash.Value); break;
            case S_UnleashEnum.Damaged_DiffLimitWeight: await HarmedByFoe(card, -Mathf.Abs(pStat.CurrentLimit - pStat.CurrentWeight)); break;
            case S_UnleashEnum.Damaged_CriticalBurst:
                if (pStat.IsBurst)
                {
                    await HarmedByFoe(card, -999);
                } break;
        }
    }
    public async Task AppliedFirstAsync() // 우선 사용 시 호출
    {
        pStat.IsFirst--;

        // 우선 해제하는 효과
        GenerateEffectLog("우선 -1");
        S_StatInfoSystem.Instance.UpdateSpecialAbility();

        await WaitEffectLifeTimeAsync();
    }
    public async Task AppliedExpansionAsync() // 전개 사용 시 호출
    {
        pStat.IsExpansion--;

        // 전개 해제하는 효과
        GenerateEffectLog("전개 -1");
        S_StatInfoSystem.Instance.UpdateSpecialAbility();

        await WaitEffectLifeTimeAsync();
    }
    #endregion
    #region 기본적인 베이스 효과.(효과의 발동 순서 : 카드 바운스 -> 실제 계산 ->  로그 생성 -> 플레이어 쪽 VFX)
    // 기본 능력치 효과
    public async Task AddOrSubWeight(S_CardBase card, int value)
    {
        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
        // 바운싱 카드
        if (card != null)
        {
            S_FieldInfoSystem.Instance.BouncingFieldCard(card);
        }
        // 무게 계산
        pStat.AddOrSubWeight(value);
        // 로그 및 VFX
        string log = "";
        log = value > 0 ? $"무게 +{value}" : $"무게 {value}";
        GenerateEffectLog(log);
        await WaitEffectLifeTimeAsync();

        // 버스트와 완벽 추가 체크
        //if (pStat.IsBurst || pStat.IsPerfect) return;
        pStat.CheckBurstAndPerfect();
        // 버스트 및 완벽 확인
        if (pStat.IsBurst)
        {
            // 로그 및 VFX
            GenerateEffectLog($"버스트");
            await WaitEffectLifeTimeAsync();
        }
        else if (pStat.IsPerfect)
        {
            // 로그 및 VFX
            GenerateEffectLog($"완벽!");
            await WaitEffectLifeTimeAsync();            
            // 지속 효과 : 완벽 시 능력치 얻기
            foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.Weight_PerfectAddStat))
            {
                S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.Weight_PerfectAddStat).First();
                await AddOrSubStat(c, persist.Stat, persist.Value);
            }
            // 지속 효과 : 완벽 시 냉혈 얻기
            foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.Weight_PerfectAddColdBlood))
            {
                S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.Weight_PerfectAddColdBlood).First();
                await AddOrSubState(c, S_UnleashEnum.ColdBlood, persist.Value);
            }
        }
    }
    public async Task AddOrSubLimit(S_CardBase card, int value)
    {
        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
        // 바운싱 카드
        if (card != null)
        {
            S_FieldInfoSystem.Instance.BouncingFieldCard(card);
        }
        // 한계 계산
        pStat.AddOrSubLimit(value);
        // 로그 및 VFX
        string log = "";
        log = value > 0 ? $"한계 +{value}" : $"한계 {value}";
        GenerateEffectLog(log);
        await WaitEffectLifeTimeAsync();

        // 버스트와 완벽 추가 체크
        //if (pStat.IsBurst || pStat.IsPerfect) return;
        pStat.CheckBurstAndPerfect();
        // 버스트 및 완벽 확인
        if (pStat.IsBurst)
        {
            // 로그 및 VFX
            GenerateEffectLog($"버스트");
            await WaitEffectLifeTimeAsync();
        }
        else if (pStat.IsPerfect)
        {
            // 로그 및 VFX
            GenerateEffectLog($"완벽!");
            await WaitEffectLifeTimeAsync();
            // 지속 효과 : 완벽 시 능력치 얻기
            foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.Weight_PerfectAddStat))
            {
                S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.Weight_PerfectAddStat).First();
                await AddOrSubStat(c, persist.Stat, persist.Value);
            }
            // 지속 효과 : 완벽 시 냉혈 얻기
            foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.Weight_PerfectAddColdBlood))
            {
                S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.Weight_PerfectAddColdBlood).First();
                await AddOrSubState(c, S_UnleashEnum.ColdBlood, persist.Value);
            }
        }
    }
    public async Task AddOrSubHealth(S_CardBase card, int value)
    {
        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
        // 바운싱 카드
        if (card != null)
        {
            S_FieldInfoSystem.Instance.BouncingFieldCard(card);
        }

        // 체력 추가
        pStat.AddOrSubHealth(value);

        // 로그 및 VFX
        string log = "";
        log = value > 0 ? $"체력 +{value}" : $"체력 {value}";
        GenerateEffectLog(log);
        await WaitEffectLifeTimeAsync();
    }
    // 전투 능력 효과
    public async Task AddOrSubStat(S_CardBase card, S_StatEnum stat, int value)
    {
        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
        // 바운싱 카드
        if (card != null)
        {
            S_FieldInfoSystem.Instance.BouncingFieldCard(card);
        }
        // 능력치를 얻을 수 없는 지속 효과
        if (pCard.GetPersistCardsInField(S_PersistEnum.Stat_CantStat).Count > 0)
        {
            value = 0;
            GenerateEffectLog($"능력치 획득 불가");
            await WaitEffectLifeTimeAsync();
            return;
        }
        // 얻는 능력치가 배수로 증가하는 지속 효과
        foreach (S_CardBase multiCard in pCard.GetPersistCardsInField(S_PersistEnum.Stat_MultiStat))
        {
            S_PersistStruct pers = multiCard.Persist.Where(x => x.Persist == S_PersistEnum.Stat_MultiStat).First();
            if (stat == pers.Stat)
            {
                value *= pers.Value;
                //S_FieldInfoSystem.Instance.BouncingFieldCard(multiCard);
            }
            else if (pers.Stat == S_StatEnum.AllStat)
            {
                value *= pers.Value;
                //S_FieldInfoSystem.Instance.BouncingFieldCard(multiCard);
            }
        }
        // 발현할 때마다 능력치 획득량이 증가하는 지속 효과
        S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.Stat_AddStatPer1Rebound).FirstOrDefault();
        if (!persist.Equals(default(S_PersistStruct)))
        {
            value += card.ReboundCount * persist.Value;
        }
        // 능력치 얻기
        string log = "";
        switch (stat)
        {
            case S_StatEnum.Str:
                pStat.AddOrSubStr(value);
                log = value > 0 ? $"힘 +{value}" : $"힘 {value}";
                GenerateEffectLog(log);
                await WaitEffectLifeTimeAsync(); break;
            case S_StatEnum.Mind:
                pStat.AddOrSubMind(value);
                log = value > 0 ? $"정신력 +{value}" : $"정신력 {value}";
                GenerateEffectLog(log);
                await WaitEffectLifeTimeAsync(); break;
            case S_StatEnum.Luck:
                pStat.AddOrSubLuck(value);
                log = value > 0 ? $"행운 +{value}" : $"행운 {value}";
                GenerateEffectLog(log);
                await WaitEffectLifeTimeAsync(); break;
            case S_StatEnum.Str_Mind:
                pStat.AddOrSubStr(value);
                log = value > 0 ? $"힘 +{value}" : $"힘 {value}";
                GenerateEffectLog(log);
                //await WaitEffectLifeTimeAsync();

                pStat.AddOrSubMind(value);
                log = value > 0 ? $"정신력 +{value}" : $"정신력 {value}";
                GenerateEffectLog(log);
                await WaitEffectLifeTimeAsync(); break;
            case S_StatEnum.Str_Luck:
                pStat.AddOrSubStr(value);
                log = value > 0 ? $"힘 +{value}" : $"힘 {value}";
                GenerateEffectLog(log);
                //await WaitEffectLifeTimeAsync();

                pStat.AddOrSubLuck(value);
                log = value > 0 ? $"행운 +{value}" : $"행운 {value}";
                GenerateEffectLog(log);
                await WaitEffectLifeTimeAsync(); break;
            case S_StatEnum.Mind_Luck:
                pStat.AddOrSubMind(value);
                log = value > 0 ? $"정신력 +{value}" : $"정신력 {value}";
                GenerateEffectLog(log);
                //await WaitEffectLifeTimeAsync();

                pStat.AddOrSubLuck(value);
                log = value > 0 ? $"행운 +{value}" : $"행운 {value}";
                GenerateEffectLog(log);
                await WaitEffectLifeTimeAsync(); break;
            case S_StatEnum.AllStat:
                pStat.AddOrSubStr(value);
                log = value > 0 ? $"힘 +{value}" : $"힘 {value}";
                GenerateEffectLog(log);
                //await WaitEffectLifeTimeAsync();

                pStat.AddOrSubMind(value);
                log = value > 0 ? $"정신력 +{value}" : $"정신력 {value}";
                GenerateEffectLog(log);
                //await WaitEffectLifeTimeAsync();

                pStat.AddOrSubLuck(value);
                log = value > 0 ? $"행운 +{value}" : $"행운 {value}";
                GenerateEffectLog(log);
                await WaitEffectLifeTimeAsync(); break;
        }
    }
    public async Task HarmFoe(S_CardBase card, S_StatEnum stat, float multi)
    {
        // 지속 효과 : 피해를 줄 수 없음
        if (pCard.GetPersistCardsInField(S_PersistEnum.Harm_CantHarm).Count > 0)
        {
            // 사운드
            S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
            // 바운싱 카드
            if (card != null)
            {
                S_FieldInfoSystem.Instance.BouncingFieldCard(card);
            }
            GenerateEffectLog($"피해를 줄 수 없음");
            await WaitEffectLifeTimeAsync();
            return;
        }
        // 피해량 정하기
        float value = 0;
        switch (stat)
        {
            case S_StatEnum.Str: value = pStat.CurrentStr; break;
            case S_StatEnum.Mind: value = pStat.CurrentMind; break;
            case S_StatEnum.Luck: value = pStat.CurrentLuck; break;
            case S_StatEnum.Str_Mind: value = pStat.CurrentStr * pStat.CurrentMind; break;
            case S_StatEnum.Str_Luck: value = pStat.CurrentStr + pStat.CurrentLuck; break;
            case S_StatEnum.Mind_Luck: value = pStat.CurrentMind * pStat.CurrentLuck; break;
            case S_StatEnum.AllStat: value = pStat.CurrentStr * pStat.CurrentMind * pStat.CurrentLuck; break;
            default: Debug.Log("S_EffectActivator Send : Error Exist In HarmFoe Method"); break;
        }
        // 지속 효과 : 발현 효과가 발동할 때마다 피해량이 증가하는 카드
        S_PersistStruct persistRebound = card.Persist.Where(x => x.Persist == S_PersistEnum.Harm_AddDamagePer1Rebound).FirstOrDefault();
        if (!persistRebound.Equals(default(S_PersistStruct)))
        {
            multi += persistRebound.Value * card.ReboundCount;
        }
        // 기본 피해량 최종 정립
        value *= multi;
        // 지속 효과 : 모든 피해 배수 증가
        foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.Harm_MultiDamage))
        {
            S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.Harm_MultiDamage).First();
            value *= persist.Value;
        }
        // 지속 효과 : 낼 때 마다 피해량 2배 증가하는 카드
        S_PersistStruct persistHitCount = card.Persist.Where(x => x.Persist == S_PersistEnum.Harm_MultiDamagePer1HitCount).FirstOrDefault();
        if (!persistHitCount.Equals(default(S_PersistStruct)))
        {
            value *= persistHitCount.Value * card.HitCount;
        }
        // 지속 효과 : 버스트 및 완벽의 피해량 증감소 효과 제거
        bool noBP = pCard.GetPersistCardsInField(S_PersistEnum.Harm_NoBurstPerfect).Count > 0;
        // 버스트와 완벽에 따른 2차 피해량 정하기
        if (pStat.IsBurst && !noBP)
        {
            value *= 0.5f;
        }
        else if (pStat.IsPerfect && !noBP)
        {
            value *= 2;
        }

        // 공격할 카드 캐싱
        GameObject cardObj = S_FieldInfoSystem.Instance.GetFieldCardObj(card);
        // 공격 애님 시퀀스 캐싱
        Sequence seq = DOTween.Sequence();
        // 살짝 들리기
        Vector3 direction = ATTACK_FOE_POS - cardObj.transform.localPosition;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction); // 2D UI 기준
        seq.Append(cardObj.transform.DOLocalMove(cardObj.transform.localPosition + HARM_READY_POS, 0.3f).SetEase(Ease.OutQuart))
            .Join(cardObj.transform.DOLocalRotateQuaternion(targetRotation, 0.3f).SetEase(Ease.OutQuart));
        // 공격
        seq.Append(cardObj.transform.DOLocalMove(ATTACK_FOE_POS, 0.15f).SetEase(Ease.OutQuart));
        // 딱 적중하는 시점에 체력 달기
        seq.AppendCallback(() =>
        {
            // 적에게 피해주기
            S_FoeInfoSystem.Instance.DamagedByHarm(Mathf.RoundToInt(value));

            // 로그 생성
            if (pStat.IsBurst && !noBP) GenerateEffectLog($"{value}의 감소된 피해(버스트)");
            else if (pStat.IsPerfect && !noBP) GenerateEffectLog($"{value}의 증가된 피해(완벽)");
            else GenerateEffectLog($"{value}의 피해");

            // 데미지에 따라 카메라 쉐이킹(하스스톤)
            if (value >= S_FoeInfoSystem.Instance.MaxHealth) S_CameraManager.Instance.ShakeCamera(2f);
            else if (value >= S_FoeInfoSystem.Instance.MaxHealth * 0.5f) S_CameraManager.Instance.ShakeCamera(1.5f);
            else if (value >= S_FoeInfoSystem.Instance.MaxHealth * 0.3f) S_CameraManager.Instance.ShakeCamera(1.0f);
            else S_CameraManager.Instance.ShakeCamera(0.5f);

            // 사운드
            S_AudioManager.Instance.PlaySFX(SFXEnum.Harm);
        });
        // 원위치
        seq.Append(cardObj.transform.DOLocalMove(cardObj.GetComponent<S_FieldCardObj>().OriginPRS.Pos, 0.25f).SetEase(Ease.OutQuart))
            .Join(cardObj.transform.DOLocalRotate(cardObj.GetComponent<S_FieldCardObj>().CARD_ROT, 0.25f).SetEase(Ease.OutQuart));

        await seq.AsyncWaitForCompletion();
    }
    public async Task HarmedByFoe(S_CardBase card, int value)
    {
        // 지속 효과 : 버스트 및 완벽의 피해량 증감소 효과 제거
        bool noBP = pCard.GetPersistCardsInField(S_PersistEnum.Harm_NoBurstPerfect).Count > 0;
        // 버스트와 완벽에 따른 2차 피해량 정하기
        if (pStat.IsBurst && !noBP)
        {
            value *= 2;
        }

        // 공격할 카드 캐싱
        GameObject cardObj = S_FoeInfoSystem.Instance.GetFoeCardObj(card);
        // 공격 애님 시퀀스 캐싱
        Sequence seq = DOTween.Sequence();
        // 살짝 들리기
        Vector3 direction = ATTACK_PLAYER_POS - cardObj.transform.localPosition;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction); // 2D UI 기준
        seq.Append(cardObj.transform.DOLocalMove(cardObj.transform.localPosition + HARM_READY_POS, 0.3f).SetEase(Ease.OutQuart))
            .Join(cardObj.transform.DOLocalRotateQuaternion(targetRotation, 0.3f).SetEase(Ease.OutQuart));
        // 공격
        seq.Append(cardObj.transform.DOLocalMove(ATTACK_PLAYER_POS, 0.15f).SetEase(Ease.OutQuart));
        // 딱 적중하는 시점에 체력 달기
        seq.AppendCallback(() =>
        {
            // 체력 잃기
            pStat.AddOrSubHealth(value);

            // 로그 생성
            if (pStat.IsBurst && !noBP) GenerateEffectLog($"체력 {value}(버스트)");
            else GenerateEffectLog($"체력 {value}");

            // 데미지에 따라 카메라 쉐이킹(하스스톤)
            if (value >= S_FoeInfoSystem.Instance.MaxHealth) S_CameraManager.Instance.ShakeCamera(2f);
            else if (value >= S_FoeInfoSystem.Instance.MaxHealth * 0.5f) S_CameraManager.Instance.ShakeCamera(1.5f);
            else S_CameraManager.Instance.ShakeCamera(1.0f);

            // 사운드
            S_AudioManager.Instance.PlaySFX(SFXEnum.Harm);
        });
        // 원위치
        seq.Append(cardObj.transform.DOLocalMove(cardObj.GetComponent<S_FieldCardObj>().OriginPRS.Pos, 0.25f).SetEase(Ease.OutQuart))
            .Join(cardObj.transform.DOLocalRotate(cardObj.GetComponent<S_FieldCardObj>().CARD_ROT, 0.25f).SetEase(Ease.OutQuart));

        await seq.AsyncWaitForCompletion();
    }
    // 상태
    public async Task AddOrSubState(S_CardBase card, S_UnleashEnum state, int value)
    {
        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
        // 바운싱 카드
        if (card != null)
        {
            S_FieldInfoSystem.Instance.BouncingFieldCard(card);
        }
        // 지속 효과 : 상태를 얻을 수 없음
        if (value > 0 && pCard.GetPersistCardsInField(S_PersistEnum.ReverbState_CantAddState).Count > 0)
        {
            GenerateEffectLog("상태를 얻을 수 없음");
            await WaitEffectLifeTimeAsync();
            return;
        }
        // 지속 효과 : 얻는 상태가 2배
        foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.ReverbState_MultiState))
        {
            S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.ReverbState_MultiState).First();
            value *= persist.Value;
        }
        // 상태 획득, 로그 및 VFX
        string log = "";
        switch (state)
        {
            case S_UnleashEnum.Delusion: 
                pStat.IsDelusion += value;
                log = value > 0 ? $"망상 +{value}" : $"망상 {value}";
                GenerateEffectLog(log);
                S_StatInfoSystem.Instance.UpdateSpecialAbility();
                await WaitEffectLifeTimeAsync();
                break;
            case S_UnleashEnum.Expansion:
                pStat.IsExpansion += value;
                log = value > 0 ? $"전개 +{value}" : $"전개 {value}";
                GenerateEffectLog(log);
                S_StatInfoSystem.Instance.UpdateSpecialAbility();
                await WaitEffectLifeTimeAsync();
                break;
            case S_UnleashEnum.First:
                pStat.IsFirst += value;
                log = value > 0 ? $"우선 +{value}" : $"우선 {value}";
                GenerateEffectLog(log);
                S_StatInfoSystem.Instance.UpdateSpecialAbility();
                await WaitEffectLifeTimeAsync();
                break;
            case S_UnleashEnum.ColdBlood:
                pStat.IsColdBlood += value;
                log = value > 0 ? $"냉혈 +{value}" : $"냉혈 {value}";
                GenerateEffectLog(log);
                S_StatInfoSystem.Instance.UpdateSpecialAbility();
                await WaitEffectLifeTimeAsync();
                break;
        }

        if (value > 0)
        {
            // 지속 효과 : 상태 얻을 때 카드 저주 해제
            foreach (S_CardBase c in pCard.GetPersistCardsInField(S_PersistEnum.ReverbState_Dispel))
            {
                S_PersistStruct persist = c.Persist.Where(x => x.Persist == S_PersistEnum.ReverbState_Dispel).First();
                S_CardBase targetCard = checker.GetUncursedCardsInField(persist.Value).FirstOrDefault();
                if (targetCard != null)
                {
                    await DispelCard(targetCard, null);
                }
            }
        }
    }
    // 저주
    public async Task CurseCard(S_CardBase cursedCard, S_CardBase triggerCard)
    {
        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
        // 바운싱 카드
        S_FieldInfoSystem.Instance.BouncingFieldCard(cursedCard);
        if (triggerCard != null)
        {
            S_FieldInfoSystem.Instance.BouncingFieldCard(triggerCard);
        }

        // 면역 관련 검사
        if (cursedCard.OriginEngraving.Contains(S_EngravingEnum.Immunity) || cursedCard.Engraving.Contains(S_EngravingEnum.Immunity)) // 면역 체크
        {
            // 넘어가기
        }
        // 지속 효과 : 왼쪽에 있는 카드에 면역 부과
        else if (pCard.GetPersistCardsInRight(cursedCard, S_PersistEnum.Cursed_LeftCardsImmunity).Count > 0)
        {
            // 넘어가기
        }
        else
        {
            cursedCard.IsCursed = true;
        }
        // 카드 상태 업데이트(카드의 저주 이펙트 켜기)
        pCard.UpdateCardObjsState();

        // 로그 및 VFX
        if (cursedCard.IsCursed) // 저주받았다면
        {
            // 지속 효과 : 저주받을 때마다 스탯 획득
            foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.Cursed_Stat))
            {
                S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.Cursed_Stat).First();
                await AddOrSubStat(card, persist.Stat, persist.Value);
            }
            // 지속 효과 : 저주받을 때마다 무게 조정
            foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.Cursed_Weight))
            {
                S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.Cursed_Weight).First();
                await AddOrSubWeight(card, persist.Value);
            }
            // 지속 효과 : 저주받을 때마다 무작위 이로운 상태 획득
            foreach (S_CardBase card in pCard.GetPersistCardsInField(S_PersistEnum.Cursed_AddRandomState))
            {
                S_PersistStruct persist = card.Persist.Where(x => x.Persist == S_PersistEnum.Cursed_AddRandomState).First();
                List<S_UnleashEnum> states = new() { S_UnleashEnum.Expansion, S_UnleashEnum.First, S_UnleashEnum.ColdBlood };
                await AddOrSubState(card, states.OrderBy(x => Random.value).Take(1).First(), persist.Value);
            }
            GenerateEffectLog("저주받음");
            await WaitEffectLifeTimeAsync();
        }
        else // 면역 카드에 대한 로그 생성
        {
            GenerateEffectLog("저주 면역");
            await WaitEffectLifeTimeAsync();
        }
    }
    public async Task DispelCard(S_CardBase cursedCard, S_CardBase triggerCard)
    {
        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.CardActivated);
        // 바운싱 카드
        S_FieldInfoSystem.Instance.BouncingFieldCard(cursedCard);
        if (triggerCard != null)
        {
            S_FieldInfoSystem.Instance.BouncingFieldCard(triggerCard);
        }

        // 저주 해제
        cursedCard.IsCursed = false;
        // 카드 상태 업데이트(카드의 저주 이펙트 켜기)
        pCard.UpdateCardObjsState();

        GenerateEffectLog("저주 해제");
        await WaitEffectLifeTimeAsync();
    }
    #endregion
    #region VFX 보조
    public async Task WaitEffectLifeTimeAsync()
    {
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    public float GetEffectLifeTime()
    {
        return EFFECT_LIFE_TIME * EFFECT_SPEED;
    }
    public float GetEffectLogLifeTime()
    {
        return EFFECT_LOG_LIFE_TIME * EFFECT_SPEED;
    }
    public float GetHitAndSortCardsTime()
    {
        return HIT_AND_SORT_STACK_TIME * EFFECT_SPEED;
    }
    public void GenerateEffectLog(string text) // 이펙트 로그 생성 메서드
    {
        // 효과 로그 생성
        GameObject go = Instantiate(prefab_EffectLog, transform);
        RectTransform rect = go.GetComponent<RectTransform>();
        S_EffectLog effectLog = go.GetComponent<S_EffectLog>();

        // 초기 위치 설정
        Vector2 startPos = new Vector2(
            Random.Range(-logStartPosX, logStartPosX),
            Random.Range(-logStartPosY, logStartPosY) - logStartPosYOffset
        );
        rect.anchoredPosition = startPos;

        // 텍스트 설정 + 투명하게 시작
        effectLog.SetEffectText(text);
        effectLog.text_EffectContent.DOFade(0f, 0f);

        // 무작위 목표 위치 (현재 위치 기준, 좌우 무작위, 아래쪽으로 낙하)
        Vector2 endPos = startPos + new Vector2(
            Random.Range(-logStartPosX, logStartPosX),
            -Random.Range(logMoveAmount * 0.5f, logMoveAmount * 1.5f)
        );

        // 중간 위치: 포물선 상단 (중간 x, y는 더 위)
        Vector2 midPos = startPos + new Vector2(
            (endPos.x - startPos.x) * 0.5f,
            logMoveAmount
        );

        // 곡선 연출용 시간
        float totalTime = GetEffectLogLifeTime();
        float riseTime = totalTime * 0.45f;
        float fallTime = totalTime * 0.55f;

        Sequence seq = DOTween.Sequence();

        // 1. 위로 뜨기
        seq.Append(rect.DOAnchorPos(midPos, riseTime).SetEase(Ease.OutQuad))
            .Join(effectLog.text_EffectContent.DOFade(1f, riseTime));

        // 2. 포물선 궤적 끝점으로 이동 및 페이드 아웃
        seq.Append(rect.DOAnchorPos(endPos, fallTime).SetEase(Ease.InQuad))
            .Join(effectLog.text_EffectContent.DOFade(0f, fallTime));

        // 3. 끝나면 파괴
        seq.OnComplete(() => Destroy(go));
    }
    #endregion
}