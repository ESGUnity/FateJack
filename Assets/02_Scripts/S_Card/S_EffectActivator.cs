using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_EffectActivator : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_EffectLog;

    [Header("컴포넌트")]
    S_EffectChecker checker;
    S_PlayerCard pCard;
    S_PlayerStat pStat;
    S_PlayerTrinket pTrinket;

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
        checker = GetComponent<S_EffectChecker>();
        pCard = S_PlayerCard.Instance;
        pStat = S_PlayerStat.Instance;
        pTrinket = S_PlayerTrinket.Instance;
    }

    #region 토탈 효과 발동
    public async Task ActivateByHit(S_Card hitCard, S_CardOrderTypeEnum type)
    {
        await ActivateCalcWeightByHit(hitCard, type);
        await ActivateTrinketByHit(hitCard);
        await ActivateFoeByHit(hitCard);
        await ActivateCardByHit(hitCard, type);
    }
    #endregion
    #region 카드 효과 발동 관련
    public void CheckCardMeetCondition() // 카드의 조건 계산(대부분 각인). 카드를 낼 때 항상 모든 카드 검사.
    {
        int count;
        foreach (S_Card card in pCard.GetStackCards())
        {
            if (card.IsCursed)
            {
                card.IsMeetCondition = false;
                card.IsEngravingActiaved = false;
                continue;
            }

            card.IsMeetCondition = true; // 기본적으로 카드 효과는 다 발동한다. 각인에 의해서 발동하지 않을 수도 있음.
            card.IsEngravingActiaved = true;

            switch (card.Engraving)
            {
                case S_EngravingEnum.Legion:
                    count = checker.GetSumInStack();
                    card.ActivatedCount = count;
                    if (count >= 30)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Legion_Flip:
                    count = checker.GetSumInStack();
                    card.ActivatedCount = count;
                    if (count >= 50)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.AllOut:
                    count = checker.GetSumInStack();
                    card.ActivatedCount = count;
                    if (count >= 15)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.AllOut_Flip:
                    count = checker.GetSumInStack();
                    card.ActivatedCount = count;
                    if (count >= 20)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Delicacy:
                    count = checker.GetCardsInStack().Count;
                    card.ActivatedCount = count;
                    if (count % 3 == 0)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Delicacy_Flip:
                    count = checker.GetCardsInStack().Count;
                    card.ActivatedCount = count;
                    if (count % 6 == 0)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Precision:
                    count = checker.GetCardsInStack().Count;
                    card.ActivatedCount = count;
                    if (count % 3 == 0)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Precision_Flip:
                    count = checker.GetCardsInStack().Count;
                    card.ActivatedCount = count;
                    if (count % 6 == 0)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Resection:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count <= 4)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Resection_Flip:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count <= 3)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Patience:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count <= 4)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Patience_Flip:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count <= 3)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Overflow:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count >= 5)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Overflow_Flip:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count >= 6)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Fierce:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count >= 5)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Fierce_Flip:
                    count = checker.GetCardsInStackInCurrentTurn().Count;
                    card.ActivatedCount = count;
                    if (count >= 6)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.GrandChaos:
                    count = checker.GetGrandChaosInStackInCurrentTurn(1).Count;
                    card.ActivatedCount = count;
                    if (count >= 4)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.GrandChaos_Flip:
                    count = checker.GetGrandChaosInStackInCurrentTurn(2).Count;
                    card.ActivatedCount = count;
                    if (count >= 4)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Crush:
                    count = checker.GetGrandChaosInStackInCurrentTurn(1).Count;
                    card.ActivatedCount = count;
                    if (count >= 4)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Crush_Flip:
                    count = checker.GetGrandChaosInStackInCurrentTurn(2).Count;
                    card.ActivatedCount = count;
                    if (count >= 4)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Overdrive:
                    if (pStat.GetCurrentHealth() == 1)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Immersion:
                    if (pStat.GetCurrentHealth() == 1)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Finale:
                    count = pCard.GetDeckCards().Count;
                    card.ActivatedCount = count;
                    if (count == 0)
                    {
                        card.IsMeetCondition = true;
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsMeetCondition = false;
                        card.IsEngravingActiaved = false;
                    }
                    break;
                case S_EngravingEnum.Climax:
                    count = pCard.GetDeckCards().Count;
                    card.ActivatedCount = count;
                    if (count == 0)
                    {
                        card.IsEngravingActiaved = true;
                    }
                    else
                    {
                        card.IsEngravingActiaved = false;
                    }
                    break;
            }
        }

        S_StackInfoSystem.Instance.UpdateStackCardState();
    }
    async Task ActivateCalcWeightByHit(S_Card hitCard, S_CardOrderTypeEnum type)
    {
        if (pStat.IsDelusion) // 망상일 경우 저주하기
        {
            // 망상 초기화
            pStat.IsDelusion = false;
            S_StatInfoSystem.Instance.UpdateSpecialAbility();

            // 저주 처리
            await CurseCard(hitCard, null);

            // 로그 생성
            GenerateEffectLog("망상 해제됨!");
            await WaitEffectLifeTimeAsync();
        }

        // 힘, 정신력, 행운, 공통 카드 낼 때마다 저주하는 쓸만한 물건 패시브
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.CurseStr, out S_Trinket tri1))
        {
            if (hitCard.CardType == S_CardTypeEnum.Str)
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri1);

                await CurseCard(hitCard, null);
            }
        }
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.CurseMind, out S_Trinket tri2))
        {
            if (hitCard.CardType == S_CardTypeEnum.Mind)
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri2);

                await CurseCard(hitCard, null);
            }
        }
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.CurseLuck, out S_Trinket tri3)) 
        {
            if (hitCard.CardType == S_CardTypeEnum.Luck)
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri3);

                await CurseCard(hitCard, null);
            }
        }
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.CurseCommon, out S_Trinket tri4)) 
        {
            if (hitCard.CardType == S_CardTypeEnum.Common)
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri4);

                await CurseCard(hitCard, null);
            }
        }

        // 힘, 정신력, 행운, 공통 카드 낼 때마다 저주하는 적 패시브
        if (S_FoeInfoSystem.Instance.IsFoeHavePassive(S_TrinketPassiveEnum.CurseStr))
        {
            if (hitCard.CardType == S_CardTypeEnum.Str)
            {
                S_FoeInfoSystem.Instance.FoeBouncingVFX();

                await CurseCard(hitCard, null);
            }
        }
        if (S_FoeInfoSystem.Instance.IsFoeHavePassive(S_TrinketPassiveEnum.CurseMind))
        {
            if (hitCard.CardType == S_CardTypeEnum.Mind)
            {
                S_FoeInfoSystem.Instance.FoeBouncingVFX();

                await CurseCard(hitCard, null);
            }
        }
        if (S_FoeInfoSystem.Instance.IsFoeHavePassive(S_TrinketPassiveEnum.CurseLuck))
        {
            if (hitCard.CardType == S_CardTypeEnum.Luck)
            {
                S_FoeInfoSystem.Instance.FoeBouncingVFX();

                await CurseCard(hitCard, null);
            }
        }
        if (S_FoeInfoSystem.Instance.IsFoeHavePassive(S_TrinketPassiveEnum.CurseCommon))
        {
            if (hitCard.CardType == S_CardTypeEnum.Common)
            {
                S_FoeInfoSystem.Instance.FoeBouncingVFX();

                await CurseCard(hitCard, null);
            }
        }

        if (pStat.IsColdBlood) // 냉혈일 경우 무게 계산 건너뛰기
        {
            // 냉혈 초기화
            pStat.IsColdBlood = false;
            S_StatInfoSystem.Instance.UpdateSpecialAbility();

            // 바운싱 카드
            if (hitCard != null)
            {
                S_StackInfoSystem.Instance.BouncingStackCard(hitCard);
            }

            // 로그 생성
            GenerateEffectLog("냉혈 사용됨!");
            await WaitEffectLifeTimeAsync();
        }
        else if (type == S_CardOrderTypeEnum.Gen) // 생성되었다면 무게 계산 건너뛰기
        {

        }
        else // 냉혈이 아니라면 무게 계산
        {
            await AddOrSubtractWeight(hitCard, hitCard.Num);
        }
    }
    async Task ActivateCardByHit(S_Card hitCard, S_CardOrderTypeEnum type) // 발현 각인이 있는 카드 발동
    {
        if (hitCard.Engraving == S_EngravingEnum.Unleash) // 발현 각인이 있다면 효과 발동
        {
            if (hitCard.IsCursed) // 저주받은 카드는 패스
            {
                await ActivateCursedCardEffect(hitCard);
                return;
            }

            if (!hitCard.IsMeetCondition) // 조건 충족이 안된 카드도 패스
            {
                return;
            }

            await ActivateCardEffectByEngraving(hitCard);
        }

        // 쓸만한 물건 패시브에 의한 효과 발동(발현)
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.LuckPerUnleashTrig, out S_Trinket tri2))
        {
            float criticalBlowFloat = pStat.CurrentLuck * 0.01f;
            if (Random.Range(0f, 1f) < criticalBlowFloat)
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri2);

                await ActivateCardEffectByEngraving(hitCard);
            }
        }
    }

    public async Task ActivatedCardByStand() // 스탠드 시 카드 효과 발동
    {
        // 수레바퀴 패시브 효과 발동(쓸물)
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.FirstCard2Trig, out S_Trinket tri1))
        {
            List<S_Card> cards = checker.GetCardsInStackInCurrentTurn();
            if (cards.Count > 0)
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri1);

                await ActivateCardEffectByEngraving(cards.First());
                await ActivateCardEffectByEngraving(cards.First());
            }
        }

        int index = 0;
        while (index < pCard.GetStackCards().Count)
        {

            if (pCard.GetStackCards()[index].IsCursed) // 저주받은 카드는 패스
            {
                await ActivateCursedCardEffect(pCard.GetStackCards()[index]);
                index++;
                continue;
            }
            if (!pCard.GetStackCards()[index].IsMeetCondition) // 조건 충족이 안된 카드도 패스
            {
                index++;
                continue;
            }

            // 카드 효과 발동
            await ActivateCardEffectByEngraving(pCard.GetStackCards()[index]);

            // 쓸만한 물건 패시브에 의한 효과 발동
            if (pCard.GetStackCards()[index].IsGenerated && pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Gen1Trig, out S_Trinket tri2))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri2);

                await ActivateCardEffectByEngraving(pCard.GetStackCards()[index]);
            }
            if (pCard.GetStackCards()[index].CardType == S_CardTypeEnum.Luck && pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.LuckPer1LuckTrig, out S_Trinket tri3))
            {
                float criticalBlowFloat = pStat.CurrentLuck * 0.01f;
                if (Random.Range(0f, 1f) < criticalBlowFloat)
                {
                    S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri3);

                    await ActivateCardEffectByEngraving(pCard.GetStackCards()[index]);
                }
            }
            if ((pCard.GetStackCards()[index].CardType == S_CardTypeEnum.Mind || pCard.GetStackCards()[index].CardType == S_CardTypeEnum.Luck) && pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoStrMindLuck1Trig, out S_Trinket tri4))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri4);

                await ActivateCardEffectByEngraving(pCard.GetStackCards()[index]);
            }
            if ((pCard.GetStackCards()[index].CardType == S_CardTypeEnum.Str || pCard.GetStackCards()[index].CardType == S_CardTypeEnum.Luck) && pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoMindStrLuck1Trig, out S_Trinket tri5))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri5);

                await ActivateCardEffectByEngraving(pCard.GetStackCards()[index]);
            }
            if ((pCard.GetStackCards()[index].CardType == S_CardTypeEnum.Str || pCard.GetStackCards()[index].CardType == S_CardTypeEnum.Mind) && pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoLuckStrMind1Trig, out S_Trinket tri6))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri6);

                await ActivateCardEffectByEngraving(pCard.GetStackCards()[index]);
            }

            index++;
        }
    }
    async Task OnlyActivateCardEffect(S_Card card) // 카드 효과만 발동하기
    {
        int value;

        switch (card.CardEffect)
        {
            case S_CardEffectEnum.Str_Stimulus:
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Str, 2);
                break;
            case S_CardEffectEnum.Str_ZenithBreak:
                await MultiBattleStat(card, S_BattleStatEnum.Str, 1.5f);
                break;
            case S_CardEffectEnum.Str_SinisterImpulse:
                await GetDelusion(card);
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Str, 8);
                break;
            case S_CardEffectEnum.Str_CalamityApproaches:
                await CurseRandomCards(2, S_CardTypeEnum.None, false, true, card);
                await MultiBattleStat(card, S_BattleStatEnum.Str, 1.5f);
                break;
            case S_CardEffectEnum.Str_UntappedPower:
                S_BattleStatEnum untappedPowerStat = checker.GetHighestStats(out value);
                await AddOrSubtractBattleStat(card, untappedPowerStat, value);
                await MultiBattleStat(card, S_BattleStatEnum.Str, 3.0f);
                break;
            case S_CardEffectEnum.Str_UnjustSacrifice:
                value = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count + checker.GetSameTypeCardsInStack(S_CardTypeEnum.Luck).Count + checker.GetSameTypeCardsInStack(S_CardTypeEnum.Common).Count;
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Str, value * 5);
                break;
            case S_CardEffectEnum.Str_WrathStrike:
                await HarmFoe(card, S_BattleStatEnum.Str, 3);
                break;
            case S_CardEffectEnum.Str_EngulfInFlames:
                if (pStat.IsBurst)
                {
                    await CurseCard(card, null);
                    break;
                }
                await HarmFoe(card, S_BattleStatEnum.Str, 12);
                break;
            case S_CardEffectEnum.Str_FinishingStrike:
                List<S_Card> finishingStrikeCards = pCard.GetStackCards();
                int index = finishingStrikeCards.IndexOf(card);
                for (int i = index + 1; i < finishingStrikeCards.Count; i++)
                {
                    await CurseCard(finishingStrikeCards[i], card);
                }
                await HarmFoe(card, S_BattleStatEnum.Str, 24);
                break;
            case S_CardEffectEnum.Str_FlowingSin:
                await HarmFoe(card, S_BattleStatEnum.Str, 4);
                for (int i = 0; i < checker.GetCursedCardsInDeckAndStack().Count; i++)
                {
                    await HarmFoe(card, S_BattleStatEnum.Str, 4);
                }
                break;
            case S_CardEffectEnum.Str_BindingForce:
                await HarmFoe(card, S_BattleStatEnum.Str_Luck, 1);
                for (int i = 0; i < checker.GetSameTypeCardsInStack(S_CardTypeEnum.Str).Count; i++)
                {
                    await HarmFoe(card, S_BattleStatEnum.Str_Luck, 1);
                }
                break;
            case S_CardEffectEnum.Str_Grudge:
                if (S_FoeInfoSystem.Instance.FoeInfo.CurrentHealth >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.75f)
                {
                    await HarmFoe(card, S_BattleStatEnum.Str_Mind, 1);
                }
                else
                {
                    await HarmFoe(card, S_BattleStatEnum.Str, 1);
                }
                break;

            // 정신력
            case S_CardEffectEnum.Mind_Focus:
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Mind, 2);
                break;
            case S_CardEffectEnum.Mind_DeepInsight:
                await MultiBattleStat(card, S_BattleStatEnum.Mind, 1.5f);
                break;
            case S_CardEffectEnum.Mind_PerfectForm:
                if (!pStat.IsBurst)
                {
                    value = pStat.CurrentLimit - pStat.CurrentWeight;
                    await AddOrSubtractWeight(card, value);
                    await AddOrSubtractBattleStat(card, S_BattleStatEnum.Mind, value);
                }
                break;
            case S_CardEffectEnum.Mind_Unshackle:
                value = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count;
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Mind, value * 3);
                break;
            case S_CardEffectEnum.Mind_Drain:
                value = 0;
                if (pStat.CurrentStr >= 5)
                {
                    value += 5;
                    await AddOrSubtractBattleStat(card, S_BattleStatEnum.Str, -5);
                }
                else
                {
                    value += pStat.CurrentStr;
                    await AddOrSubtractBattleStat(card, S_BattleStatEnum.Str, -pStat.CurrentStr);
                }

                if (pStat.CurrentLuck >= 5)
                {
                    value += 5;
                    await AddOrSubtractBattleStat(card, S_BattleStatEnum.Luck, -5);
                }
                else
                {
                    value += pStat.CurrentLuck;
                    await AddOrSubtractBattleStat(card, S_BattleStatEnum.Luck, -pStat.CurrentLuck);
                }

                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Mind, value * 2);
                break;
            case S_CardEffectEnum.Mind_WingsOfFreedom:
                await AddOrSubtractLimit(card, -5);
                await MultiBattleStat(card, S_BattleStatEnum.Mind, 1.5f);
                break;
            case S_CardEffectEnum.Mind_PreciseStrike:
                await HarmFoe(card, S_BattleStatEnum.Mind, 3);
                break;
            case S_CardEffectEnum.Mind_SharpCut:
                if (pStat.IsPerfect)
                {
                    await HarmFoe(card, S_BattleStatEnum.Mind, 12);
                }
                else
                {
                    await HarmFoe(card, S_BattleStatEnum.Mind, 1);
                }
                break;
            case S_CardEffectEnum.Mind_Split:
                await HarmFoe(card, S_BattleStatEnum.Mind, pStat.CurrentWeight);
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Mind, pStat.CurrentWeight);
                break;
            case S_CardEffectEnum.Mind_Accept:
                await AddOrSubtractLimit(card, pStat.CurrentMind / 10);
                break;
            case S_CardEffectEnum.Mind_Dissolute:
                await HarmFoe(card, S_BattleStatEnum.Str_Mind, 1);
                if (pStat.IsBurst)
                {
                    await MultiBattleStat(card, S_BattleStatEnum.Mind, 0.5f);
                }
                break;
            case S_CardEffectEnum.Mind_Awakening:
                if (pStat.IsPerfect)
                {
                    await HarmFoe(card, S_BattleStatEnum.Mind_Luck, 1);
                }
                else
                {
                    await HarmFoe(card, S_BattleStatEnum.Mind, 1);
                }
                break;

            // 행운
            case S_CardEffectEnum.Luck_Chance:
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Luck, 2);
                break;
            case S_CardEffectEnum.Luck_Disorder:
                await MultiBattleStat(card, S_BattleStatEnum.Luck, 1.5f);
                break;
            case S_CardEffectEnum.Luck_Composure:
                value = 0;
                List<S_Card> composureCards = pCard.GetStackCards();
                int composureIndex = composureCards.IndexOf(card);
                for (int i = composureIndex + 1; i < composureCards.Count; i++)
                {
                    value++;
                }
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Luck, value);
                break;
            case S_CardEffectEnum.Luck_SilentDomination:
                S_Card genCard = S_CardManager.Instance.GenerateRandomCard();
                await GenCard(card, genCard);
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.Luck, genCard.Num);
                break;
            case S_CardEffectEnum.Luck_Artifice:
                List<S_Card> artificeCards = pCard.GetStackCards();
                int artificeIndex = artificeCards.IndexOf(card);

                if (artificeIndex + 1 < artificeCards.Count)
                {
                    S_Card nextCard = artificeCards[artificeIndex + 1];

                    if (nextCard != null)
                    {
                        await RetriggerCard(card, nextCard);

                        if (nextCard.CardType != S_CardTypeEnum.Luck)
                        {
                            await AddOrSubtractBattleStat(card, S_BattleStatEnum.Luck, 12);
                        }
                    }
                }
                break;
            case S_CardEffectEnum.Luck_AllForOne:
                S_Card gen1 = S_CardManager.Instance.GenerateRandomCard();
                await GenCard(card, gen1);
                S_Card gen2 = S_CardManager.Instance.GenerateRandomCard();
                await GenCard(card, gen2);
                S_Card gen3 = S_CardManager.Instance.GenerateRandomCard();
                await GenCard(card, gen3);

                if (gen1.CardType != S_CardTypeEnum.Luck)
                {
                    await MultiBattleStat(card, S_BattleStatEnum.Luck, 1.5f);
                }
                if (gen2.CardType != S_CardTypeEnum.Luck)
                {
                    await MultiBattleStat(card, S_BattleStatEnum.Luck, 1.5f);
                }
                if (gen3.CardType != S_CardTypeEnum.Luck)
                {
                    await MultiBattleStat(card, S_BattleStatEnum.Luck, 1.5f);
                }
                break;
            case S_CardEffectEnum.Luck_SuddenStrike:
                await HarmFoe(card, S_BattleStatEnum.Luck, 3);
                break;
            case S_CardEffectEnum.Luck_CriticalBlow:
                float criticalBlowFloat = pStat.CurrentLuck * 0.01f;
                if (Random.Range(0f, 1f) < criticalBlowFloat)
                {
                    await HarmFoe(card, S_BattleStatEnum.Luck, 16);
                }
                break;
            case S_CardEffectEnum.Luck_ForcedTake:
                await HarmFoeByFloat(card, S_BattleStatEnum.Luck, 0.5f);
                value = Mathf.RoundToInt(pStat.CurrentLuck * 0.5f);
                await AddOrSubtractGold(card, value);
                break;
            case S_CardEffectEnum.Luck_Grill:
                List<S_Card> grillCards = pCard.GetStackCards();
                int grillIndex = grillCards.IndexOf(card);

                if (grillIndex + 1 < grillCards.Count)
                {
                    S_Card nextCard = grillCards[grillIndex + 1];

                    if (nextCard != null)
                    {
                        for (int i = 0; i < pStat.CurrentLuck / 10; i++)
                        {
                            await RetriggerCard(card, nextCard);
                        }
                    }
                }
                break;
            case S_CardEffectEnum.Luck_Shake:
                await HarmFoeByFloat(card, S_BattleStatEnum.Str_Luck, 0.25f);
                break;
            case S_CardEffectEnum.Luck_FatalBlow:
                await HarmFoe(card, S_BattleStatEnum.Str_Mind, 1);
                for (int i = 0; i < pStat.CurrentLuck / 10; i++)
                {
                    await HarmFoe(card, S_BattleStatEnum.Str_Mind, 1);
                }
                break;

            // 공용
            case S_CardEffectEnum.Common_Trinity:
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.AllStat, 3);
                break;
            case S_CardEffectEnum.Common_Balance:
                await MultiBattleStat(card, card.Stat, 2);
                break;
            case S_CardEffectEnum.Common_Berserk:
                await HarmFoe(card, card.Stat, 1);
                break;
            case S_CardEffectEnum.Common_Carnage:
                await HarmFoe(card, S_BattleStatEnum.AllStat, 1);
                break;
            case S_CardEffectEnum.Common_LastStruggle:
                await AddOrSubtractBattleStat(card, S_BattleStatEnum.AllStat, -15);
                await HarmFoe(card, S_BattleStatEnum.AllStat, 1);
                break;
            case S_CardEffectEnum.Common_Resistance:
                await AddOrSubtractLimit(card, 1);
                break;
            case S_CardEffectEnum.Common_Realization:
                List<S_Card> realizationCards = pCard.GetStackCards();
                int realizationIndex = realizationCards.IndexOf(card);
                int realizationLimit = 0;
                for (int i = realizationIndex + 1; i < realizationCards.Count; i++)
                {
                    realizationLimit++;
                }
                int realizationWeight = 0;
                for (int i = realizationIndex + 1; i >= 0; i--)
                {
                    realizationWeight++;
                }
                await AddOrSubtractLimit(card, realizationLimit);
                await AddOrSubtractWeight(card, realizationWeight);
                break;
            case S_CardEffectEnum.Common_Corrupt:
                await GenCard(card);
                break;
            case S_CardEffectEnum.Common_Imitate:
                List<S_Card> imitateCards = pCard.GetStackCards();
                int imitateIndex = imitateCards.IndexOf(card);

                if (imitateIndex + 1 < imitateCards.Count)
                {
                    S_Card nextCard = imitateCards[imitateIndex + 1];

                    if (nextCard != null)
                    {
                        await GenCard(card, nextCard);
                    }
                }
                break;
            case S_CardEffectEnum.Common_Plunder:
                await AddOrSubtractGold(card, 2);
                break;
            case S_CardEffectEnum.Common_Undertow:
                List<S_Card> undertowCards = pCard.GetStackCards();
                int undertowIndex = undertowCards.IndexOf(card);

                if (undertowIndex + 1 < undertowCards.Count)
                {
                    S_Card nextCard = undertowCards[undertowIndex + 1];

                    if (nextCard != null)
                    {
                        await RetriggerCard(card, nextCard);
                    }
                }
                break;
            case S_CardEffectEnum.Common_Adventure:
                await GetExpansion(card);
                break;
            case S_CardEffectEnum.Common_Inspiration:
                await GetFirst(card);
                break;
            case S_CardEffectEnum.Common_Repose:
                await GetColdBlood(card);
                break;
        }
    }
    async Task ActivateCardEffectByEngraving(S_Card card) // 각인 포함된 발동 메서드. 망상이랑 조건 미달 검사는 실제 발동하는 곳에서
    {
        // 먼저 효과 한 번 발동.
        await OnlyActivateCardEffect(card);

        int count;

        // 각인에 의한 효과 추가 발동
        switch (card.Engraving)
        {
            case S_EngravingEnum.None:
                break;
            case S_EngravingEnum.AllOut:
                for (int i = 0; i < checker.GetSumInStack() / 15; i++)
                {
                    await OnlyActivateCardEffect(card);
                }
                break;
            case S_EngravingEnum.AllOut_Flip:
                for (int i = 0; i < checker.GetSumInStack() / 20; i++)
                {
                    await OnlyActivateCardEffect(card);
                }
                break;
            case S_EngravingEnum.Precision:
                count = checker.GetCardsInStack().Count;
                if (count % 3 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Precision_Flip:
                count = checker.GetCardsInStack().Count;
                if (count % 6 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Patience:
                count = checker.GetCardsInStackInCurrentTurn().Count;
                if (count <= 4)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Patience_Flip:
                count = checker.GetCardsInStackInCurrentTurn().Count;
                if (count <= 3)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Fierce:
                count = checker.GetCardsInStackInCurrentTurn().Count;
                if (count >= 5)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Fierce_Flip:
                count = checker.GetCardsInStackInCurrentTurn().Count;
                if (count >= 6)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Crush:
                count = checker.GetGrandChaosInStackInCurrentTurn(1).Count;
                if (count >= 4)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Crush_Flip:
                count = checker.GetGrandChaosInStackInCurrentTurn(2).Count;
                if (count >= 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Immersion:
                if (pStat.GetCurrentHealth() == 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Climax:
                if (pCard.GetDeckCards().Count == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        await OnlyActivateCardEffect(card);
                    }
                }
                break;
            case S_EngravingEnum.Spell:
                List<S_Card> curseTarget = checker.GetCardsInStack().Where(x => !x.IsCursed).ToList();
                if (curseTarget.Count > 0)
                {
                    await CurseCard(curseTarget.OrderBy(x => Random.value).Take(1).First(), card);
                }
                break;
            case S_EngravingEnum.DeepShadow:
                await GetDelusion(card);
                break;
            default: break;
        }
    }

    public async Task AppliedFirstAsync() // 우선 사용 시 호출
    {
        pStat.IsFirst = false;

        // 우선 해제하는 효과
        GenerateEffectLog("우선 사용됨!");
        S_StatInfoSystem.Instance.UpdateSpecialAbility();

        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    public async Task AppliedExpansionAsync() // 전개 사용 시 호출
    {
        pStat.IsExpansion = false;

        // 전개 해제하는 효과
        GenerateEffectLog("전개 사용됨!");
        S_StatInfoSystem.Instance.UpdateSpecialAbility();

        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    public async Task ActivateCursedCardEffect(S_Card triggerCard) // 저주받은 카드가 효과를 발동하려할 때
    {
        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        GenerateEffectLog("저주받은 카드!");

        // 플레이어 이미지 VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Cursed, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
    }

    public void CalcExpectedValue() // 예상 값 계산 메서드.(호버링 설명 용)
    {
        int temp;
        int expStr = pStat.CurrentStr;
        int expMind = pStat.CurrentMind;
        int expLuck = pStat.CurrentLuck;
        int accHarm = 0;

        // 카드의 예상 값 계산
        List<S_Card> cards = pCard.GetStackCards();
        for (int i = 0; i < cards.Count; i++)
        {
            S_Card card = cards[i];
            switch (card.CardEffect)
            {
                case S_CardEffectEnum.Str_Stimulus: expStr += 2; break;
                case S_CardEffectEnum.Str_ZenithBreak:
                    temp = Mathf.RoundToInt(expStr * 0.5f);
                    expStr += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_SinisterImpulse: expStr += 8; break;
                case S_CardEffectEnum.Str_CalamityApproaches:
                    temp = Mathf.RoundToInt(expStr * 0.5f);
                    expStr += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_UntappedPower:
                    var stats = new (S_BattleStatEnum stat, int val)[] { (S_BattleStatEnum.Str, expStr), (S_BattleStatEnum.Mind, expMind), (S_BattleStatEnum.Luck, expLuck) };
                    var max = stats[0];
                    foreach (var stat in stats)
                    {
                        if (stat.val > max.val)
                        {
                            max = stat;
                        }
                    }
                    if (max.stat == S_BattleStatEnum.Str) 
                    {
                        temp = 0;
                        expStr = 0;
                        card.ExpectedValue = 0;
                    }
                    else
                    {
                        temp = expStr * 2;
                        expStr += temp;
                        card.ExpectedValue = temp;
                    }
                    break;
                case S_CardEffectEnum.Str_UnjustSacrifice:
                    temp = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count + checker.GetSameTypeCardsInStack(S_CardTypeEnum.Luck).Count + checker.GetSameTypeCardsInStack(S_CardTypeEnum.Common).Count;
                    temp *= 5;
                    expStr += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_WrathStrike:
                    temp = expStr * 3;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_EngulfInFlames:
                    temp = expStr * 12;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_FinishingStrike:
                    temp = expStr * 24;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_FlowingSin:
                    temp = expStr * 4;
                    temp += expStr * 4 * (checker.GetCursedCardsInDeckAndStack().Count);
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_BindingForce:
                    temp = expStr * expLuck;
                    temp += temp * (checker.GetSameTypeCardsInStack(S_CardTypeEnum.Str).Count);
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Str_Grudge:
                    int foeHp = S_FoeInfoSystem.Instance.FoeInfo.CurrentHealth - accHarm;
                    if (foeHp >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.75f)
                    {
                        temp = expStr * expMind;
                    }
                    else
                    {
                        temp = expStr;
                    }
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;

                // 정신력
                case S_CardEffectEnum.Mind_Focus:
                    expMind += 2; 
                    break;
                case S_CardEffectEnum.Mind_DeepInsight:
                    temp = Mathf.RoundToInt(expMind * 0.5f);
                    expMind += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Mind_PerfectForm:
                    if (!pStat.IsBurst)
                    {
                        temp = pStat.CurrentLimit - pStat.CurrentWeight;
                        expMind += temp;
                        card.ExpectedValue = temp;
                    }
                    else
                    {
                        card.ExpectedValue = 0;
                    }
                    break;
                case S_CardEffectEnum.Mind_Unshackle:
                    temp = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count;
                    temp *= 3;
                    expMind += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Mind_Drain:
                    temp = 0;
                    if (pStat.CurrentStr >= 5)
                    {
                        temp += 5;
                    }
                    else
                    {
                        temp += pStat.CurrentStr;
                    }

                    if (pStat.CurrentLuck >= 5)
                    {
                        temp += 5;
                    }
                    else
                    {
                        temp += pStat.CurrentLuck;
                    }
                    expMind += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Mind_WingsOfFreedom:
                    temp = Mathf.RoundToInt(expMind * 0.5f);
                    expMind += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Mind_PreciseStrike:
                    temp = expMind * 3;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Mind_SharpCut:
                    if (pStat.IsPerfect)
                    {
                        temp = expMind * 12;
                    }
                    else
                    {
                        temp = expMind;
                    }
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Mind_Split:
                    temp = expMind * pStat.CurrentWeight;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    expMind -= pStat.CurrentWeight;
                    break;
                case S_CardEffectEnum.Mind_Accept:
                    card.ExpectedValue = pStat.CurrentMind / 10;
                    break;
                case S_CardEffectEnum.Mind_Dissolute:
                    temp = expStr * expMind;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    if (pStat.IsBurst)
                    {
                        expMind = Mathf.RoundToInt(expMind * 0.5f);
                    }
                    break;
                case S_CardEffectEnum.Mind_Awakening:
                    if (pStat.IsPerfect)
                    {
                        temp = expMind * expLuck;
                    }
                    else
                    {
                        temp = expMind;
                    }
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;

                // 행운
                case S_CardEffectEnum.Luck_Chance:
                    expLuck += 2;
                    break;
                case S_CardEffectEnum.Luck_Disorder:
                    temp = Mathf.RoundToInt(expLuck * 0.5f);
                    expLuck += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Luck_Composure:
                    temp = 0;
                    List<S_Card> composureCards = pCard.GetStackCards();
                    int composureIndex = composureCards.IndexOf(card);
                    for (int j = composureIndex + 1; j < composureCards.Count; j++)
                    {
                        temp++;
                    }
                    expLuck += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Luck_SilentDomination:
                    break;
                case S_CardEffectEnum.Luck_Artifice:
                    List<S_Card> artificeCards = pCard.GetStackCards();
                    int artificeIndex = artificeCards.IndexOf(card);
                    if (artificeIndex + 1 < artificeCards.Count)
                    {
                        S_Card nextCard = artificeCards[artificeIndex + 1];

                        if (nextCard != null)
                        {
                            if (nextCard.CardType != S_CardTypeEnum.Luck)
                            {
                                expLuck += 12;
                            }
                        }
                    }
                    break;
                case S_CardEffectEnum.Luck_AllForOne:
                    break;
                case S_CardEffectEnum.Luck_SuddenStrike:
                    temp = expLuck * 3;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Luck_CriticalBlow:
                    temp = expLuck * 16;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Luck_ForcedTake:
                    temp = Mathf.RoundToInt(expLuck * 0.5f);
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Luck_Grill:
                    card.ExpectedValue = expLuck / 10;
                    break;
                case S_CardEffectEnum.Luck_Shake:
                    temp = Mathf.RoundToInt(expStr * expLuck * 0.25f);
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Luck_FatalBlow:
                    temp = expStr * expMind;
                    temp = CalcExpectedValueByHarm(temp);
                    temp += temp * expLuck / 10;
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;

                // 공용
                case S_CardEffectEnum.Common_Trinity:
                    expStr += 3;
                    expMind += 3;
                    expLuck += 3;
                    break;
                case S_CardEffectEnum.Common_Balance:
                    if (card.Stat == S_BattleStatEnum.Str)
                    {
                        temp = Mathf.RoundToInt(expStr);
                        expStr += temp;
                        card.ExpectedValue = temp;
                    }
                    else if (card.Stat == S_BattleStatEnum.Mind)
                    {
                        temp = Mathf.RoundToInt(expMind);
                        expMind += temp;
                        card.ExpectedValue = temp;
                    }
                    else if (card.Stat == S_BattleStatEnum.Luck)
                    {
                        temp = Mathf.RoundToInt(expLuck);
                        expLuck += temp;
                        card.ExpectedValue = temp;
                    }
                    else
                    {
                        temp = 0;
                        card.ExpectedValue = 0;
                        Debug.Log("EffectActivator Send : CalcExpectedValue Method Has Error");
                    }
                    break;
                case S_CardEffectEnum.Common_Berserk:
                    if (card.Stat == S_BattleStatEnum.Str_Mind)
                    {
                        temp = expStr * expMind;
                    }
                    else if (card.Stat == S_BattleStatEnum.Str_Luck)
                    {
                        temp = expStr * expLuck;
                    }
                    else if (card.Stat == S_BattleStatEnum.Mind_Luck)
                    {
                        temp = expMind * expLuck;
                    }
                    else
                    {
                        temp = 0;
                        Debug.Log("EffectActivator Send : CalcExpectedValue Method Has Error");
                    }
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Common_Carnage:
                    temp = expStr * expMind * expLuck;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Common_LastStruggle:
                    expStr -= 15;
                    expMind -= 15;
                    expLuck -= 15;
                    temp = expStr * expMind * expLuck;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    card.ExpectedValue = temp;
                    break;
                case S_CardEffectEnum.Common_Resistance:
                    break;
                case S_CardEffectEnum.Common_Realization:
                    break;
                case S_CardEffectEnum.Common_Corrupt:
                    break;
                case S_CardEffectEnum.Common_Imitate:
                    break;
                case S_CardEffectEnum.Common_Plunder:
                    break;
                case S_CardEffectEnum.Common_Undertow:
                    break;
                case S_CardEffectEnum.Common_Adventure:
                    break;
                case S_CardEffectEnum.Common_Inspiration:
                    break;
                case S_CardEffectEnum.Common_Repose:
                    break;
            }
        }

        // 쓸만한 물건의 예상 값 계산
        List<S_Trinket> tris = pTrinket.GetPlayerOwnedTrinkets();
        for (int i = 0; i < tris.Count; i++)
        {
            S_Trinket tri = tris[i];
            switch (tri.Effect)
            {
                case S_TrinketEffectEnum.Stat_Multi:
                    if (tri.Stat == S_BattleStatEnum.Str)
                    {
                        temp = Mathf.RoundToInt(expStr * (tri.FloatValue - 1));
                    }
                    else if (tri.Stat == S_BattleStatEnum.Mind)
                    {
                        temp = Mathf.RoundToInt(expMind * (tri.FloatValue - 1));
                    }
                    else if (tri.Stat == S_BattleStatEnum.Luck)
                    {
                        temp = Mathf.RoundToInt(expLuck * (tri.FloatValue - 1));
                    }
                    else
                    {
                        temp = 0;
                        Debug.Log("S_EffectActivate Send : CalcExpectedValue Methos Error");
                    }
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    tri.ExpectedValue = temp;
                    break;
                case S_TrinketEffectEnum.Harm_TwoStat_Random:
                    if (tri.Stat == S_BattleStatEnum.Str_Mind)
                    {
                        temp = expStr * expMind;
                    }
                    else if (tri.Stat == S_BattleStatEnum.Str_Luck)
                    {
                        temp = expStr * expLuck;
                    }
                    else if (tri.Stat == S_BattleStatEnum.Mind_Luck)
                    {
                        temp = expMind * expLuck;
                    }
                    else
                    {
                        temp = 0;
                        Debug.Log("EffectActivator Send : CalcExpectedValue Method Has Error");
                    }
                    temp = CalcExpectedValueByHarm(temp);

                    switch (tri.Condition)
                    {
                        case S_TrinketConditionEnum.Always:
                            break;
                        case S_TrinketConditionEnum.Precision_Six:
                            temp *= 3;
                            break;
                        case S_TrinketConditionEnum.Legion_Twenty:
                            temp *= checker.GetSumInStack() / 20 * 2;
                            break;
                        case S_TrinketConditionEnum.Resection_Three:
                            temp *= 3;
                            break;
                        case S_TrinketConditionEnum.Overflow_Six:
                            temp *= 3;
                            break;
                    }
                    accHarm += temp;
                    tri.ExpectedValue = temp;
                    break;
                case S_TrinketEffectEnum.Harm:
                    if (tri.Stat == S_BattleStatEnum.Str)
                    {
                        temp = expStr;
                    }
                    else if (tri.Stat == S_BattleStatEnum.Mind)
                    {
                        temp = expMind;
                    }
                    else if (tri.Stat == S_BattleStatEnum.Luck)
                    {
                        temp = expLuck;
                    }
                    else
                    {
                        temp = 0;
                        Debug.Log("EffectActivator Send : CalcExpectedValue Method Has Error");
                    }
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    tri.ExpectedValue = temp;
                    break;
                case S_TrinketEffectEnum.Harm_AllStat:
                    temp = expStr * expMind * expLuck;
                    temp = CalcExpectedValueByHarm(temp);
                    accHarm += temp;
                    tri.ExpectedValue = temp;
                    break;
            }
        }
    }
    public int CalcExpectedValueByHarm(int temp) // (지우개)버스트, 완벽, 가시손잡이 뼈톱 등
    {
        bool noBP = pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoBurstPerfect, out S_Trinket tri2);

        if (pStat.IsBurst && !noBP)
        {
            temp = Mathf.RoundToInt(temp * 0.25f);
        }
        if (pStat.IsPerfect && !noBP)
        {
            temp = Mathf.RoundToInt(temp * 2);
        }

        // 피해량에 20 곱하는 애
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Multi20Harm, out S_Trinket tri3))
        {
            temp *= 20;
        }

        return temp;
    }
    #endregion
    #region 쓸만한 물건 효과 발동 관련
    public void CheckTrinketMeetCondition(S_Card hitCard = null) // 시련 시작 시, 카드 낼 때, 비틀기, 효과 발동 시 틈틈이. 낼 때마다 조건이 있기에 매개변수 필요
    {
        int count;
        int mod;
        foreach (S_Trinket tri in S_PlayerTrinket.Instance.GetPlayerOwnedTrinkets())
        {
            switch (tri.Condition)
            {
                case S_TrinketConditionEnum.None:
                    tri.IsMeetCondition = false;
                    break;
                case S_TrinketConditionEnum.StartTrial:
                    tri.IsMeetCondition = false;
                    break;
                case S_TrinketConditionEnum.Always:
                    tri.IsMeetCondition = true;
                    break;
                case S_TrinketConditionEnum.Reverb_One:
                    if (hitCard == null)
                    {
                        break;
                    }

                    switch (tri.Modify)
                    {
                        case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                        case S_TrinketModifyEnum.Any: tri.IsMeetCondition = true; break;
                        case S_TrinketModifyEnum.Str:
                            if (hitCard.CardType == S_CardTypeEnum.Str)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                        case S_TrinketModifyEnum.Mind:
                            if (hitCard.CardType == S_CardTypeEnum.Mind)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                        case S_TrinketModifyEnum.Luck:
                            if (hitCard.CardType == S_CardTypeEnum.Luck)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                        case S_TrinketModifyEnum.Common:
                            if (hitCard.CardType == S_CardTypeEnum.Common)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                    }
                    break;
                case S_TrinketConditionEnum.Reverb_Two:
                    if (hitCard == null)
                    {
                        break;
                    }

                    switch (tri.Modify)
                    {
                        case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                        case S_TrinketModifyEnum.Any:
                            count = checker.GetCardsInStack().Count;
                            CheckReverbAFewTrinket(tri, count, 2);
                            break;
                        case S_TrinketModifyEnum.Str:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Str).Count;
                            CheckReverbAFewTrinket(tri, count, 2);
                            break;
                        case S_TrinketModifyEnum.Mind:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count;
                            CheckReverbAFewTrinket(tri, count, 2);
                            break;
                        case S_TrinketModifyEnum.Luck:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Luck).Count;
                            CheckReverbAFewTrinket(tri, count, 2);
                            break;
                        case S_TrinketModifyEnum.Common:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Common).Count;
                            CheckReverbAFewTrinket(tri, count, 2);
                            break;
                    }
                    break;
                case S_TrinketConditionEnum.Reverb_Three:
                    if (hitCard == null)
                    {
                        break;
                    }

                    switch (tri.Modify)
                    {
                        case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                        case S_TrinketModifyEnum.Any:
                            count = checker.GetCardsInStack().Count;
                            CheckReverbAFewTrinket(tri, count, 3);
                            break;
                        case S_TrinketModifyEnum.Str:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Str).Count;
                            CheckReverbAFewTrinket(tri, count, 3);
                            break;
                        case S_TrinketModifyEnum.Mind:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count;
                            CheckReverbAFewTrinket(tri, count, 3);
                            break;
                        case S_TrinketModifyEnum.Luck:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Luck).Count;
                            CheckReverbAFewTrinket(tri, count, 3);
                            break;
                        case S_TrinketModifyEnum.Common:
                            count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Common).Count;
                            CheckReverbAFewTrinket(tri, count, 3);
                            break;
                    }
                    break;
                case S_TrinketConditionEnum.Only:
                    switch (tri.Modify)
                    {
                        case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                        case S_TrinketModifyEnum.Str:
                            if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1)[0] == S_CardTypeEnum.Str)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                        case S_TrinketModifyEnum.Mind:
                            if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1)[0] == S_CardTypeEnum.Mind)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                        case S_TrinketModifyEnum.Luck:
                            if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1)[0] == S_CardTypeEnum.Luck)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                        case S_TrinketModifyEnum.Common:
                            if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1)[0] == S_CardTypeEnum.Common)
                            {
                                tri.IsMeetCondition = true;
                            }
                            else
                            {
                                tri.IsMeetCondition = false;
                            }
                            break;
                    }
                    break;
                case S_TrinketConditionEnum.Precision_Six:
                    count = checker.GetCardsInStack().Count;

                    if (count == 0)
                    {
                        tri.ActivatedCount = 0;
                        tri.IsMeetCondition = false;
                        break;
                    }

                    mod = count % 6;
                    tri.ActivatedCount = mod == 0 ? 6 : mod;
                    tri.IsMeetCondition = mod == 0;
                    break;
                case S_TrinketConditionEnum.Legion_Twenty:
                    count = checker.GetSumInStack();

                    tri.ActivatedCount = count;
                    if (count >= 20)
                    {
                        tri.IsMeetCondition = true;
                    }
                    else
                    {
                        tri.IsMeetCondition = false;
                    }
                    break;
                case S_TrinketConditionEnum.Resection_Three:
                    count = checker.GetCardsInStackInCurrentTurn().Count;

                    tri.ActivatedCount = count;
                    if (count <= 3)
                    {
                        tri.IsMeetCondition = true;
                    }
                    else
                    {
                        tri.IsMeetCondition = false;
                    }
                    break;
                case S_TrinketConditionEnum.Overflow_Six:
                    count = checker.GetCardsInStackInCurrentTurn().Count;

                    tri.ActivatedCount = count;
                    if (count >= 6)
                    {
                        tri.IsMeetCondition = true;
                    }
                    else
                    {
                        tri.IsMeetCondition = false;
                    }
                    break;
                case S_TrinketConditionEnum.GrandChaos_One:
                    if (checker.GetGrandChaosInStackInCurrentTurn(1).Count >= 4)
                    {
                        tri.IsMeetCondition = true;
                    }
                    else
                    {
                        tri.IsMeetCondition = false;
                    }
                    break;
                case S_TrinketConditionEnum.GrandChaos_Two:
                    if (checker.GetGrandChaosInStackInCurrentTurn(2).Count >= 4)
                    {
                        tri.IsMeetCondition = true;
                    }
                    else
                    {
                        tri.IsMeetCondition = false;
                    }
                    break;
            }
        }

        // ActivatedCount와 IsMeetCondition 체크
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();
    }
    public void CheckReverbAFewTrinket(S_Trinket tri, int count, int reverbAmount) // ~장 낼 때마다의 조건을 지닌 쓸만한 물건에 대한 보조 메서드
    {
        if (count == 0)
        {
            tri.ActivatedCount = 0;
            tri.IsMeetCondition = false;
            return;
        }

        int mod;
        mod = count % reverbAmount;
        tri.ActivatedCount = mod == 0 ? 0 : mod;
        tri.IsMeetCondition = mod == 0;

        if (tri.IsAccumulate)
        {
            tri.CurrentAccumulateValue = (count / reverbAmount) * tri.IntValue + tri.TotalTrialAccumulateValue;
        }
    }

    public async Task ActivateTrinketByStartTrial()
    {
        foreach (S_Trinket tri in S_PlayerTrinket.Instance.GetPlayerOwnedTrinkets())
        {
            if (tri.Condition == S_TrinketConditionEnum.StartTrial) // 시련 시작 시 효과를 발동한다.
            {
                await ActivateTrinket(tri);
            }
        }
    }
    public async Task ActivateTrinketByHit(S_Card hitCard) // 조건을 충족한 쓸만한 물건의 효과 발동(카드를 내거나 스탠드 할 때)
    {
        foreach (S_Trinket tri in S_PlayerTrinket.Instance.GetPlayerOwnedTrinkets())
        {
            if (tri.IsMeetCondition && (tri.Condition == S_TrinketConditionEnum.Reverb_One || tri.Condition == S_TrinketConditionEnum.Reverb_Two || tri.Condition == S_TrinketConditionEnum.Reverb_Three))
            {
                await ActivateTrinket(tri, hitCard); // 효과 발동

                // ~낼 때마다의 효과 발동을 했다면 ActivatedCount = 0, MeetCondition도 false로
                if (tri.Condition == S_TrinketConditionEnum.Reverb_One || tri.Condition == S_TrinketConditionEnum.Reverb_Two || tri.Condition == S_TrinketConditionEnum.Reverb_Three)
                {
                    tri.ActivatedCount = 0;
                    tri.IsMeetCondition = false;
                }
            }
        }

        // ActivatedCount와 IsMeetCondition 체크
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();
    }
    public void ActivateTrinketByTwist()
    {
        foreach (S_Trinket tri in S_PlayerTrinket.Instance.GetPlayerOwnedTrinkets())
        {
            if (tri.Condition == S_TrinketConditionEnum.Reverb_One || tri.Condition == S_TrinketConditionEnum.Reverb_Two || tri.Condition == S_TrinketConditionEnum.Reverb_Three)
            {
                tri.ActivatedCount = 0;
                tri.IsMeetCondition = false;
            }
        }

        // ActivatedCount와 IsMeetCondition 체크
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();
    }
    public async Task ActivateTrinketByStand()
    {
        foreach (S_Trinket tri in S_PlayerTrinket.Instance.GetPlayerOwnedTrinkets())
        {
            if (tri.IsMeetCondition && tri.Condition != S_TrinketConditionEnum.Reverb_One && tri.Condition != S_TrinketConditionEnum.Reverb_Two && tri.Condition != S_TrinketConditionEnum.Reverb_Three)
            {
                await ActivateTrinket(tri); // 효과 발동
            }
        }

        // ActivatedCount와 IsMeetCondition 체크
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();
    }
    public async Task ActivateTrinket(S_Trinket tri, S_Card hitCard = null) // 쓸만한 물건 효과 발동
    {
        int value;

        // 쓸만한 물건의 바운싱 VFX
        if (tri.Effect != S_TrinketEffectEnum.None)
        {
            S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri);
        }

        // 쓸만한 물건의 효과 발동
        switch (tri.Effect)
        {
            case S_TrinketEffectEnum.None: break;
            case S_TrinketEffectEnum.Harm:
                value = tri.IntValue <= 0 ? 1 : tri.IntValue;
                await HarmFoe(hitCard, tri.Stat, value);
                break;
            case S_TrinketEffectEnum.Harm_TwoStat_Random:
                value = tri.IntValue <= 0 ? 1 : tri.IntValue;
                if (tri.Condition == S_TrinketConditionEnum.Legion_Twenty)
                {
                    value *= checker.GetSumInStack() / 20;
                }
                await HarmFoe(hitCard, tri.Stat, value);
                break;
            case S_TrinketEffectEnum.Harm_AllStat:
                value = tri.IntValue <= 0 ? 1 : tri.IntValue;
                await HarmFoe(hitCard, S_BattleStatEnum.AllStat, value);
                break;
            case S_TrinketEffectEnum.Stat:
                await AddOrSubtractBattleStat(hitCard, tri.Stat, tri.IntValue);
                break;
            case S_TrinketEffectEnum.Stat_Multi:
                await MultiBattleStat(hitCard, tri.Stat, tri.FloatValue);
                break;
            case S_TrinketEffectEnum.Weight:
                await AddOrSubtractWeight(hitCard, tri.IntValue);
                break;
            case S_TrinketEffectEnum.Limit:
                value = tri.IntValue;
                if (tri.Condition == S_TrinketConditionEnum.Legion_Twenty)
                {
                    value *= checker.GetSumInStack() / 20;
                }
                await AddOrSubtractLimit(hitCard, value);
                break;
            case S_TrinketEffectEnum.Expansion:
                await GetExpansion(hitCard);
                break;
            case S_TrinketEffectEnum.First:
                await GetFirst(hitCard);
                break;
            case S_TrinketEffectEnum.ColdBlood:
                await GetColdBlood(hitCard);
                break;
            case S_TrinketEffectEnum.Gen:
                for (int i = 0; i < tri.IntValue; i++)
                {
                    await GenCard(hitCard);
                }
                break;
            case S_TrinketEffectEnum.Gen_Deck:
                S_Card c = checker.GetRandomCardsInDeck(1).First();
                for (int i = 0; i < tri.IntValue; i++)
                {
                    await GenCard(hitCard, c);
                }
                break;
            case S_TrinketEffectEnum.Retrigger_CurrentTurn:
                List<S_Card> cards = checker.GetCardsInStackInCurrentTurn();
                foreach (S_Card card in cards)
                {
                    await RetriggerCard(hitCard, card);
                }
                break;
            case S_TrinketEffectEnum.Health:
                await AddOrSubtractHealth(hitCard, tri.IntValue);
                break;
        }
    }
    #endregion
    #region 적 효과 발동 관련
    public void CheckFoeMeetCondition(S_Card hitCard = null) // 시련 시작 시, 카드 낼 때, 비틀기, 효과 발동 시 틈틈이. 낼 때마다 조건이 있기에 매개변수 필요
    {
        int count;
        int mod;

        S_Foe foe = S_FoeInfoSystem.Instance.FoeInfo.CurrentFoe;
        switch (foe.Condition)
        {
            case S_TrinketConditionEnum.None:
                foe.IsMeetCondition = false;
                break;
            case S_TrinketConditionEnum.StartTrial:
                foe.IsMeetCondition = false;
                break;
            case S_TrinketConditionEnum.Always:
                foe.IsMeetCondition = true;
                break;
            case S_TrinketConditionEnum.Reverb_One:
                if (hitCard == null)
                {
                    break;
                }
                switch (foe.Modify)
                {
                    case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                    case S_TrinketModifyEnum.Any: foe.IsMeetCondition = true; break;
                    case S_TrinketModifyEnum.Str:
                        if (hitCard.CardType == S_CardTypeEnum.Str)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                    case S_TrinketModifyEnum.Mind:
                        if (hitCard.CardType == S_CardTypeEnum.Mind)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                    case S_TrinketModifyEnum.Luck:
                        if (hitCard.CardType == S_CardTypeEnum.Luck)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                    case S_TrinketModifyEnum.Common:
                        if (hitCard.CardType == S_CardTypeEnum.Common)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                }
                break;
            case S_TrinketConditionEnum.Reverb_Two:
                if (hitCard == null)
                {
                    break;
                }
                switch (foe.Modify)
                {
                    case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                    case S_TrinketModifyEnum.Any:
                        count = checker.GetCardsInStack().Count;
                        CheckReverbAFewFoe(foe, count, 2);
                        break;
                    case S_TrinketModifyEnum.Str:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Str).Count;
                        CheckReverbAFewFoe(foe, count, 2);
                        break;
                    case S_TrinketModifyEnum.Mind:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count;
                        CheckReverbAFewFoe(foe, count, 2);
                        break;
                    case S_TrinketModifyEnum.Luck:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Luck).Count;
                        CheckReverbAFewFoe(foe, count, 2);
                        break;
                    case S_TrinketModifyEnum.Common:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Common).Count;
                        CheckReverbAFewFoe(foe, count, 2);
                        break;
                }
                break;
            case S_TrinketConditionEnum.Reverb_Three:
                if (hitCard == null)
                {
                    break;
                }
                switch (foe.Modify)
                {
                    case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                    case S_TrinketModifyEnum.Any:
                        count = checker.GetCardsInStack().Count;
                        CheckReverbAFewFoe(foe, count, 3);
                        break;
                    case S_TrinketModifyEnum.Str:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Str).Count;
                        CheckReverbAFewFoe(foe, count, 3);
                        break;
                    case S_TrinketModifyEnum.Mind:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Mind).Count;
                        CheckReverbAFewFoe(foe, count, 3);
                        break;
                    case S_TrinketModifyEnum.Luck:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Luck).Count;
                        CheckReverbAFewFoe(foe, count, 3);
                        break;
                    case S_TrinketModifyEnum.Common:
                        count = checker.GetSameTypeCardsInStack(S_CardTypeEnum.Common).Count;
                        CheckReverbAFewFoe(foe, count, 3);
                        break;
                }
                break;
            case S_TrinketConditionEnum.Only:
                switch (foe.Modify)
                {
                    case S_TrinketModifyEnum.None: Debug.Log("S_EffectActivator Send : Modify None Trinket Exist"); break;
                    case S_TrinketModifyEnum.Str:
                        if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1).First() == S_CardTypeEnum.Str)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                    case S_TrinketModifyEnum.Mind:
                        if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1).First() == S_CardTypeEnum.Mind)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                    case S_TrinketModifyEnum.Luck:
                        if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1).First() == S_CardTypeEnum.Luck)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                    case S_TrinketModifyEnum.Common:
                        if (checker.GetGrandChaosInStackInCurrentTurn(1).Count == 1 && checker.GetGrandChaosInStackInCurrentTurn(1).First() == S_CardTypeEnum.Common)
                        {
                            foe.IsMeetCondition = true;
                        }
                        else
                        {
                            foe.IsMeetCondition = false;
                        }
                        break;
                }
                break;
            case S_TrinketConditionEnum.Precision_Six:
                count = checker.GetCardsInStack().Count;

                if (count == 0)
                {
                    foe.ActivatedCount = 0;
                    foe.IsMeetCondition = false;
                    break;
                }

                mod = count % 6;
                foe.ActivatedCount = mod == 0 ? 6 : mod;
                foe.IsMeetCondition = mod == 0;
                break;
            case S_TrinketConditionEnum.Legion_Twenty:
                count = checker.GetSumInStack();

                foe.ActivatedCount = count;
                if (count >= 20)
                {
                    foe.IsMeetCondition = true;
                }
                else
                {
                    foe.IsMeetCondition = false;
                }
                break;
            case S_TrinketConditionEnum.Resection_Three:
                count = checker.GetCardsInStackInCurrentTurn().Count;

                foe.ActivatedCount = count;
                if (count <= 3)
                {
                    foe.IsMeetCondition = true;
                }
                else
                {
                    foe.IsMeetCondition = false;
                }
                break;
            case S_TrinketConditionEnum.Overflow_Six:
                count = checker.GetCardsInStackInCurrentTurn().Count;

                foe.ActivatedCount = count;
                if (count >= 6)
                {
                    foe.IsMeetCondition = true;
                }
                else
                {
                    foe.IsMeetCondition = false;
                }
                break;
            case S_TrinketConditionEnum.GrandChaos_One:
                if (checker.GetGrandChaosInStackInCurrentTurn(1).Count >= 4)
                {
                    foe.IsMeetCondition = true;
                }
                else
                {
                    foe.IsMeetCondition = false;
                }
                break;
            case S_TrinketConditionEnum.GrandChaos_Two:
                if (checker.GetGrandChaosInStackInCurrentTurn(2).Count >= 4)
                {
                    foe.IsMeetCondition = true;
                }
                else
                {
                    foe.IsMeetCondition = false;
                }
                break;
            case S_TrinketConditionEnum.GrandChaos_One_Flip:
                if (checker.GetGrandChaosInStackInCurrentTurn(1).Count >= 4)
                {
                    foe.IsMeetCondition = false;
                }
                else
                {
                    foe.IsMeetCondition = true;
                }
                break;
        }

        // ActivatedCount와 IsMeetCondition 체크
        S_FoeInfoSystem.Instance.UpdateFoeObject();
    }
    public void CheckReverbAFewFoe(S_Foe foe, int count, int reverbAmount) // ~장 낼 때마다의 조건을 지닌 적에 대한 보조 메서드
    {
        if (count == 0)
        {
            foe.ActivatedCount = 0;
            foe.IsMeetCondition = false;
            return;
        }

        int mod;
        mod = count % reverbAmount;
        foe.ActivatedCount = mod == 0 ? reverbAmount : mod;
        foe.IsMeetCondition = mod == 0;
    }

    public async Task ActivateFoeByStartTrial()
    {
        S_Foe foe = S_FoeInfoSystem.Instance.FoeInfo.CurrentFoe;
        if (foe.Condition == S_TrinketConditionEnum.StartTrial) // 시련 시작 시 효과를 발동한다.
        {
            await ActivateFoe(foe);
        }
    }
    public async Task ActivateFoeByHit(S_Card hitCard) // 조건을 충족한 쓸만한 물건의 효과 발동(카드를 내거나 스탠드 할 때)
    {
        S_Foe foe = S_FoeInfoSystem.Instance.FoeInfo.CurrentFoe;

        if (foe.IsMeetCondition && (foe.Condition == S_TrinketConditionEnum.Reverb_One || foe.Condition == S_TrinketConditionEnum.Reverb_Two || foe.Condition == S_TrinketConditionEnum.Reverb_Three))
        {
            await ActivateFoe(foe, hitCard); // 효과 발동

            // ~낼 때마다의 효과 발동을 했다면 ActivatedCount = 0, MeetCondition도 false로
            if (foe.Condition == S_TrinketConditionEnum.Reverb_One || foe.Condition == S_TrinketConditionEnum.Reverb_Two || foe.Condition == S_TrinketConditionEnum.Reverb_Three)
            {
                foe.ActivatedCount = 0;
                foe.IsMeetCondition = false;
            }
        }
    }
    public void ActivateFoeByTwist()
    {
        S_Foe foe = S_FoeInfoSystem.Instance.FoeInfo.CurrentFoe;

        if (foe.Condition == S_TrinketConditionEnum.Reverb_One || foe.Condition == S_TrinketConditionEnum.Reverb_Two || foe.Condition == S_TrinketConditionEnum.Reverb_Three)
        {
            foe.ActivatedCount = 0;
            foe.IsMeetCondition = false;
        }

        // ActivatedCount와 IsMeetCondition 체크
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();
    }
    public async Task ActivateFoeByStand()
    {
        S_Foe foe = S_FoeInfoSystem.Instance.FoeInfo.CurrentFoe;

        if (foe.IsMeetCondition && foe.Condition != S_TrinketConditionEnum.Reverb_One && foe.Condition != S_TrinketConditionEnum.Reverb_Two && foe.Condition != S_TrinketConditionEnum.Reverb_Three)
        {
            await ActivateFoe(foe); // 효과 발동
        }
    }
    public async Task ActivateFoe(S_Foe foe, S_Card hitCard = null) // 적 효과 발동
    {
        // 적의 바운싱 VFX
        if (foe.Effect != S_TrinketEffectEnum.None)
        {
            S_FoeInfoSystem.Instance.FoeBouncingVFX();
        }

        // 적의 효과 발동
        switch (foe.Effect)
        {
            case S_TrinketEffectEnum.None: break;
            case S_TrinketEffectEnum.Stat:
                await AddOrSubtractBattleStat(hitCard, foe.Stat, foe.IntValue);
                break;
            case S_TrinketEffectEnum.Stat_Multi:
                await MultiBattleStat(hitCard, foe.Stat, foe.FloatValue);
                break;
            case S_TrinketEffectEnum.Weight:
                await AddOrSubtractWeight(hitCard, foe.IntValue);
                break;
            case S_TrinketEffectEnum.Limit:
                await AddOrSubtractLimit(hitCard, foe.IntValue);
                break;
            case S_TrinketEffectEnum.CurseCurrentTurn:
                List<S_Card> cursedCards = checker.GetCardsInStackInCurrentTurn();
                foreach (S_Card cursedCard in cursedCards)
                {
                    await CurseCard(cursedCard, hitCard);
                }
                break;
            case S_TrinketEffectEnum.Delusion:
                await GetDelusion(hitCard);
                break;

        }
    }
    #endregion
    #region 기본적인 베이스 효과.(효과의 발동 순서 : 실제 계산 -> 카드 바운스 -> 로그 생성 -> 플레이어 쪽 VFX)
    // 기본 능력치 효과
    async Task AddOrSubtractWeight(S_Card triggerCard, int value)
    {
        // value를 무게에 더하기
        pStat.AddOrSubtractWeight(value);

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"무게 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Weight, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"무게 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Weight, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }

        // 버스트와 완벽 추가 체크
        pStat.CheckBurstAndPerfect();

        // 버스트 및 완벽 확인
        if (pStat.IsBurst)
        {
            GenerateEffectLog($"버스트!");
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Burst, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
        else if (pStat.IsPerfect)
        {
            GenerateEffectLog($"완벽!");
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Perfect, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

            // 쓸만한 물건 패시브 체크
            if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Perfect15Mind, out S_Trinket tri1))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri1);

                await AddOrSubtractBattleStat(triggerCard, S_BattleStatEnum.Mind, 15);
            }
            if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Perfect8StrLuck, out S_Trinket tri2))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri2);

                await AddOrSubtractBattleStat(triggerCard, S_BattleStatEnum.Str_Mind, 8);
            }
        }
    }
    async Task AddOrSubtractLimit(S_Card triggerCard, int value)
    {
        // value를 한계에 더하기
        pStat.AddOrSubtractLimit(value);

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"한계 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Limit, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"한계 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Limit, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }

        // 버스트와 클린히트 추가 체크
        pStat.CheckBurstAndPerfect();

        // 버스트 및 클린히트 확인
        if (pStat.IsBurst)
        {
            // 로그 생성
            GenerateEffectLog($"버스트!");
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Burst, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
        else if (pStat.IsPerfect)
        {
            GenerateEffectLog($"완벽!");
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Perfect, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

            // 쓸만한 물건 패시브 체크
            if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Perfect15Mind, out S_Trinket tri1))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri1);

                await AddOrSubtractBattleStat(triggerCard, S_BattleStatEnum.Mind, 15);
            }
            if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Perfect8StrLuck, out S_Trinket tri2))
            {
                S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri2);

                await AddOrSubtractBattleStat(triggerCard, S_BattleStatEnum.Str_Mind, 8);
            }
        }
    }
    async Task AddOrSubtractHealth(S_Card triggerCard, int value)
    {
        // 체력 추가
        pStat.AddOrSubtractHealth(value);

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"체력 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Health, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"체력 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Health, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
    }
    async Task AddOrSubtractDetermination(S_Card triggerCard, int value)
    {
        // 의지 추가
        pStat.AddOrSubtractDetermination(value);

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"의지 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Determination, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"의지 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Determination, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
    }
    async Task AddOrSubtractGold(S_Card triggerCard, int value)
    {
        // 골드 추가
        pStat.AddOrSubtractGold(value);

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 양수일 때
        if (value > 0)
        {
            // 로그 생성
            GenerateEffectLog($"골드 +{value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Gold, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
        else
        {
            // 로그 생성
            GenerateEffectLog($"골드 {value}");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Gold, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
        }
    }

    // 전투 능력 효과
    public async Task AddOrSubtractBattleStat(S_Card triggerCard, S_BattleStatEnum stat, int value)
    {
        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        if (value > 0) // 능력치 증가
        {
            switch (stat)
            {
                case S_BattleStatEnum.Str:
                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoStrMindLuck1Trig, out S_Trinket tri1))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri1);
                        break;
                    }
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Mind:
                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoMindStrLuck1Trig, out S_Trinket tri5))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri5);
                        break;
                    }
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Luck:
                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoLuckStrMind1Trig, out S_Trinket tri8))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri8);
                        break;
                    }
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Str_Mind:
                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoStrMindLuck1Trig, out S_Trinket tri2))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri2);
                        break;
                    }
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoMindStrLuck1Trig, out S_Trinket tri6))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri6);
                        break;
                    }
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Str_Luck:
                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoStrMindLuck1Trig, out S_Trinket tri3))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri3);
                        break;
                    }
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoLuckStrMind1Trig, out S_Trinket tri9))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri9);
                        break;
                    }
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Mind_Luck:
                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoMindStrLuck1Trig, out S_Trinket tri7))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri7);
                        break;
                    }
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoLuckStrMind1Trig, out S_Trinket tri10))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri10);
                        break;
                    }
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.AllStat:
                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoStrMindLuck1Trig, out S_Trinket tri4))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri4);
                        break;
                    }
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoMindStrLuck1Trig, out S_Trinket tri12))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri12);
                        break;
                    }
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                    if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoLuckStrMind1Trig, out S_Trinket tri11))
                    {
                        S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri11);
                        break;
                    }
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
            }
        }
        else // 능력치 감소
        {
            switch (stat)
            {
                case S_BattleStatEnum.Str:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Mind:
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Luck:
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.AllStat:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    break;
                case S_BattleStatEnum.Random:
                    List<S_BattleStatEnum> stats = new() { S_BattleStatEnum.Str, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                    S_BattleStatEnum s = stats[Random.Range(0, stats.Count)];
                    if (s == S_BattleStatEnum.Str)
                    {
                        pStat.AddStrength(value);
                        GenerateEffectLog($"힘 {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    }
                    else if (s == S_BattleStatEnum.Mind)
                    {
                        pStat.AddMind(value);
                        GenerateEffectLog($"정신력 {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    }
                    else if (s == S_BattleStatEnum.Luck)
                    {
                        pStat.AddLuck(value);
                        GenerateEffectLog($"행운 {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                    }
                    break;
            }
        }
    }
    async Task MultiBattleStat(S_Card triggerCard, S_BattleStatEnum stat, float multi)
    {
        int value;

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        switch (stat)
        {
            case S_BattleStatEnum.Str:
                value = Mathf.RoundToInt(pStat.CurrentStr * (multi - 1));
                pStat.AddStrength(value);
                GenerateEffectLog($"힘 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                break;
            case S_BattleStatEnum.Mind:
                value = Mathf.RoundToInt(pStat.CurrentMind * (multi - 1));
                pStat.AddMind(value);
                GenerateEffectLog($"정신력 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                break;
            case S_BattleStatEnum.Luck:
                value = Mathf.RoundToInt(pStat.CurrentLuck * (multi - 1));
                pStat.AddLuck(value);
                GenerateEffectLog($"행운 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                break;
            case S_BattleStatEnum.Str_Mind:
                value = Mathf.RoundToInt(pStat.CurrentStr * (multi - 1));
                pStat.AddStrength(value);
                GenerateEffectLog($"힘 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                value = Mathf.RoundToInt(pStat.CurrentMind * (multi - 1));
                pStat.AddMind(value);
                GenerateEffectLog($"정신력 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                break;
            case S_BattleStatEnum.Str_Luck:
                value = Mathf.RoundToInt(pStat.CurrentStr * (multi - 1));
                pStat.AddStrength(value);
                GenerateEffectLog($"힘 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                value = Mathf.RoundToInt(pStat.CurrentLuck * (multi - 1));
                pStat.AddLuck(value);
                GenerateEffectLog($"행운 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                break;
            case S_BattleStatEnum.Mind_Luck:
                value = Mathf.RoundToInt(pStat.CurrentMind * (multi - 1));
                pStat.AddMind(value);
                GenerateEffectLog($"정신력 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                value = Mathf.RoundToInt(pStat.CurrentLuck * (multi - 1));
                pStat.AddLuck(value);
                GenerateEffectLog($"행운 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                break;
            case S_BattleStatEnum.AllStat:
                value = Mathf.RoundToInt(pStat.CurrentStr * (multi - 1));
                pStat.AddStrength(value);
                GenerateEffectLog($"힘 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                value = Mathf.RoundToInt(pStat.CurrentMind * (multi - 1));
                pStat.AddMind(value);
                GenerateEffectLog($"정신력 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

                value = Mathf.RoundToInt(pStat.CurrentLuck * (multi - 1));
                pStat.AddLuck(value);
                GenerateEffectLog($"행운 +{value}");
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                break;
            case S_BattleStatEnum.Random:
                List<S_BattleStatEnum> stats = new() { S_BattleStatEnum.Str, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                S_BattleStatEnum s = stats[Random.Range(0, stats.Count)];
                if (s == S_BattleStatEnum.Str)
                {
                    value = Mathf.RoundToInt(pStat.CurrentStr * (multi - 1));
                    pStat.AddStrength(value);
                    GenerateEffectLog($"힘 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                }
                else if (s == S_BattleStatEnum.Mind)
                {
                    value = Mathf.RoundToInt(pStat.CurrentMind * (multi - 1));
                    pStat.AddMind(value);
                    GenerateEffectLog($"정신력 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                }
                else if (s == S_BattleStatEnum.Luck)
                {
                    value = Mathf.RoundToInt(pStat.CurrentLuck * (multi - 1));
                    pStat.AddLuck(value);
                    GenerateEffectLog($"행운 +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
                }
                break;
        }
    }
    async Task HarmFoe(S_Card triggerCard, S_BattleStatEnum stat, int multi = 1)
    {
        // 피해량 정하기
        int value = 0;
        switch (stat)
        {
            case S_BattleStatEnum.Str: value = pStat.CurrentStr; break;
            case S_BattleStatEnum.Mind: value = pStat.CurrentMind; break;
            case S_BattleStatEnum.Luck: value = pStat.CurrentLuck; break;
            case S_BattleStatEnum.Str_Mind: value = pStat.CurrentStr * pStat.CurrentMind; break;
            case S_BattleStatEnum.Str_Luck: value = pStat.CurrentStr + pStat.CurrentLuck; break;
            case S_BattleStatEnum.Mind_Luck: value = pStat.CurrentMind * pStat.CurrentLuck; break;
            case S_BattleStatEnum.AllStat: value = pStat.CurrentStr * pStat.CurrentMind * pStat.CurrentLuck; break;
            default: Debug.Log("S_EffectActivator Send : Error Exist In HarmFoe Method"); break;
        }
        value *= multi;
        // 쓸만한 물건 패시브 효과
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Multi20Harm, out S_Trinket tri1))
        {
            S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri1);

            value *= 20;
        }
        bool noBP = pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoBurstPerfect, out S_Trinket tri2);

        // 버스트와 완벽에 따른 2차 피해량 정하기
        if (pStat.IsBurst && !noBP)
        {
            value = Mathf.RoundToInt(value * 0.25f);
        }
        else if (pStat.IsPerfect && !noBP)
        {
            value = Mathf.RoundToInt(value * 2f);
        }

        // 적에게 피해주기
        S_FoeInfoSystem.Instance.DamagedByHarm(value);

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        if (pStat.IsBurst && !noBP)
        {
            GenerateEffectLog($"{value}의 감소된 피해(버스트)");
        }
        else if (pStat.IsPerfect && !noBP)
        {
            GenerateEffectLog($"{value}의 증가된 피해(완벽)");
        }
        else
        {
            GenerateEffectLog($"{value}의 피해");
        }

        // 데미지에 따라 카메라 쉐이킹(하스스톤)
        if (value >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth)
        {
            ShakeCamera(1.2f);
        }
        else if (value >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.5f)
        {
            ShakeCamera(0.9f);
        }
        else if (value >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.3f)
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
            case S_BattleStatEnum.Str: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength); break;
            case S_BattleStatEnum.Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind); break;
            case S_BattleStatEnum.Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Luck); break;
            case S_BattleStatEnum.Str_Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Mind); break;
            case S_BattleStatEnum.Str_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Luck); break;
            case S_BattleStatEnum.Mind_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind_Luck); break;
            case S_BattleStatEnum.AllStat: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Carnage); break;
        }
    }
    async Task HarmFoeByFloat(S_Card triggerCard, S_BattleStatEnum stat, float multi)
    {
        // 피해량 정하기
        int value = 0;
        switch (stat)
        {
            case S_BattleStatEnum.Str: value = pStat.CurrentStr; break;
            case S_BattleStatEnum.Mind: value = pStat.CurrentMind; break;
            case S_BattleStatEnum.Luck: value = pStat.CurrentLuck; break;
            case S_BattleStatEnum.Str_Mind: value = pStat.CurrentStr * pStat.CurrentMind; break;
            case S_BattleStatEnum.Str_Luck: value = pStat.CurrentStr + pStat.CurrentLuck; break;
            case S_BattleStatEnum.Mind_Luck: value = pStat.CurrentMind * pStat.CurrentLuck; break;
            case S_BattleStatEnum.AllStat: value = pStat.CurrentStr * pStat.CurrentMind * pStat.CurrentLuck; break;
            default: Debug.Log("S_EffectActivator Send : Error Exist In HarmFoe Method"); break;
        }
        value = Mathf.RoundToInt(value * multi);
        // 쓸만한 물건 패시브 효과
        if (pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.Multi20Harm, out S_Trinket tri1))
        {
            S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri1);

            value *= 20;
        }
        bool noBP = pTrinket.IsPlayerHavePassive(S_TrinketPassiveEnum.NoBurstPerfect, out S_Trinket tri2);

        // 버스트와 완벽에 따른 2차 피해량 정하기
        if (pStat.IsBurst && !noBP)
        {
            value = Mathf.RoundToInt(value * 0.25f);
        }
        else if (pStat.IsPerfect && !noBP)
        {
            value = Mathf.RoundToInt(value * 2f);
        }

        // 적 패시브 효과
        if (S_FoeInfoSystem.Instance.IsFoeHavePassive(S_TrinketPassiveEnum.Ignore25PerHarm))
        {
            if (value >= Mathf.RoundToInt(S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.25f))
            {
                value = Mathf.RoundToInt(S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.25f);
            }
        }
        if (S_FoeInfoSystem.Instance.IsFoeHavePassive(S_TrinketPassiveEnum.Immunity30PerHarm))
        {
            if (value < Mathf.RoundToInt(S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.3f))
            {
                value = 0;
            }
        }

        // 적에게 피해주기
        S_FoeInfoSystem.Instance.DamagedByHarm(value);

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        if (pStat.IsBurst && !noBP)
        {
            GenerateEffectLog($"{value}의 감소된 피해(버스트)");
        }
        else if (pStat.IsPerfect && !noBP)
        {
            GenerateEffectLog($"{value}의 증가된 피해(완벽)");
        }
        else
        {
            GenerateEffectLog($"{value}의 피해");
        }

        // 데미지에 따라 카메라 쉐이킹(하스스톤)
        if (value >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth)
        {
            ShakeCamera(1.2f);
        }
        else if (value >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.5f)
        {
            ShakeCamera(0.9f);
        }
        else if (value >= S_FoeInfoSystem.Instance.FoeInfo.MaxHealth * 0.3f)
        {
            ShakeCamera(0.6f);
        }
        else
        {
            ShakeCamera(0.3f);
        }

        // 피해 VFX
        switch (stat)
        {
            case S_BattleStatEnum.Str: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength); break;
            case S_BattleStatEnum.Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind); break;
            case S_BattleStatEnum.Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Luck); break;
            case S_BattleStatEnum.Str_Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Mind); break;
            case S_BattleStatEnum.Str_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Luck); break;
            case S_BattleStatEnum.Mind_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind_Luck); break;
            case S_BattleStatEnum.AllStat: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Carnage); break;
        }
    }
    async Task GenCard(S_Card triggerCard, S_Card targetCard = null, S_CardTypeEnum type = default)
    {
        // 카드 생성
        S_Card genCard;
        if (targetCard != null)
        {
            genCard = S_CardManager.Instance.CopyCard(targetCard);
        }
        else
        {
            genCard = S_CardManager.Instance.GenerateRandomCard(type);
        }
        genCard.IsGenerated = true;

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        GenerateEffectLog($"카드 생성함!");

        // 플레이어 이미지 VFX
        _ = S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Creation, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

        // 카드 내기
        await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(genCard, S_CardOrderTypeEnum.Gen);
    }
    async Task RetriggerCard(S_Card triggerCard, S_Card targetCard)
    {
        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        GenerateEffectLog($"카드 효과 1번 더 발동!");

        // 플레이어 이미지 VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Undertow, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));

        // 효과 발동
        await OnlyActivateCardEffect(targetCard);
    }

    // 특수상태 류
    async Task GetExpansion(S_Card triggerCard)
    {
        // 전개 획득
        pStat.IsExpansion = true;

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        GenerateEffectLog($"전개 획득!");

        // 효과 및 대기
        S_StatInfoSystem.Instance.UpdateSpecialAbility();
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Expansion, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
    }
    async Task GetFirst(S_Card triggerCard)
    {
        // 우선 획득
        pStat.IsFirst = true;

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        GenerateEffectLog($"우선 획득!");

        // 효과 및 대기
        S_StatInfoSystem.Instance.UpdateSpecialAbility();
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.First, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
    }
    async Task GetColdBlood(S_Card triggerCard)
    {
        // 전개 획득
        pStat.IsColdBlood = true;

        // 바운싱 카드
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        // 로그 생성
        GenerateEffectLog($"냉혈 획득!");

        // 효과 및 대기
        S_StatInfoSystem.Instance.UpdateSpecialAbility();
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.ColdBlood, S_StackInfoSystem.Instance.GetStackCardObj(triggerCard));
    }
    async Task GetDelusion(S_Card triggerCards)
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
        S_StatInfoSystem.Instance.UpdateSpecialAbility();
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Delusion, S_StackInfoSystem.Instance.GetStackCardObj(triggerCards));
    }

    // 디버프 류
    async Task CurseCard(S_Card cursedCard, S_Card triggerCard) // 카드 저주
    {
        // 면역 체크
        if (cursedCard.Engraving == S_EngravingEnum.Immunity)
        {
            cursedCard.IsCursed = false;
        }
        else
        {
            cursedCard.IsCursed = true;
        }

        // 카드의 저주 이펙트 켜기
        S_StackInfoSystem.Instance.UpdateStackCardState();

        // 저주 대상 및 트리거 카드 바운싱 VFX
        S_StackInfoSystem.Instance.BouncingStackCard(cursedCard);
        if (triggerCard != null)
        {
            S_StackInfoSystem.Instance.BouncingStackCard(triggerCard);
        }

        if (cursedCard.IsCursed) // 저주한 카드 로그 생성
        {
            // 로그 생성
            GenerateEffectLog("저주받음!");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Cursed);
        }
        else // 면역 카드에 대한 로그 생성
        {
            // 로그 생성
            GenerateEffectLog("저주 저항함!!(면역 각인)");

            // 플레이어 이미지 VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.ResistanceCurse);
        }

        // 깨진 그릇 있는지 체크(쓸만한 물건 패시브)
        if (S_PlayerTrinket.Instance.IsPlayerHavePassive(S_TrinketPassiveEnum.CurseGet10Str, out S_Trinket tri))
        {
            S_TrinketInfoSystem.Instance.BouncingTrinketObjVFX(tri);

            await AddOrSubtractBattleStat(null, S_BattleStatEnum.Str, 10);
        }
    }
    async Task CurseRandomCards(int count, S_CardTypeEnum type, bool inDeck, bool inStack, S_Card triggerCard) // 조건이 있으면 조건대로 랜덤 저주, 아니면 그냥 랜덤 저주
    {
        List<S_Card> cursedCards = new();

        if (inDeck) // 덱에서 카드 찾기
        {
            cursedCards.AddRange(checker.GetRandomCardsInDeck(count, type));
        }

        if (inStack) // 스택에서 카드 찾기
        {
            cursedCards.AddRange(checker.GetRandomCardsInStack(count, type));
        }

        // 저주할 카드가 있다면 저주 진행
        if (cursedCards.Count > 0)
        {
            foreach (S_Card cursedCard in cursedCards)
            {
                await CurseCard(cursedCard, triggerCard);
            }
        }
    }
    #endregion
    #region VFX 보조
    public float GetEffectLifeTime()
    {
        return EFFECT_LIFE_TIME * EFFECT_SPEED;
    }
    public async Task WaitEffectLifeTimeAsync()
    {
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
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


//async Task ExclusionRandomCards(S_Card triggerCards, int count, S_CardTypeEnum type = S_CardTypeEnum.None) // 무작위 카드 제외
//{
//    List<S_Card> exclusionCards = S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(count, type);

//    if (exclusionCards.Count > 0)
//    {
//        foreach (S_Card exclusionCard in exclusionCards)
//        {
//            // 카드 제외하기
//            _ = S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(exclusionCard, S_CardOrderTypeEnum.Exclusion);

//            // 바운싱 카드
//            if (triggerCards != null)
//            {
//                S_StackInfoSystem.Instance.BouncingStackCard(triggerCards);
//            }

//            // 로그 생성
//            GenerateEffectLog($"카드 제외함!");

//            // 플레이어 이미지 VFX
//            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Exclusion, S_StackInfoSystem.Instance.GetStackCardObj(triggerCards));
//        }
//    }
//}