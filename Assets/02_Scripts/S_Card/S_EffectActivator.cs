using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_EffectActivator : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject prefab_EffectLog;

    [Header("������Ʈ")]
    S_PlayerCard pCard;
    S_PlayerStat pStat;
    S_PlayerSkill pSkill;

    [Header("�ִ� ����")]
    float EFFECT_SPEED = 1;
    const float HIT_AND_SORT_STACK_TIME = 0.3f;
    const float EFFECT_LIFE_TIME = 0.5f;
    const float EFFECT_LOG_LIFE_TIME = 0.8f; // �α״� ȿ������ ���� �� ���� ��Ƴ��´�.
    int logStartPosX = 80;
    int logStartPosY = 40;
    int logStartPosYOffset = 300;
    int logMoveAmount = 30;

    // �̱���
    static S_EffectActivator instance;
    public static S_EffectActivator Instance { get { return instance; } }

    void Awake()
    {
        // ������Ʈ �Ҵ�
        pCard = S_PlayerCard.Instance;
        pStat = S_PlayerStat.Instance;
        pSkill = S_PlayerSkill.Instance;

        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region �ֿ� ȿ�� �ߵ�
    public async Task ActivateHitCard(S_Card hitCard, S_CardOrderTypeEnum type) // ��Ʈ �� ����, �޾Ƹ� �ߵ�
    {
        List<S_Card> hittedCards = new List<S_Card> { hitCard };

        // ī�� ���ڿ� ���� ���� �հ� �ɷ�ġ ���
        if (hitCard.AdditiveEffect == S_CardAdditiveEffectEnum.ColdBlood) // ������ ��� ����.
        {
            // �ٿ�� ī��
            if (hittedCards != null && hittedCards.Count > 0)
            {
                S_StackInfoSystem.Instance.BouncingStackCards(hittedCards);
            }

            // �α� ����
            GenerateEffectLog("���� ī��!");
        }
        else if (type == S_CardOrderTypeEnum.IllusionHit) // ȯ�� ī�忩�� ����.
        {
            // �ٿ�� ī��
            if (hittedCards != null && hittedCards.Count > 0)
            {
                S_StackInfoSystem.Instance.BouncingStackCards(hittedCards);
            }

            // �α� ����
            GenerateEffectLog("ȯ�� ī��!");
        }
        else if (type == S_CardOrderTypeEnum.BasicHit)// ������ ȯ�� �ƴ� �׳� ��Ʈ��� ���� �� ���
        {
            await AddOrSubtractStackSum(hittedCards, hitCard.Number);
            await AddOrSubtractBattleStats(hittedCards, hitCard.StatValue, hitCard.Number);
        }

        // ��Ʈ ī�尡 ���� ��
        if (!hitCard.IsCursed && hitCard.BasicCondition == S_CardBasicConditionEnum.Unleash && hitCard.CanActivateEffect)
        {
            // ȿ�� �ߵ�
            await ActivateBasicEffect(hitCard);
            await ActivateDebuff(hitCard);
            await ActivateAdditiveEffect(hitCard);

            hitCard.CanActivateEffect = false;
            S_StackInfoSystem.Instance.UpdateStackCardsState();
        }
        else if (hitCard.IsCursed && hitCard.BasicCondition == S_CardBasicConditionEnum.Unleash && hitCard.CanActivateEffect) // �����ε� �߰� ���ǵ� �����ߴµ� ���ֹ޾Ҵٸ� ���ֹ��� ī��!��� ǥ�����ֱ�
        {
            await CursedCardActivateEffect(hittedCards);
        }

        // ��Ʈ ī�忡 ���� �޾Ƹ� �ߵ�
        foreach (S_Card targetCard in pCard.GetPreStackCards())
        {
            if (!targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Reverb && targetCard.CanActivateEffect)
            {
                // ȿ�� �ߵ�
                await ActivateBasicEffect(targetCard, hitCard);
                await ActivateDebuff(targetCard, hitCard);
                await ActivateAdditiveEffect(targetCard, hitCard);

                targetCard.CanActivateEffect = IsMeetAdditiveCondition(targetCard);
                S_StackInfoSystem.Instance.UpdateStackCardsState();
            }
            else if (targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Reverb && targetCard.CanActivateEffect)
            {
                await CursedCardActivateEffect(new List<S_Card> { targetCard });
            }
        }

        // ī����� �ʷϺ� üũ(����, �Ϻ� �޾Ƹ����� ����)
        foreach (S_Card targetCard in pCard.GetPreStackCards())
        {
            targetCard.CanActivateEffect = IsMeetAdditiveCondition(targetCard);

            if (targetCard.BasicCondition == S_CardBasicConditionEnum.Unleash)
            {
                targetCard.CanActivateEffect = false;
            }
        }
        S_StackInfoSystem.Instance.UpdateStackCardsState();

        // ī�忡 ���� �����丮 ����
        pStat.SaveStatHistory(hitCard, S_StatHistoryTriggerEnum.Card);
    }
    public async Task ActivatedResolveCard() // ���ĵ� �� ���� �ߵ�
    {
        // ���� �ߵ�
        foreach (S_Card targetCard in pCard.GetPreStackCards())
        {
            if (!targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Resolve && targetCard.CanActivateEffect)
            {
                // ȿ�� �ߵ�
                await ActivateBasicEffect(targetCard);
                await ActivateDebuff(targetCard);
                await ActivateAdditiveEffect(targetCard);
            }
            else if (targetCard.IsCursed && targetCard.BasicCondition == S_CardBasicConditionEnum.Resolve && targetCard.CanActivateEffect)
            {
                await CursedCardActivateEffect(new List<S_Card> { targetCard });
            }
        }
    }
    public async Task ApplyDelusionAsync(S_Card card) // ������ �ִٸ� ������ �Ŵ� �޼���
    {
        if (pStat.IsDelusion)
        {
            // ���� �ʱ�ȭ
            pStat.IsDelusion = false;

            // ���� ó��
            await CurseCard(card, null);

            // ���� �����ϴ� ȿ��
            GenerateEffectLog("���� ������!");
            S_StatInfoSystem.Instance.ChangeSpecialAbility();
            S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();
            await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
        }
    }
    public async Task AppliedFirstAsync() // �켱 ��� �� ȣ��
    {
        pStat.IsFirst = S_FirstEffectEnum.None;

        // �켱 �����ϴ� ȿ��
        GenerateEffectLog("�켱 ����!");
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    public async Task AppliedExpansionAsync() // ���� ��� �� ȣ��
    {
        pStat.IsExpansion = false;

        // ���� �����ϴ� ȿ��
        GenerateEffectLog("���� ����!");
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    async Task ActivateBasicEffect(S_Card card, S_Card reverbedCard = null) // �⺻ ȿ�� �ߵ�
    {
        List<S_Card> hittedCard = new List<S_Card> { card };

        if (card.IsCursed) 
        {
            if (IsMeetAdditiveCondition(card))
            {
                await CursedCardActivateEffect(hittedCard);
            }
            return;
        }

        if (reverbedCard != null) hittedCard.Add(reverbedCard);

        switch (card.BasicEffect)
        {
            case S_CardBasicEffectEnum.None: break;
            case S_CardBasicEffectEnum.Increase_Strength: await AddOrSubtractBattleStats(hittedCard, S_BattleStatEnum.Strength, 3); break;
            case S_CardBasicEffectEnum.Increase_Mind: await AddOrSubtractBattleStats(hittedCard, S_BattleStatEnum.Mind, 3); break;
            case S_CardBasicEffectEnum.Increase_Luck: await AddOrSubtractBattleStats(hittedCard, S_BattleStatEnum.Luck, 3); break;
            case S_CardBasicEffectEnum.Increase_AllStat: await AddOrSubtractBattleStats(hittedCard, S_BattleStatEnum.AllStat, 2); break;
            case S_CardBasicEffectEnum.Break_Zenith:
                S_BattleStatEnum highestStat = S_EffectChecker.Instance.GetHighestStats(out int val);
                int amount = (int)System.Math.Round(val * 0.5f, System.MidpointRounding.AwayFromZero);
                await AddOrSubtractBattleStats(hittedCard, highestStat, amount);
                break;
            case S_CardBasicEffectEnum.Break_Genesis:
                List<S_BattleStatEnum> randomStats = new List<S_BattleStatEnum>() { S_BattleStatEnum.Strength, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                S_BattleStatEnum randomStat = randomStats[Random.Range(0, randomStats.Count)];
                int amount2 = 0;
                if (randomStat == S_BattleStatEnum.Strength) amount2 = pStat.CurrentStrength;
                else if (randomStat == S_BattleStatEnum.Mind) amount2 = pStat.CurrentMind;
                else if (randomStat == S_BattleStatEnum.Luck) amount2 = pStat.CurrentLuck;
                await AddOrSubtractBattleStats(hittedCard, randomStat, amount2);
                break;
            case S_CardBasicEffectEnum.Manipulation: await AddOrSubtractStackSum(hittedCard, -2); break;
            case S_CardBasicEffectEnum.Manipulation_Cheat: await AddOrSubtractStackSum(hittedCard, -card.Number); break;
            case S_CardBasicEffectEnum.Manipulation_Judge:
                int judgeValue = pStat.CurrentLimit - pStat.StackSum;
                await AddOrSubtractStackSum(hittedCard, judgeValue);
                break;
            case S_CardBasicEffectEnum.Resistance: await AddOrSubtractLimit(hittedCard, 1); break;
            case S_CardBasicEffectEnum.Resistance_Indomitable: await AddOrSubtractLimit(hittedCard, card.Number); break;
            case S_CardBasicEffectEnum.Harm_Strength: await HarmCreature(hittedCard, S_BattleStatEnum.Strength, pStat.CurrentStrength); break;
            case S_CardBasicEffectEnum.Harm_Mind: await HarmCreature(hittedCard, S_BattleStatEnum.Mind, pStat.CurrentMind); break;
            case S_CardBasicEffectEnum.Harm_Luck: await HarmCreature(hittedCard, S_BattleStatEnum.Luck, pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Harm_StrengthAndMind: await HarmCreature(hittedCard, S_BattleStatEnum.Strength_Mind, pStat.CurrentStrength * pStat.CurrentMind); break;
            case S_CardBasicEffectEnum.Harm_StrengthAndLuck: await HarmCreature(hittedCard, S_BattleStatEnum.Strength_Luck, pStat.CurrentStrength * pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Harm_MindAndLuck: await HarmCreature(hittedCard, S_BattleStatEnum.Mind_Luck, pStat.CurrentMind * pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Harm_Carnage: await HarmCreature(hittedCard, S_BattleStatEnum.AllStat, pStat.CurrentStrength * pStat.CurrentMind * pStat.CurrentLuck); break;
            case S_CardBasicEffectEnum.Tempering: await AddOrSubtractDetermination(hittedCard, 1); break;
            case S_CardBasicEffectEnum.Plunder: await AddOrSubtractGold(hittedCard, 2); break;
            case S_CardBasicEffectEnum.Plunder_Raid: await AddOrSubtractGold(hittedCard, pStat.CurrentGold); break;
            case S_CardBasicEffectEnum.Creation:
                await CreationCard(hittedCard);
                break;
            case S_CardBasicEffectEnum.Creation_SameSuit:
                await CreationCard(hittedCard, -1, card.Suit);
                break;
            case S_CardBasicEffectEnum.Creation_SameNumber:
                await CreationCard(hittedCard, card.Number);
                break;
            case S_CardBasicEffectEnum.Creation_PlethoraNumber:
                int[] ints = new int[] { 8, 9, 10 };
                int plethoraNum = ints[Random.Range(0, ints.Length)];
                await CreationCard(hittedCard, plethoraNum);
                break;
            case S_CardBasicEffectEnum.AreaExpansion: await GetExpansion(hittedCard); break;
            case S_CardBasicEffectEnum.First_SameSuit: 
                switch (card.Suit)
                {
                    case S_CardSuitEnum.Spade: await GetFirst(hittedCard, S_FirstEffectEnum.Spade); break;
                    case S_CardSuitEnum.Heart: await GetFirst(hittedCard, S_FirstEffectEnum.Heart); break;
                    case S_CardSuitEnum.Diamond: await GetFirst(hittedCard, S_FirstEffectEnum.Diamond); break;
                    case S_CardSuitEnum.Clover: await GetFirst(hittedCard, S_FirstEffectEnum.Clover); break;
                }
                break;
            case S_CardBasicEffectEnum.First_LeastSuit: await GetFirst(hittedCard, S_FirstEffectEnum.LeastSuit); break;
            case S_CardBasicEffectEnum.First_SameNumber:
                switch (card.Number)
                {
                    case 1: await GetFirst(hittedCard, S_FirstEffectEnum.One); break;
                    case 2: await GetFirst(hittedCard, S_FirstEffectEnum.Two); break;
                    case 3: await GetFirst(hittedCard, S_FirstEffectEnum.Three); break;
                    case 4: await GetFirst(hittedCard, S_FirstEffectEnum.Four); break;
                    case 5: await GetFirst(hittedCard, S_FirstEffectEnum.Five); break;
                    case 6: await GetFirst(hittedCard, S_FirstEffectEnum.Six); break;
                    case 7: await GetFirst(hittedCard, S_FirstEffectEnum.Seven); break;
                    case 8: await GetFirst(hittedCard, S_FirstEffectEnum.Eight); break;
                    case 9: await GetFirst(hittedCard, S_FirstEffectEnum.Nine); break;
                    case 10: await GetFirst(hittedCard, S_FirstEffectEnum.Ten); break;
                }
                break;
            case S_CardBasicEffectEnum.First_CleanHitNumber: await GetFirst(hittedCard, S_FirstEffectEnum.CleanHitNumber); break;
            case S_CardBasicEffectEnum.Undertow: await ActivateUndertow(hittedCard); break;
            case S_CardBasicEffectEnum.Guidance_LeastSuit:
                S_EffectChecker.Instance.GetLeastSuitCardsInDeck(out S_CardSuitEnum leastSuit);
                List<S_Card> lsCards = S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(999, leastSuit, -1);
                if (lsCards.Count > 0)
                {
                    await GuidanceCard(hittedCard, lsCards);
                }
                break;
            case S_CardBasicEffectEnum.Guidance_LeastNumber:
                S_EffectChecker.Instance.GetLeastNumberCardsInDeck(out int leastNum);
                List<S_Card> lnCards = S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(999, S_CardSuitEnum.None, leastNum);
                if (lnCards.Count > 0)
                {
                    await GuidanceCard(hittedCard, lnCards);
                }
                break;
            default: break;
        }
    }
    async Task ActivateDebuff(S_Card card, S_Card reverbedCard = null) // ����� �ߵ�
    {
        List<S_Card> hittedCard = new List<S_Card> { card };

        if (card.IsCursed)
        {
            if (IsMeetAdditiveCondition(card))
            {
                await CursedCardActivateEffect(hittedCard);
            }
            return;
        }

        if (reverbedCard != null) hittedCard.Add(reverbedCard);

        switch (card.DebuffCondition)
        {
            case S_CardDebuffConditionEnum.None: break;
            case S_CardDebuffConditionEnum.Breakdown: await ExclusionRandomCards(1, S_CardSuitEnum.None, -1, hittedCard); break;
            case S_CardDebuffConditionEnum.Paranoia: await GetDelusion(hittedCard); break;
            case S_CardDebuffConditionEnum.Spell: 
                await CurseRandomCards(1, S_CardSuitEnum.None, -1, true, false, hittedCard);
                await CurseRandomCards(1, S_CardSuitEnum.None, -1, false, true, hittedCard);
                break;
            case S_CardDebuffConditionEnum.Rebel: await AddOrSubtractLimit(hittedCard, -1); break;
            default: break;
        }
    }
    async Task ActivateAdditiveEffect(S_Card card, S_Card reverbedCard = null) // �߰� ȿ�� �ߵ�
    {
        List<S_Card> hittedCard = new List<S_Card> { card };

        if (card.IsCursed)
        {
            if (IsMeetAdditiveCondition(card))
            {
                await CursedCardActivateEffect(hittedCard);
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
                if (pStat.GetCurrentHealth() == 1) // ü���� 1�� �� �ߵ�
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
    #region �߰� ���� ���
    public bool IsMeetAdditiveCondition(S_Card targetCard, S_Card hitCard = null) // �߰� ���� ���
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
    #region ī�� �⺻ ȿ��(�Ѱ�, ������, ��, ���ŷ�, ���, ����, ���) (ȿ���� �ߵ� ���� : ���� ��� -> ī�� �ٿ -> �α� ���� -> �÷��̾� �� VFX)
    async Task AddOrSubtractStackSum(List<S_Card> triggerCards, int value)
    {
        // value�� ���� �տ� ���ϱ�
        pStat.AddOrSubtractStackSum(value);

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // ����� ��
        if (value > 0)
        {
            // �α� ����
            GenerateEffectLog($"���� �� +{value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_StackSum);
        }
        else
        {
            // �α� ����
            GenerateEffectLog($"���� �� {value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_StackSum);
        }

        // ����Ʈ�� Ŭ����Ʈ �߰� üũ
        pStat.CheckBurstAndCleanHit();

        // ����Ʈ�� Ŭ����Ʈ�� ���� ȿ�� �ѱ�
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();

        // ����Ʈ �� Ŭ����Ʈ Ȯ��
        if (pStat.IsBurst)
        {
            // �α� ����
            GenerateEffectLog($"����Ʈ!");

            await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
        }
        else if (pStat.IsCleanHit)
        {
            // �α� ����
            GenerateEffectLog($"Ŭ����Ʈ!");

            await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
        }
    }
    async Task AddOrSubtractLimit(List<S_Card> triggerCards, int value)
    {
        // value�� �Ѱ迡 ���ϱ�
        pStat.AddOrSubtractLimit(value);

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // ����� ��
        if (value > 0)
        {
            // �α� ����
            GenerateEffectLog($"�Ѱ� +{value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Limit);
        }
        else
        {
            // �α� ����
            GenerateEffectLog($"�Ѱ� {value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Limit);
        }

        // ����Ʈ�� Ŭ����Ʈ �߰� üũ
        pStat.CheckBurstAndCleanHit();

        // ����Ʈ�� Ŭ����Ʈ�� ���� ȿ�� �ѱ�
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();

        // ����Ʈ �� Ŭ����Ʈ Ȯ��
        if (pStat.IsBurst)
        {
            // �α� ����
            GenerateEffectLog($"����Ʈ!");

            await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
        }
        else if (pStat.IsCleanHit)
        {
            // �α� ����
            GenerateEffectLog($"Ŭ����Ʈ!");

            await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
        }
    }
    async Task AddOrSubtractBattleStats(List<S_Card> triggerCards, S_BattleStatEnum stat, int value)
    {
        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // �ɷ�ġ ����
        if (value > 0)
        {
            switch (stat)
            {
                case S_BattleStatEnum.Strength:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"�� +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength);
                    break;
                case S_BattleStatEnum.Mind:
                    pStat.AddMind(value);
                    GenerateEffectLog($"���ŷ� +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind);
                    break;
                case S_BattleStatEnum.Luck:
                    pStat.AddLuck(value);
                    GenerateEffectLog($"��� +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck);
                    break;
                case S_BattleStatEnum.AllStat:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"�� +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength);

                    pStat.AddMind(value);
                    GenerateEffectLog($"���ŷ� +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind);

                    pStat.AddLuck(value);
                    GenerateEffectLog($"��� +{value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck);
                    break;
                case S_BattleStatEnum.Random:
                    List<S_BattleStatEnum> stats = new() { S_BattleStatEnum.Strength, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                    S_BattleStatEnum s = stats[Random.Range(0, stats.Count)];
                    if (s == S_BattleStatEnum.Strength)
                    {
                        pStat.AddStrength(value);
                        GenerateEffectLog($"�� +{value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Strength);
                    }
                    else if (s == S_BattleStatEnum.Mind)
                    {
                        pStat.AddMind(value);
                        GenerateEffectLog($"���ŷ� +{value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Mind);
                    }
                    else if (s == S_BattleStatEnum.Luck)
                    {
                        pStat.AddLuck(value);
                        GenerateEffectLog($"��� +{value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Luck);
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
                    GenerateEffectLog($"�� {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength);
                    break;
                case S_BattleStatEnum.Mind:
                    pStat.AddMind(value);
                    GenerateEffectLog($"���ŷ� {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind);
                    break;
                case S_BattleStatEnum.Luck:
                    pStat.AddLuck(value);
                    GenerateEffectLog($"��� {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck);
                    break;
                case S_BattleStatEnum.AllStat:
                    pStat.AddStrength(value);
                    GenerateEffectLog($"�� {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength);

                    pStat.AddMind(value);
                    GenerateEffectLog($"���ŷ� {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind);

                    pStat.AddLuck(value);
                    GenerateEffectLog($"��� {value}");
                    await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck);
                    break;
                case S_BattleStatEnum.Random:
                    List<S_BattleStatEnum> stats = new() { S_BattleStatEnum.Strength, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                    S_BattleStatEnum s = stats[Random.Range(0, stats.Count)];
                    if (s == S_BattleStatEnum.Strength)
                    {
                        pStat.AddStrength(value);
                        GenerateEffectLog($"�� {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Strength);
                    }
                    else if (s == S_BattleStatEnum.Mind)
                    {
                        pStat.AddMind(value);
                        GenerateEffectLog($"���ŷ� {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Mind);
                    }
                    else if (s == S_BattleStatEnum.Luck)
                    {
                        pStat.AddLuck(value);
                        GenerateEffectLog($"��� {value}");
                        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Luck);
                    }
                    break;
            }
        }
    }
    async Task HarmCreature(List<S_Card> triggerCards, S_BattleStatEnum stat, int value)
    {
        // �����ֱ�
        if (pStat.IsBurst)
        {
            value = (value + 1) / 2;
        }
        else if (pStat.IsCleanHit)
        {
            value = (int)System.Math.Round(value * 1.5f, System.MidpointRounding.AwayFromZero);
        }
        S_FoeInfoSystem.Instance.DamagedByHarm(value);

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // �α� ����
        if (pStat.IsBurst)
        {
            GenerateEffectLog($"{value}�� ���ҵ� ����(����Ʈ)");
        }
        else if (pStat.IsCleanHit)
        {
            GenerateEffectLog($"{value}�� ������ ����(Ŭ����Ʈ)");
        }
        else
        {
            GenerateEffectLog($"{value}�� ����");
        }

        // �������� ���� ī�޶� ����ŷ(�Ͻ�����)
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

        // ���� VFX
        switch(stat)
        {
            case S_BattleStatEnum.Strength: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength); break;
            case S_BattleStatEnum.Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind); break;
            case S_BattleStatEnum.Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Luck); break;
            case S_BattleStatEnum.Strength_Mind: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Mind); break;
            case S_BattleStatEnum.Strength_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Strength_Luck); break;
            case S_BattleStatEnum.Mind_Luck: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Mind_Luck); break;
            case S_BattleStatEnum.AllStat: await S_PlayerInfoSystem.Instance.HarmVFXAsync(S_PlayerVFXEnum.Harm_Carnage); break;
        }
    }
    async Task AddOrSubtractHealth(List<S_Card> triggerCards, int value)
    {
        // ü�� �߰�
        pStat.AddOrSubtractHealth(value);

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // ����� ��
        if (value > 0)
        {
            // �α� ����
            GenerateEffectLog($"ü�� +{value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Health);
        }
        else
        {
            // �α� ����
            GenerateEffectLog($"ü�� {value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Health);
        }
    }
    async Task AddOrSubtractDetermination(List<S_Card> triggerCards, int value)
    {
        // ���� �߰�
        pStat.AddOrSubtractDetermination(value);

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // ����� ��
        if (value > 0)
        {
            // �α� ����
            GenerateEffectLog($"���� +{value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Determination);
        }
        else
        {
            // �α� ����
            GenerateEffectLog($"���� {value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Determination);
        }
    }
    async Task AddOrSubtractGold(List<S_Card> triggerCards, int value)
    {
        // ��� �߰�
        pStat.AddOrSubtractGold(value);

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // ����� ��
        if (value > 0)
        {
            // �α� ����
            GenerateEffectLog($"��� +{value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Add_Gold);
        }
        else
        {
            // �α� ����
            GenerateEffectLog($"��� {value}");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Subtract_Gold);
        }
    }
    async Task CreationCard(List<S_Card> triggerCards, int number = -1, S_CardSuitEnum suit = S_CardSuitEnum.Random)
    {
        // ī�� â��
        S_Card creationCard = S_CardManager.Instance.GenerateRandomCard(number, suit);

        // ī�� ����
        _ = S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(creationCard, S_CardOrderTypeEnum.IllusionHit);

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // �α� ����
        GenerateEffectLog($"ī�� ������!");

        // �÷��̾� �̹��� VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Creation);
    }
    async Task GetExpansion(List<S_Card> triggerCards)
    {
        // ���� ȹ��
        pStat.IsExpansion = true;

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // �α� ����
        GenerateEffectLog($"��Ʈ �� �߰� ����!");

        // ȿ�� �� ���
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    async Task GetFirst(List<S_Card> triggerCards, S_FirstEffectEnum effect)
    {
        // �켱 ȹ��
        pStat.IsFirst = effect;

        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // �α� ����
        switch (effect)
        {
            case S_FirstEffectEnum.Spade: GenerateEffectLog($"�����̵� �켱!"); break;
            case S_FirstEffectEnum.Heart: GenerateEffectLog($"��Ʈ �켱!"); break;
            case S_FirstEffectEnum.Diamond: GenerateEffectLog($"��Ƹ�� �켱!"); break;
            case S_FirstEffectEnum.Clover: GenerateEffectLog($"Ŭ�ι� �켱!"); break;
            case S_FirstEffectEnum.LeastSuit: GenerateEffectLog($"���� ���� ���� �켱!"); break;
            case S_FirstEffectEnum.One: GenerateEffectLog($"���� 1 �켱!"); break;
            case S_FirstEffectEnum.Two: GenerateEffectLog($"���� 2 �켱!"); break;
            case S_FirstEffectEnum.Three: GenerateEffectLog($"���� 3 �켱!"); break;
            case S_FirstEffectEnum.Four: GenerateEffectLog($"���� 4 �켱!"); break;
            case S_FirstEffectEnum.Five: GenerateEffectLog($"���� 5 �켱!"); break;
            case S_FirstEffectEnum.Six: GenerateEffectLog($"���� 6 �켱!"); break;
            case S_FirstEffectEnum.Seven: GenerateEffectLog($"���� 7 �켱!"); break;
            case S_FirstEffectEnum.Eight: GenerateEffectLog($"���� 8 �켱!"); break;
            case S_FirstEffectEnum.Nine: GenerateEffectLog($"���� 9 �켱!"); break;
            case S_FirstEffectEnum.Ten: GenerateEffectLog($"���� 10 �켱!"); break;
            case S_FirstEffectEnum.CleanHitNumber: GenerateEffectLog($"Ŭ����Ʈ�Ǵ� ���� �켱!"); break;
        }

        // ȿ�� �� ���
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    async Task ActivateUndertow(List<S_Card> triggerCards)
    {
        List<S_Card> stacks = pCard.GetPreStackCards();
        List<S_Card> picked = new();

        if (stacks.Count > 2) // 2�� �̻��̶�� ������ 2�� �̱�
        {
            picked = stacks
                .OrderBy(x => UnityEngine.Random.value)
                .Take(2)
                .ToList();
        }
        else // 2�� ���϶�� �׳� ��� ���� ī�� �ߵ�
        {
            picked = stacks;
        }

        if (picked.Count > 0)
        {
            foreach (S_Card card in picked)
            {
                List<S_Card> triggers = new();
                triggers = triggerCards;
                triggers.Add(card);

                // �ٿ�� ī��
                if (triggers != null && triggers.Count > 0)
                {
                    S_StackInfoSystem.Instance.BouncingStackCards(triggers);
                }

                // �α� ����
                GenerateEffectLog($"������ ī���� ȿ�� �ߵ�!");

                // �÷��̾� �̹��� VFX
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Undertow);

                // ȿ�� �ߵ�
                await ActivateBasicEffect(card);
                await ActivateDebuff(card);
                await ActivateAdditiveEffect(card);
            }
        }
    }
    async Task GuidanceCard(List<S_Card> triggerCards, List<S_Card> guidanceCards)
    {
        foreach (S_Card card in guidanceCards)
        {
            // ī�� ����
            _ = S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(card, S_CardOrderTypeEnum.BasicHit);

            // �ٿ�� ī��
            if (triggerCards != null && triggerCards.Count > 0)
            {
                S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
            }

            // �α� ����
            GenerateEffectLog($"������ ��Ʈ��!");

            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Guidance);
        }
    }
    #endregion
    #region ����� ȿ��
    async Task CurseCard(S_Card cursedCard, List<S_Card> triggerCards = null)
    {
        // ���ֳ�����(�鿪�̶�� ����)
        if (cursedCard.AdditiveEffect != S_CardAdditiveEffectEnum.Immunity)
        {
            cursedCard.IsCursed = true;
        }
        else
        {
            cursedCard.IsCursed = false;
        }

        if (cursedCard.IsInDeck) // ���� �ִ� ī�带 ������ ��� UICard�� �����ֱ�
        {
            // ī���� ���� ����Ʈ �ѱ�
            S_DeckInfoSystem.Instance.UpdateDeckCardsState();

            // UICard�� ���� �ִ� ���ֹ��� ī�� �����ֱ�
            S_UICardEffecter.Instance.CurseDeckCardVFX(cursedCard);

            // �ٿ�� ī��
            if (triggerCards != null && triggerCards.Count > 0)
            {
                S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
            }
        }
        else // ���ÿ� �ִ� ī�带 ������ ��� 
        {
            // ī���� ���� ����Ʈ �ѱ�
            S_StackInfoSystem.Instance.UpdateStackCardsState();

            // ������ ī�� �� Ʈ���� ī�� �ٿ�� VFX
            List<S_Card> cards = new() { cursedCard };
            if (triggerCards != null && triggerCards.Count > 0)
            {
                cards.AddRange(triggerCards);
            }
            S_StackInfoSystem.Instance.BouncingStackCards(cards);
        }

        // �鿪 ī�带 �����Ϸ������� ������ �α׿� �̹��� VFX 
        if (cursedCard.IsCursed)
        {
            // �α� ����
            GenerateEffectLog("���ֹ���!");
            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Cursed);
        }
        else
        {
            // �α� ����
            GenerateEffectLog("���� ������!!(�鿪ī��)");
            // �÷��̾� �̹��� VFX
            await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.ResistanceCurse);
        }
    }
    async Task CurseRandomCards(int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, bool inDeck = true, bool inStack = false, List<S_Card> triggerCards = null) // ������ ������ ���Ǵ�� ���� ����, �ƴϸ� �׳� ���� ����
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
    async Task ExclusionRandomCards(int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, List<S_Card> triggerCards = null) // ��ü ���� ����
    {
        List<S_Card> exclusionCards = S_EffectChecker.Instance.GetRandomCardsInImmediateDeck(count, suit, num);

        if (exclusionCards.Count > 0)
        {
            foreach (S_Card exclusionCard in exclusionCards)
            {
                // ī�� �����ϱ�
                _ = S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(exclusionCard, S_CardOrderTypeEnum.Exclusion);

                // �ٿ�� ī��
                if (triggerCards != null && triggerCards.Count > 0)
                {
                    S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
                }

                // �α� ����
                GenerateEffectLog($"ī�� ������!");

                // �÷��̾� �̹��� VFX
                await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Exclusion);
            }
        }
    }
    async Task GetDelusion(List<S_Card> triggerCards) // ����
    {
        // ���� ȹ��
        pStat.IsDelusion = true;

        // �ٿ�� VFX
        S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);

        // �α� ����
        GenerateEffectLog($"���� �ɸ�!");

        // ȿ�� �� ���
        S_StatInfoSystem.Instance.ChangeSpecialAbility();
        S_PlayerInfoSystem.Instance.ChangeSpecialAbilityVFX();
        await Task.Delay(Mathf.RoundToInt(GetEffectLifeTime() * 1000));
    }
    async Task CursedCardActivateEffect(List<S_Card> triggerCards) // ���ֹ��� ī�尡 ȿ���� �ߵ��Ϸ��� ��
    {
        // �ٿ�� ī��
        if (triggerCards != null && triggerCards.Count > 0)
        {
            S_StackInfoSystem.Instance.BouncingStackCards(triggerCards);
        }

        // �α� ����
        GenerateEffectLog("���ֹ��� ī��!");

        // �÷��̾� �̹��� VFX
        await S_PlayerInfoSystem.Instance.PlayerVFXAsync(S_PlayerVFXEnum.Cursed);
    }
    #endregion
    #region �ɷ� ȿ��
    public async Task AddBattleStats(S_Skill skill, List<S_Card> triggerCards, S_BattleStatEnum stat, int value)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await AddOrSubtractBattleStats(triggerCards, stat, value);
    }
    public async Task HarmCreature(S_Skill skill, List<S_Card> triggerCards, S_BattleStatEnum stat, int value)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await HarmCreature(triggerCards, stat, value);
    }
    public async Task CreationCard(S_Skill skill, List<S_Card> triggerCards, int number = -1, S_CardSuitEnum suit = S_CardSuitEnum.Random)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await CreationCard(triggerCards, number, suit);
    }
    public async Task GetExpansion(S_Skill skill, List<S_Card> triggerCards)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await GetExpansion(triggerCards);
    }
    public async Task GetFirst(S_Skill skill, List<S_Card> triggerCards, S_FirstEffectEnum effect)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await GetFirst(triggerCards, effect);
    }
    public async Task ActivateUndertow(S_Skill skill, List<S_Card> triggerCards)
    {
        S_SkillInfoSystem.Instance.BouncingSkillObjectVFX(skill);

        await ActivateUndertow(triggerCards);
    }
    #endregion
    #region �� ȿ��
    public async Task CurseRandomCards(S_Foe foe, int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, bool inDeck = true, bool inStack = false, List<S_Card> triggerCards = null) // ����
    {
        S_FoeInfoSystem.Instance.FoeImageBouncingVFX();

        await CurseRandomCards(count, suit, num, inDeck, inStack, triggerCards);
    }
    public async Task ExclusionRandomCard(S_Foe foe, int count, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1, List<S_Card> triggerCards = null) // ����
    {
        S_FoeInfoSystem.Instance.FoeImageBouncingVFX();

        await ExclusionRandomCards(count, suit, num, triggerCards);
    }
    public async Task GetDelusion(S_Foe foe, List<S_Card> triggerCards) // ����
    {
        S_FoeInfoSystem.Instance.FoeImageBouncingVFX();

        await GetDelusion(triggerCards);
    }
    public async Task AddOrSubtractHealth(S_Foe foe, List<S_Card> triggerCards, int value)
    {
        S_FoeInfoSystem.Instance.FoeImageBouncingVFX();

        await AddOrSubtractHealth(triggerCards, value);
    }
    public async Task AddOrSubtractDetermination(S_Foe foe, List<S_Card> triggerCards, int value)
    {
        S_FoeInfoSystem.Instance.FoeImageBouncingVFX();

        await AddOrSubtractDetermination(triggerCards, value);
    }
    public async Task AddOrSubtractGold(S_Foe foe, List<S_Card> triggerCards, int value)
    {
        await AddOrSubtractGold(triggerCards, value);
    }
    #endregion
    #region VFX ����
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
    public void GenerateEffectLog(string text) // ����Ʈ �α� ���� �޼���
    {
        // ȿ�� �α� ����
        GameObject go = Instantiate(prefab_EffectLog, transform);
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-logStartPosX, logStartPosX), Random.Range(-logStartPosY, logStartPosY) - logStartPosYOffset);
        go.GetComponent<S_EffectLog>().SetEffectText(text);

        // ȿ�� �α� VFX
        go.GetComponent<S_EffectLog>().text_EffectContent.DOFade(0f, 0f);

        Sequence seq = DOTween.Sequence();

        seq.Append(go.GetComponent<RectTransform>().DOAnchorPosY(logMoveAmount, GetEffectLogLifeTime()))
            .Join(go.GetComponent<S_EffectLog>().text_EffectContent.DOFade(1f, GetEffectLogLifeTime() / 3))
            .AppendInterval(GetEffectLogLifeTime() / 3)
            .Join(go.GetComponent<S_EffectLog>().text_EffectContent.DOFade(0f, GetEffectLogLifeTime() / 3))
            .OnComplete(() => Destroy(go));
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
