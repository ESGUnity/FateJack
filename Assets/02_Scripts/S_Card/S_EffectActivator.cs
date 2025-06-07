using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_EffectActivator : MonoBehaviour
{
    [SerializeField] GameObject sprite_Foe;

    [Header("프리팹")]
    [SerializeField] GameObject prefab_EffectLog;

    [Header("컴포넌트")]
    S_PlayerCard pCard;
    S_PlayerStat pStat;
    S_PlayerSkill pSkill;

    [Header("애님 관련")]
    float EFFECT_SPEED = 1;
    const float HIT_AND_SORT_STACK_TIME = 0.3f;
    const float EFFECT_LIFE_TIME = 0.35f;
    const float EFFECT_LOG_LIFE_TIME = 0.85f; // 로그는 효과보다 조금 더 오래 살아남는다.
    int logStartPosX = 80;
    int logStartPosY = 40;
    int logStartPosYOffset = 250;
    int logMoveAmount = 30;

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
        pCard = S_PlayerCard.Instance;
        pStat = S_PlayerStat.Instance;
        pSkill = S_PlayerSkill.Instance;
    }

    #region 주요 효과 발동
    public async Task ActivateHitCard(S_Card hitCard, S_CardOrderTypeEnum type) // 히트 시 발현, 메아리 발동
    {
        // 카드 숫자에 따른 숫자 합과 능력치 계산
        if (hitCard.AdditiveEffect == S_CardAdditiveEffectEnum.ColdBlood) // 냉혈일 경우 안함.
        {
            // 바운싱 카드
            if (hitCard != null)
            {
                S_StackInfoSystem.Instance.BouncingStackCard(hitCard);
            }

            // 로그 생성
            GenerateEffectLog("냉혈 카드!");
        }
        else // 냉혈이 아니라면 계산
        {
            await AddOrSubtractStackSum(hitCard, hitCard.Number);
            await AddOrSubtractBattleStats(hitCard, hitCard.StatValue, hitCard.Number);
        }

        // 히트 카드가 발현 시
        if (!hitCard.IsCursed && hitCard.BasicCondition == S_CardBasicConditionEnum.Unleash && hitCard.IsMeetCondition)
        {
            // 효과 발동
            await ActivateBasicEffect(hitCard);
            await ActivateDebuff(hitCard);
            await ActivateAdditiveEffect(hitCard);
        }
        else if (hitCard.IsCursed && hitCard.BasicCondition == S_CardBasicConditionEnum.Unleash && hitCard.IsMeetCondition) // 발현인데 추가 조건도 충족했는데 저주받았다면 저주받은 카드!라고 표시해주기
        {
            await CursedCardActivateEffect(hitCard);
        }
        S_PlayerCard.Instance.CheckCardMeetConditionAfterEffect(hitCard);

        // 히트 카드에 의한 메아리 발동
        foreach (S_Card targetCard in pCard.GetPreStackCards())
        {
            if (!targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Reverb && targetCard.IsMeetCondition && targetCard != hitCard)
            {
                // 효과 발동
                await ActivateBasicEffect(targetCard, hitCard);
                await ActivateDebuff(targetCard, hitCard);
                await ActivateAdditiveEffect(targetCard, hitCard);
            }
            else if (targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Reverb && targetCard.IsMeetCondition && targetCard != hitCard)
            {
                await CursedCardActivateEffect(targetCard);
            }

            S_PlayerCard.Instance.CheckCardMeetConditionAfterEffect(targetCard);
        }

        // 카드에 의한 히스토리 저장
        pStat.SaveStatHistory(hitCard, S_StatHistoryTriggerEnum.Card);
    }
    public async Task ActivatedResolveCard() // 스탠드 시 결의 발동
    {
        // 결의 발동
        foreach (S_Card targetCard in pCard.GetPreStackCards())
        {
            if (!targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Resolve && targetCard.IsMeetCondition)
            {
                // 효과 발동
                await ActivateBasicEffect(targetCard);
                await ActivateDebuff(targetCard);
                await ActivateAdditiveEffect(targetCard);
            }
            else if (targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Resolve && targetCard.IsMeetCondition)
            {
                await CursedCardActivateEffect(targetCard);
            }

            S_PlayerCard.Instance.CheckCardMeetConditionAfterEffect(targetCard);
        }
    }
    public async Task ApplyDelusionAsync(S_Card card) // 망상이 있다면 망상을 거는 메서드
    {
        if (pStat.IsDelusion)
        {
            // 망상 초기화
            pStat.IsDelusion = false;

            // 저주 처리
            await CurseCard(card, null);

            // 망상 해제하는 효과
            GenerateEffectLog("망상 해제됨!");
            S_StatInfoSystem.Instance.ChangeSpecialAbility();
            await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
        }
    }
    public async Task AppliedFirstAsync() // 우선 사용 시 호출
    {
        pStat.IsFirst = S_FirstEffectEnum.None;

        // 우선 해제하는 효과
        GenerateEffectLog("우선 사용됨!");
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    public async Task AppliedExpansionAsync() // 전개 사용 시 호출
    {
        pStat.IsExpansion = false;

        // 전개 해제하는 효과
        GenerateEffectLog("전개 사용됨!");
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    async Task ActivateBasicEffect(S_Card card, S_Card reverbedCard = null) // 기본 효과 발동
    {
        if (card.IsCursed) 
        {
            if (IsMeetAdditiveCondition(card))
            {
                await CursedCardActivateEffect(card);
            }
            return;
        }

        switch (card.BasicEffect)
        {
            case S_CardBasicEffectEnum.None: break;
            case S_CardBasicEffectEnum.Growth_Strength: await AddOrSubtractBattleStats(card, S_BattleStatEnum.Strength, 3); break;
            case S_CardBasicEffectEnum.Growth_Mind: await AddOrSubtractBattleStats(card, S_BattleStatEnum.Mind, 3); break;
            case S_CardBasicEffectEnum.Growth_Luck: await AddOrSubtractBattleStats(card, S_BattleStatEnum.Luck, 3); break;
            case S_CardBasicEffectEnum.Growth_AllStat: await AddOrSubtractBattleStats(card, S_BattleStatEnum.AllStat, 2); break;
            case S_CardBasicEffectEnum.Break_MostStat:
                S_BattleStatEnum highestStat = S_EffectChecker.Instance.GetHighestStats(out int val);
                int amount = (int)System.Math.Round(val * 0.5f, System.MidpointRounding.AwayFromZero);
                await AddOrSubtractBattleStats(card, highestStat, amount);
                break;
            case S_CardBasicEffectEnum.Break_RandomStat:
                List<S_BattleStatEnum> randomStats = new List<S_BattleStatEnum>() { S_BattleStatEnum.Strength, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                S_BattleStatEnum randomStat = randomStats[Random.Range(0, randomStats.Count)];
                int amount2 = 0;
                if (randomStat == S_BattleStatEnum.Strength) amount2 = pStat.CurrentStrength;
                else if (randomStat == S_BattleStatEnum.Mind) amount2 = pStat.CurrentMind;
                else if (randomStat == S_BattleStatEnum.Luck) amount2 = pStat.CurrentLuck;
                await AddOrSubtractBattleStats(card, randomStat, amount2);
                break;
            case S_CardBasicEffectEnum.Manipulation: await AddOrSubtractStackSum(card, -2); break;
            case S_CardBasicEffectEnum.Manipulation_CardNumber: await AddOrSubtractStackSum(card, -card.Number); break;
            case S_CardBasicEffectEnum.Manipulation_CleanHit:
                int judgeValue = pStat.CurrentLimit - pStat.StackSum;
                await AddOrSubtractStackSum(card, judgeValue);
                break;
            case S_CardBasicEffectEnum.Resistance: await AddOrSubtractLimit(card, 1); break;
            case S_CardBasicEffectEnum.Resistance_CardNumber: await AddOrSubtractLimit(card, card.Number); break;
            case S_CardBasicEffectEnum.Harm_Strength: await HarmFoe(card, S_BattleStatEnum.Strength, pStat.CurrentStrength); break;
            case S_CardBasicEffectEnum.Harm_Mind: await HarmFoe(card, S_BattleStatEnum.Mind, pStat.CurrentMind); break;
            case S_CardBasicEffectEnum.Harm_Luck: await HarmFoe(card, S_BattleStatEnum.Luck, pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Harm_StrengthAndMind: await HarmFoe(card, S_BattleStatEnum.Strength_Mind, pStat.CurrentStrength * pStat.CurrentMind); break;
            case S_CardBasicEffectEnum.Harm_StrengthAndLuck: await HarmFoe(card, S_BattleStatEnum.Strength_Luck, pStat.CurrentStrength * pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Harm_MindAndLuck: await HarmFoe(card, S_BattleStatEnum.Mind_Luck, pStat.CurrentMind * pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Harm_Carnage: await HarmFoe(card, S_BattleStatEnum.AllStat, pStat.CurrentStrength * pStat.CurrentMind * pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Tempering: await AddOrSubtractDetermination(card, 1); break;
            case S_CardBasicEffectEnum.Plunder: await AddOrSubtractGold(card, 2); break;
            case S_CardBasicEffectEnum.Plunder_Break: await AddOrSubtractGold(card, pStat.CurrentGold); break;
            case S_CardBasicEffectEnum.Creation_Random:
                await CreationCard(card);
                break;
            case S_CardBasicEffectEnum.Creation_SameSuit:
                await CreationCard(card, -1, card.Suit);
                break;
            case S_CardBasicEffectEnum.Creation_SameNumber:
                await CreationCard(card, card.Number);
                break;
            case S_CardBasicEffectEnum.Creation_PlethoraNumber:
                int[] ints = new int[] { 8, 9, 10 };
                int plethoraNum = ints[Random.Range(0, ints.Length)];
                await CreationCard(card, plethoraNum);
                break;
            case S_CardBasicEffectEnum.Expansion: await GetExpansion(card); break;
            case S_CardBasicEffectEnum.First_SameSuit: 
                switch (card.Suit)
                {
                    case S_CardSuitEnum.Spade: await GetFirst(card, S_FirstEffectEnum.Spade); break;
                    case S_CardSuitEnum.Heart: await GetFirst(card, S_FirstEffectEnum.Heart); break;
                    case S_CardSuitEnum.Diamond: await GetFirst(card, S_FirstEffectEnum.Diamond); break;
                    case S_CardSuitEnum.Clover: await GetFirst(card, S_FirstEffectEnum.Clover); break;
                }
                break;
            case S_CardBasicEffectEnum.First_LeastSuit: await GetFirst(card, S_FirstEffectEnum.LeastSuit); break;
            case S_CardBasicEffectEnum.First_SameNumber:
                switch (card.Number)
                {
                    case 1: await GetFirst(card, S_FirstEffectEnum.One); break;
                    case 2: await GetFirst(card, S_FirstEffectEnum.Two); break;
                    case 3: await GetFirst(card, S_FirstEffectEnum.Three); break;
                    case 4: await GetFirst(card, S_FirstEffectEnum.Four); break;
                    case 5: await GetFirst(card, S_FirstEffectEnum.Five); break;
                    case 6: await GetFirst(card, S_FirstEffectEnum.Six); break;
                    case 7: await GetFirst(card, S_FirstEffectEnum.Seven); break;
                    case 8: await GetFirst(card, S_FirstEffectEnum.Eight); break;
                    case 9: await GetFirst(card, S_FirstEffectEnum.Nine); break;
                    case 10: await GetFirst(card, S_FirstEffectEnum.Ten); break;
                }
                break;
            case S_CardBasicEffectEnum.First_CleanHitNumber: await GetFirst(card, S_FirstEffectEnum.CleanHitNumber); break;
            case S_CardBasicEffectEnum.Undertow: await ActivateUndertow(card); break;
            case S_CardBasicEffectEnum.Guidance_LeastSuit:
                S_EffectChecker.Instance.GetLeastSuitCardsInDeck(out S_CardSuitEnum leastSuit);
                List<S_Card> lsCards = S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(999, leastSuit, -1);
                if (lsCards.Count > 0)
                {
                    await GuidanceCard(card, lsCards);
                }
                break;
            case S_CardBasicEffectEnum.Guidance_LeastNumber:
                S_EffectChecker.Instance.GetLeastNumberCardsInDeck(out int leastNum);
                List<S_Card> lnCards = S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(999, S_CardSuitEnum.None, leastNum);
                if (lnCards.Count > 0)
                {
                    await GuidanceCard(card, lnCards);
                }
                break;
            default: break;
        }
    }
    async Task ActivateDebuff(S_Card card, S_Card reverbedCard = null) // 디버프 발동
    {
        if (card.IsCursed)
        {
            if (IsMeetAdditiveCondition(card))
            {
                await CursedCardActivateEffect(card);
            }
            return;
        }

        switch (card.Debuff)
        {
            case S_CardDebuffConditionEnum.None: break;
            case S_CardDebuffConditionEnum.Breakdown: await ExclusionRandomCards(1, S_CardSuitEnum.None, -1, card); break;
            case S_CardDebuffConditionEnum.Delusion: await GetDelusion(card); break;
            case S_CardDebuffConditionEnum.Spell: 
                await CurseRandomCards(1, S_CardSuitEnum.None, -1, true, false, card);
                await CurseRandomCards(1, S_CardSuitEnum.None, -1, false, true, card);
                break;
            case S_CardDebuffConditionEnum.Rebel: await AddOrSubtractLimit(card, -1); break;
            default: break;
        }
    }
    async Task ActivateAdditiveEffect(S_Card card, S_Card reverbedCard = null) // 추가 효과 발동
    {
        List<S_Card> hittedCard = new List<S_Card> { card };

        if (card.IsCursed)
        {
            if (IsMeetAdditiveCondition(card))
            {
                await CursedCardActivateEffect(card);
            }
            return;
        }

        if (reverbedCard != null) hittedCard.Add(reverbedCard);

        switch (card.AdditiveEffect)
        {
            case S_CardAdditiveEffectEnum.None: break;
            case S_CardAdditiveEffectEnum.Reflux_Subtle: await ActivateBasicEffect(card); break;
            case S_CardAdditiveEffectEnum.Reflux_Violent: 
                for (int i = 0; i < 2; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Shatter:
                for (int i = 0; i < 3; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Stack:
                int stackCount = pCard.GetPreStackCards().Count / 4;
                for (int i = 0; i < stackCount; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_PlethoraNumber:
                int plethoraCount = pCard.GetPreStackCards().Count(x => x.Number >= 8) / 3;
                for (int i = 0; i < plethoraCount; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Deck:
                int deckCount = pCard.GetPreDeckCards().Count / 6;
                for (int i = 0; i < deckCount; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Chaos:
                int chaosCount = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1);
                for (int i = 0; i < chaosCount; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Offensive:
                int numMaxLen = S_EffectChecker.Instance.GetContinueNumMaxLengthInStack() / 2;
                for (int i = 0; i < numMaxLen; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Curse:
                int cursedDeck = pCard.GetPreDeckCards().Count(x => x.IsCursed);
                int cursedStack = pCard.GetPreStackCards().Count(x => x.IsCursed);
                int cursed = (cursedDeck + cursedStack) / 3;
                for (int i = 0; i < cursed; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Exclusion:
                int exCards = pCard.GetPreExclusionTotalCards().Count / 3;
                for (int i = 0; i < exCards; i++)
                {
                    await ActivateBasicEffect(card);
                }
                break;
            case S_CardAdditiveEffectEnum.Reflux_Overdrive:
                if (pStat.GetCurrentHealth() == 1) // 체력이 1일 때 발동
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await ActivateBasicEffect(card);
                    }
                }
                break;
            default: break;
        }
    }
    #endregion
    #region 추가 조건 계산
    public bool IsMeetAdditiveCondition(S_Card targetCard, S_Card hitCard = null) // 추가 조건 계산
    {
        bool meetCondition = false;

        if (targetCard.IsCursed) return false;

        if (targetCard.BasicCondition == S_CardBasicConditionEnum.Reverb && hitCard != null)
        {
            switch (targetCard.AdditiveCondition)
            {
                case S_CardAdditiveConditionEnum.Reverb_SameSuit:
                    meetCondition = S_EffectChecker.Instance.IsSameSuit(targetCard.Suit, hitCard.Suit);
                    break;
                case S_CardAdditiveConditionEnum.Reverb_SameNumber:
                    meetCondition = hitCard.Number == targetCard.Number;
                    break;
                case S_CardAdditiveConditionEnum.Reverb_PlethoraNumber:
                    meetCondition = hitCard.Number >= 8;
                    break;
                case S_CardAdditiveConditionEnum.Reverb_CursedCard:
                    meetCondition = hitCard.IsCursed;
                    break;
            }
        }

        switch (targetCard.AdditiveCondition)
        {
            case S_CardAdditiveConditionEnum.None: meetCondition = true; break;
            case S_CardAdditiveConditionEnum.Legion_SameSuit:
                meetCondition = S_EffectChecker.Instance.GetSameSuitSumInStack(targetCard.Suit) >= 40;
                break;
            case S_CardAdditiveConditionEnum.GreatLegion_SameSuit:
                meetCondition = S_EffectChecker.Instance.GetSameSuitSumInStack(targetCard.Suit) >= 60;
                break;
            case S_CardAdditiveConditionEnum.Finale:
                meetCondition = S_EffectChecker.Instance.GetDeckSuitCount() <= 3;
                break;
            case S_CardAdditiveConditionEnum.Finale_Climax:
                meetCondition = S_EffectChecker.Instance.GetDeckSuitCount() <= 2;
                break;
            case S_CardAdditiveConditionEnum.Chaos:
                meetCondition = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1) >= 4;
                break;
            case S_CardAdditiveConditionEnum.Chaos_Anarchy:
                meetCondition = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(2) >= 4;
                break;
            case S_CardAdditiveConditionEnum.GrandChaos_Anarchy:
                meetCondition = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(3) >= 4;
                break;
            case S_CardAdditiveConditionEnum.Chaos_Overflow:
                meetCondition = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStackInCurrentTurn(1) >= 4;
                break;
            case S_CardAdditiveConditionEnum.Offensive:
                meetCondition = S_EffectChecker.Instance.GetContinueNumMaxLengthInStack() >= 4;
                break;
            case S_CardAdditiveConditionEnum.Offensive_SameSuit:
                meetCondition = S_EffectChecker.Instance.GetContinueNumSameSuitMaxLengthInStack() >= 4;
                break;
            case S_CardAdditiveConditionEnum.AllOutOffensive:
                meetCondition = S_EffectChecker.Instance.GetContinueNumMaxLengthInStack() >= 8;
                break;
            case S_CardAdditiveConditionEnum.AllOutOffensive_SameSuit:
                meetCondition = S_EffectChecker.Instance.GetContinueNumSameSuitMaxLengthInStack() >= 8;
                break;
            case S_CardAdditiveConditionEnum.Offensive_Overflow:
                meetCondition = S_EffectChecker.Instance.GetContinueNumMaxLengthInStackInCurrentTurn() >= 4;
                break;
            case S_CardAdditiveConditionEnum.Precision_SameSuit:
                meetCondition = S_EffectChecker.Instance.GetSameSuitCardsInStack(targetCard.Suit).Count % 3 == 0;
                break;
            case S_CardAdditiveConditionEnum.HyperPrecision_SameSuit:
                meetCondition = S_EffectChecker.Instance.GetSameSuitCardsInStack(targetCard.Suit).Count % 6 == 0;
                break;
            case S_CardAdditiveConditionEnum.Precision_SameNumber:
                meetCondition = S_EffectChecker.Instance.GetSameNumberCardsInStack(targetCard.Number).Count % 3 == 0;
                break;
            case S_CardAdditiveConditionEnum.HyperPrecision_SameNumber:
                meetCondition = S_EffectChecker.Instance.GetSameNumberCardsInStack(targetCard.Number).Count % 6 == 0;
                break;
            case S_CardAdditiveConditionEnum.Precision_PlethoraNumber:
                meetCondition = S_EffectChecker.Instance.GetPlethoraNumberCardsInStack().Count % 3 == 0;
                break;
            case S_CardAdditiveConditionEnum.HyperPrecision_PlethoraNumber:
                meetCondition = S_EffectChecker.Instance.GetPlethoraNumberCardsInStack().Count % 6 == 0;
                break;
            case S_CardAdditiveConditionEnum.Overflow:
                meetCondition = S_EffectChecker.Instance.GetCardsInStackInCurrentTurn().Count >= 4;
                break;
            case S_CardAdditiveConditionEnum.Unity: 
                meetCondition = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1) <= 2; 
                break;
            case S_CardAdditiveConditionEnum.Unity_Drastic: 
                meetCondition = S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1) == 1; 
                break;
        }

        return meetCondition;
    }
    #endregion
    #region 카드 기본 효과(한계, 숫자합, 힘, 정신력, 행운, 의지, 골드) (효과의 발동 순서 : 실제 계산 -> 카드 바운스 -> 로그 생성 -> 플레이어 쪽 VFX)
    async Task AddOrSubtractStackSum(S_Card triggerCards, int value)
    {
        // value를 숫자 합에 더하기
        pStat.AddOrSubtractStackSum(value);

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"숫자 합 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_StackSum, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"숫자 합 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_StackSum, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }

        // 버스트와 클린히트 추가 체크
        pStat.CheckBurstAndCleanHit();

        // 버스트와 클린히트에 따른 효과 켜기
        S_StatInfoSystem.Instance.ChangeSpecialAbility();

        // 버스트 및 클린히트 확인
        if (pStat.IsBurst)
        {
            // 로그 생성
            GenerateEffectLog($"버스트!");

            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Burst, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
        else if (pStat.IsCleanHit)
        {
            // 로그 생성
            GenerateEffectLog($"클린히트!");

            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.CleanHit, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
    }
    async Task AddOrSubtractLimit(S_Card triggerCards, int value)
    {
        // value를 한계에 더하기
        pStat.AddOrSubtractLimit(value);

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"한계 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Limit, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"한계 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Limit, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }

        // 버스트와 클린히트 추가 체크
        pStat.CheckBurstAndCleanHit();

        // 버스트와 클린히트에 따른 효과 켜기
        S_StatInfoSystem.Instance.ChangeSpecialAbility();

        // 버스트 및 클린히트 확인
        if (pStat.IsBurst)
        {
            // 로그 생성
            GenerateEffectLog($"버스트!");

            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Burst, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
        else if (pStat.IsCleanHit)
        {
            // 로그 생성
            GenerateEffectLog($"클린히트!");

            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.CleanHit, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
    }
    async Task AddOrSubtractBattleStats(S_Card triggerCards, S_BattleStatEnum stat, int value)
    {
        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 능력치 증가
        if (value > 0)
        {
            switch (stat)
            {
                case S_BattleStatEnum.Strength:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.Mind:
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.Luck:
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.AllStat:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetCardObject(triggerCards));

                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetCardObject(triggerCards));

                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.Random:
                    List<S_BattleStatEnum> stats = new() { S_BattleStatEnum.Strength, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                    S_BattleStatEnum s = stats[Random.Range(0, stats.Count)];
                    if (s == S_BattleStatEnum.Strength)
                    {
                        pStat.AddStrength(value);
                        GenerateEffectLog($"힘 +{value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    }
                    else if (s == S_BattleStatEnum.Mind)
                    {
                        pStat.AddMind(value);
                        GenerateEffectLog($"정신력 +{value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    }
                    else if (s == S_BattleStatEnum.Luck)
                    {
                        pStat.AddLuck(value);
                        GenerateEffectLog($"행운 +{value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    }
                    break;
            }
        }
        else
        {
            switch (stat)
            {
                case S_BattleStatEnum.Strength:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.Mind:
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.Luck:
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.AllStat:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength, S_StackInfoSystem.Instance.GetCardObject(triggerCards));

                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind, S_StackInfoSystem.Instance.GetCardObject(triggerCards));

                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    break;
                case S_BattleStatEnum.Random:
                    List<S_BattleStatEnum> stats = new() { S_BattleStatEnum.Strength, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                    S_BattleStatEnum s = stats[Random.Range(0, stats.Count)];
                    if (s == S_BattleStatEnum.Strength)
                    {
                        pStat.AddStrength(value);
                        GenerateEffectLog($"힘 {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    }
                    else if (s == S_BattleStatEnum.Mind)
                    {
                        pStat.AddMind(value);
                        GenerateEffectLog($"정신력 {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    }
                    else if (s == S_BattleStatEnum.Luck)
                    {
                        pStat.AddLuck(value);
                        GenerateEffectLog($"행운 {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
                    }
                    break;
            }
        }
    }
    async Task HarmFoe(S_Card triggerCards, S_BattleStatEnum stat, int value)
    {
        // 피해주기
        if (pStat.IsBurst)
        {
            value = (value + 1) / 2;
        }
        else if (pStat.IsCleanHit)
        {
            value = (int)System.Math.Round(value * 1.5f, System.MidpointRounding.AwayFromZero);
        }
        S_FoeInfoSystem.Instance.DamagedByHarm(value);

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 로그 생성
        if (pStat.IsBurst)
        {
            GenerateEffectLog($"{value}의 감소된 피해(버스트)");
        }
        else if (pStat.IsCleanHit)
        {
            GenerateEffectLog($"{value}의 증가된 피해(클린히트)");
        }
        else
        {
            GenerateEffectLog($"{value}의 피해");
        }

        // 데미지에 따라 카메라 쉐이킹(하스스톤)
        if (value >= S_FoeInfoSystem.Instance.CurrentFoe.MaxHealth)
        {
            ShakeCamera(1.2f);
        }
        else if (value >= S_FoeInfoSystem.Instance.CurrentFoe.MaxHealth * 0.5f)
        {
            ShakeCamera(0.9f);
        }
        else if (value >= S_FoeInfoSystem.Instance.CurrentFoe.MaxHealth * 0.3f)
        {
            ShakeCamera(0.6f);
        }
        else
        {
            ShakeCamera(0.3f);
        }

        // 피해 VFX
        switch(stat)
        {
            case S_BattleStatEnum.Strength: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength, sprite_Foe); break;
            case S_BattleStatEnum.Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind, sprite_Foe); break;
            case S_BattleStatEnum.Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Luck, sprite_Foe); break;
            case S_BattleStatEnum.Strength_Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Mind, sprite_Foe); break;
            case S_BattleStatEnum.Strength_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Luck, sprite_Foe); break;
            case S_BattleStatEnum.Mind_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind_Luck, sprite_Foe); break;
            case S_BattleStatEnum.AllStat: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Carnage, sprite_Foe); break;
        }
    }
    async Task AddOrSubtractHealth(S_Card triggerCards, int value)
    {
        // 체력 추가
        pStat.AddOrSubtractHealth(value);

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"체력 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Health, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"체력 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Health, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
    }
    async Task AddOrSubtractDetermination(S_Card triggerCards, int value)
    {
        // 의지 추가
        pStat.AddOrSubtractDetermination(value);

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"의지 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Determination, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"의지 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Determination, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
    }
    async Task AddOrSubtractGold(S_Card triggerCards, int value)
    {
        // 골드 추가
        pStat.AddOrSubtractGold(value);

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"골드 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Gold, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"골드 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Gold, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
    }
    async Task CreationCard(S_Card triggerCards, int number = -1, S_CardSuitEnum suit = S_CardSuitEnum.Random)
    {
        // 카드 창조
        S_Card creationCard = S_CardManager.Instance.GenerateRandomCard(number, suit);

        // 냉혈로 바꾸기
        creationCard.IsIllusion = true;
        creationCard.AdditiveEffect = S_CardAdditiveEffectEnum.ColdBlood;

        // 카드 내기
        _ = S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(creationCard, S_CardOrderTypeEnum.IllusionHit);

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 로그 생성
        GenerateEffectLog($"카드 생성함!");

        // 플레이어 이미지 VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Creation, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
    }
    async Task GetExpansion(S_Card triggerCards)
    {
        // 전개 획득
        pStat.IsExpansion = true;

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 로그 생성
        GenerateEffectLog($"히트 시 추가 보기!");

        // 효과 및 대기
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Expansion, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
    }
    async Task GetFirst(S_Card triggerCards, S_FirstEffectEnum effect)
    {
        // 우선 획득
        pStat.IsFirst = effect;

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 로그 생성
        switch (effect)
        {
            case S_FirstEffectEnum.Spade: GenerateEffectLog($"스페이드 우선!"); break;
            case S_FirstEffectEnum.Heart: GenerateEffectLog($"하트 우선!"); break;
            case S_FirstEffectEnum.Diamond: GenerateEffectLog($"디아몬드 우선!"); break;
            case S_FirstEffectEnum.Clover: GenerateEffectLog($"클로버 우선!"); break;
            case S_FirstEffectEnum.LeastSuit: GenerateEffectLog($"가장 적은 문양 우선!"); break;
            case S_FirstEffectEnum.One: GenerateEffectLog($"숫자 1 우선!"); break;
            case S_FirstEffectEnum.Two: GenerateEffectLog($"숫자 2 우선!"); break;
            case S_FirstEffectEnum.Three: GenerateEffectLog($"숫자 3 우선!"); break;
            case S_FirstEffectEnum.Four: GenerateEffectLog($"숫자 4 우선!"); break;
            case S_FirstEffectEnum.Five: GenerateEffectLog($"숫자 5 우선!"); break;
            case S_FirstEffectEnum.Six: GenerateEffectLog($"숫자 6 우선!"); break;
            case S_FirstEffectEnum.Seven: GenerateEffectLog($"숫자 7 우선!"); break;
            case S_FirstEffectEnum.Eight: GenerateEffectLog($"숫자 8 우선!"); break;
            case S_FirstEffectEnum.Nine: GenerateEffectLog($"숫자 9 우선!"); break;
            case S_FirstEffectEnum.Ten: GenerateEffectLog($"숫자 10 우선!"); break;
            case S_FirstEffectEnum.CleanHitNumber: GenerateEffectLog($"클린히트되는 숫자 우선!"); break;
        }

        // 효과 및 대기
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.First, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
    }
    async Task ActivateUndertow(S_Card triggerCards)
    {
        List<S_Card> stacks = pCard.GetPreStackCards();
        List<S_Card> picked = new();

        if (stacks.Count > 2) // 2장 이상이라면 무작위 2장 뽑기
        {
            picked = stacks
                .OrderBy(x => UnityEngine.Random.value)
                .Take(2)
                .ToList();
        }
        else // 2장 이하라면 그냥 모든 스택 카드 발동
        {
            picked = stacks;
        }

        if (picked.Count > 0)
        {
            foreach (S_Card card in picked)
            {
                // 바운싱 카드
                if (triggerCards != null)
                {
                    S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
                }

                // 로그 생성
                GenerateEffectLog($"무작위 카드의 효과 발동!");

                // 플레이어 이미지 VFX
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Undertow, S_StackInfoSystem.Instance.GetCardObject(triggerCards));

                // 효과 발동
                await ActivateBasicEffect(card);
                await ActivateDebuff(card);
                await ActivateAdditiveEffect(card);
            }
        }
    }
    async Task GuidanceCard(S_Card triggerCards, List<S_Card> guidanceCards)
    {
        foreach (S_Card card in guidanceCards)
        {
            // 카드 내기
            _ = S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(card, S_CardOrderTypeEnum.BasicHit);

            // 바운싱 카드
            if (triggerCards != null)
            {
                S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
            }

            // 로그 생성
            GenerateEffectLog($"덱에서 히트함!");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Guidance, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
        }
    }
    #endregion
    #region 디버프 효과
    async Task CurseCard(S_Card cursedCard, S_Card triggerCards = null)
    {
        // 저주내리기(면역이라면 안함)
        if (cursedCard.AdditiveEffect != S_CardAdditiveEffectEnum.Immunity)
        {
            cursedCard.IsCursed = true;
        }
        else
        {
            cursedCard.IsCursed = false;
        }

        if (cursedCard.IsInDeck) // 덱에 있는 카드를 저주한 경우 UICard로 보여주기
        {
            // 카드의 저주 이펙트 켜기
            S_DeckInfoSystem.Instance.UpdateDeckCardsState();

            // UICard로 덱에 있는 저주받은 카드 보여주기
            S_UICardEffecter.Instance.CurseDeckCardVFX(cursedCard);

            // 바운싱 카드
            if (triggerCards != null)
            {
                S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
            }
        }
        else // 스택에 있는 카드를 저주한 경우 
        {
            // 카드의 저주 이펙트 켜기
            S_StackInfoSystem.Instance.UpdateStackCardsState();

            // 저주한 카드 및 트리거 카드 바운싱 VFX
            if (triggerCards != null)
            {
                S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
            }

            S_StackInfoSystem.Instance.BouncingStackCard(cursedCard);
        }

        // 면역 카드를 저주하려했으면 별도의 로그와 이미지 VFX 
        if (cursedCard.IsCursed)
        {
            // 로그 생성
            GenerateEffectLog("저주받음!");
            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Cursed);
        }
        else
        {
            // 로그 생성
            GenerateEffectLog("저주 저항함!!(면역카드)");
            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.ResistanceCurse);
        }
    }
    async Task CurseRandomCards(int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, bool inDeck = true, bool inStack = false, S_Card triggerCards = null) // 조건이 있으면 조건대로 랜덤 저주, 아니면 그냥 랜덤 저주
    {
        List<S_Card> cursedCards = new();

        if (inDeck)
        {
            cursedCards.AddRange(S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(count, suit, num));
        }
        if (inStack)
        {
            cursedCards = S_EffectChecker.Instance.GetRandomCardsInImmediateStack(count, suit, num);
        }

        if (cursedCards.Count > 0)
        {
            foreach (S_Card cursedCard in cursedCards)
            {
                await CurseCard(cursedCard, triggerCards);
            }
        }
    }
    async Task ExclusionRandomCards(int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, S_Card triggerCards = null) // 전체 랜덤 제외
    {
        List<S_Card> exclusionCards = S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(count, suit, num);

        if (exclusionCards.Count > 0)
        {
            foreach (S_Card exclusionCard in exclusionCards)
            {
                // 카드 제외하기
                _ = S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(exclusionCard, S_CardOrderTypeEnum.Exclusion);

                // 바운싱 카드
                if (triggerCards != null)
                {
                    S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
                }

                // 로그 생성
                GenerateEffectLog($"카드 제외함!");

                // 플레이어 이미지 VFX
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Exclusion, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
            }
        }
    }
    async Task GetDelusion(S_Card triggerCards) // 망상
    {
        // 전개 획득
        pStat.IsDelusion = true;

        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 로그 생성
        GenerateEffectLog($"망상에 걸림!");

        // 효과 및 대기
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Delusion, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
    }
    async Task CursedCardActivateEffect(S_Card triggerCards) // 저주받은 카드가 효과를 발동하려할 때
    {
        // 바운싱 카드
        if (triggerCards != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
        }

        // 로그 생성
        GenerateEffectLog("저주받은 카드!");

        // 플레이어 이미지 VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Cursed, S_StackInfoSystem.Instance.GetCardObject(triggerCards));
    }
    #endregion
    #region 능력 효과
    public async Task AddBattleStats(S_Skill skill, S_Card triggerCards, S_BattleStatEnum stat, int value)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await AddOrSubtractBattleStats(triggerCards, stat, value);
    }
    public async Task HarmFoe(S_Skill skill, S_Card triggerCards, S_BattleStatEnum stat, int value)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await HarmFoe(triggerCards, stat, value);
    }
    public async Task CreationCard(S_Skill skill, S_Card triggerCards, int number = -1, S_CardSuitEnum suit = S_CardSuitEnum.Random)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await CreationCard(triggerCards, number, suit);
    }
    public async Task GetExpansion(S_Skill skill, S_Card triggerCards)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await GetExpansion(triggerCards);
    }
    public async Task GetFirst(S_Skill skill, S_Card triggerCards, S_FirstEffectEnum effect)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await GetFirst(triggerCards, effect);
    }
    public async Task ActivateUndertow(S_Skill skill, S_Card triggerCards)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await ActivateUndertow(triggerCards);
    }
    #endregion
    #region 적 효과
    public async Task CurseRandomCards(S_Foe foe, int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, bool inDeck = true, bool inStack = false, S_Card triggerCards = null) // 저주
    {
        S_FoeInfoSystem.Instance.FoeSpriteBouncingVFX();

        await CurseRandomCards(count, suit, num, inDeck, inStack, triggerCards);
    }
    public async Task ExclusionRandomCard(S_Foe foe, int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, S_Card triggerCards = null) // 제외
    {
        S_FoeInfoSystem.Instance.FoeSpriteBouncingVFX();

        await ExclusionRandomCards(count, suit, num, triggerCards);
    }
    public async Task GetDelusion(S_Foe foe, S_Card triggerCards) // 망상
    {
        S_FoeInfoSystem.Instance.FoeSpriteBouncingVFX();

        await GetDelusion(triggerCards);
    }
    public async Task AddOrSubtractHealth(S_Foe foe, S_Card triggerCards, int value)
    {
        S_FoeInfoSystem.Instance.FoeSpriteBouncingVFX();

        await AddOrSubtractHealth(triggerCards, value);
    }
    public async Task AddOrSubtractDetermination(S_Foe foe, S_Card triggerCards, int value)
    {
        S_FoeInfoSystem.Instance.FoeSpriteBouncingVFX();

        await AddOrSubtractDetermination(triggerCards, value);
    }
    public async Task AddOrSubtractGold(S_Foe foe, S_Card triggerCards, int value)
    {
        await AddOrSubtractGold(triggerCards, value);
    }
    #endregion
    #region VFX 보조
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
    void ShakeCamera(float power)
    {
        Vector3 originPos = Camera.main.transform.localPosition;

        Camera.main.DOKill();
        Camera.main.DOShakePosition(GetEffectLifeTime() / 2, power)
                 .OnComplete(() => Camera.main.transform.localPosition = originPos);
    }
    #endregion
}
