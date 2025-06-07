using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerCard : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerSkill pSkill;
    S_PlayerStat pStat;

    [Header("카드 관련")]
    List<S_Card> originPlayerDeck = new(); // 시련 진행 중엔 절대 바뀌지 않는 불변 덱

    // pre는 카드 효과 계산 시 카드가 내진 순간의 조건을 검사하기 위한 것.
    // immediate는 카드오더큐에 들어갈 때 즉시 계산되는 리스트. 즉 카드에 있는 것을 인도하거나 제외할 때 필요한 것.
    List<S_Card> preDeckCards = new(); // 덱에 있는 카드
    List<S_Card> immediateDeckCards = new(); // 덱에 있는 카드

    List<S_Card> preStackCards = new(); // 스택에 있는 카드
    List<S_Card> immediateStackCards = new(); // 스택에 있는 카드

    List<S_Card> preExclusionDeckCards = new(); // 제외된 카드
    List<S_Card> immediateExclusionDeckCards = new(); // 제외된 카드

    List<S_Card> preExclusionTotalCards = new(); // 환상 카드 합쳐서 제외된 카드
    List<S_Card> immediateExclusionTotalCards = new(); // 환상 카드 합쳐서 제외된 카드

    // 싱글턴
    static S_PlayerCard instance;
    public static S_PlayerCard Instance { get { return instance; } }

    void Awake()
    {
        // 컴포넌트 할당
        pSkill = GetComponent<S_PlayerSkill>();
        pStat = GetComponent<S_PlayerStat>();

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

    #region 카드 뽑기 및 덱 관리 부분
    public void InitDeckByStartGame() // 시작 시 카드 추가
    {
        foreach (S_Card card in S_CardManager.Instance.GenerateCardByStartGame())
        {
            card.IsInDeck = true;
            card.IsIllusion = false;
            card.IsCurrentTurnHit = false;
            card.IsCursed = false;
            AddCard(card);
        }
    }
    public void AddCard(S_Card card) // 덱에 카드 추가
    {
        originPlayerDeck.Add(card);
        S_DeckInfoSystem.Instance.AddDeck(card);
    }
    public void RemoveCard(S_Card card) // 덱에서 카드 제거
    {
        originPlayerDeck.Remove(card);
        S_DeckInfoSystem.Instance.RemoveDeck(card);
    }
    public List<S_Card> DrawRandomCard(int drawCount) // 히트 시 호출. 전개에도
    {
        List<S_Card> remainDeck = GetPreDeckCards();
        List<S_Card> pickedCards = new();
        List<S_Card> selected = new();

        if (remainDeck.Count > drawCount)
        {
            while (selected.Count < drawCount)
            {
                // 우선 체크
                switch (pStat.IsFirst)
                {
                    case S_FirstEffectEnum.None: break;
                    case S_FirstEffectEnum.Spade: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Spade).ToList(); break;
                    case S_FirstEffectEnum.Heart: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Heart).ToList(); break;
                    case S_FirstEffectEnum.Diamond: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Diamond).ToList(); break;
                    case S_FirstEffectEnum.Clover: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Clover).ToList(); break;
                    case S_FirstEffectEnum.LeastSuit:
                        var grouped = remainDeck.GroupBy(c => c.Suit)
                            .OrderBy(x => x.Count())
                            .ToList();

                        // 동률 대비 로직
                        int minCount = grouped.First().Count();
                        var leastSuitGroups = grouped.Where(g => g.Count() == minCount).ToList();
                        var chosenGroup = leastSuitGroups[UnityEngine.Random.Range(0, leastSuitGroups.Count)];

                        // 제일 적은 문양 카드들 반환
                        pickedCards = chosenGroup.ToList();
                        break;
                    case S_FirstEffectEnum.One: pickedCards = remainDeck.Where(x => x.Number == 1).ToList(); break;
                    case S_FirstEffectEnum.Two: pickedCards = remainDeck.Where(x => x.Number == 2).ToList(); break;
                    case S_FirstEffectEnum.Three: pickedCards = remainDeck.Where(x => x.Number == 3).ToList(); break;
                    case S_FirstEffectEnum.Four: pickedCards = remainDeck.Where(x => x.Number == 4).ToList(); break;
                    case S_FirstEffectEnum.Five: pickedCards = remainDeck.Where(x => x.Number == 5).ToList(); break;
                    case S_FirstEffectEnum.Six: pickedCards = remainDeck.Where(x => x.Number == 6).ToList(); break;
                    case S_FirstEffectEnum.Seven: pickedCards = remainDeck.Where(x => x.Number == 7).ToList(); break;
                    case S_FirstEffectEnum.Eight: pickedCards = remainDeck.Where(x => x.Number == 8).ToList(); break;
                    case S_FirstEffectEnum.Nine: pickedCards = remainDeck.Where(x => x.Number == 9).ToList(); break;
                    case S_FirstEffectEnum.Ten: pickedCards = remainDeck.Where(x => x.Number == 10).ToList(); break;
                    case S_FirstEffectEnum.CleanHitNumber:
                        int diff = S_PlayerStat.Instance.CurrentLimit - S_PlayerStat.Instance.StackSum;
                        if (diff <= 0)
                        {
                            pickedCards = remainDeck.Where(x => x.Number == 1).ToList();
                        }
                        else if (diff > 0 && diff <= 10)
                        {
                            pickedCards = remainDeck.Where(x => x.Number == diff).ToList();
                        }
                        else if (diff > 10)
                        {
                            pickedCards = remainDeck.Where(x => x.Number == 10).ToList();
                        }
                        break;
                }

                // cards가 비었다면, 즉 우선으로 뽑을 카드가 없다면
                if (pickedCards.Count <= 0)
                {
                    pickedCards = remainDeck.ToList();
                }

                int randomIndex = Random.Range(0, pickedCards.Count);
                S_Card pick = pickedCards[randomIndex];

                selected.Add(pick);
                remainDeck.Remove(pick);
            }

            return selected;
        }
        else
        {
            return remainDeck;
        }
    }
    public List<S_Card> GetValidCardsByFirst() // 우선 대상 카드가 없으면 빈 리스트 반환함. 조심
    {
        List<S_Card> deckCards = GetPreDeckCards();
        List<S_Card> pickedCards = new();
        if (deckCards.Count <= 0) return pickedCards;

        S_FirstEffectEnum first = pStat.IsFirst;

        if (first == S_FirstEffectEnum.None)
        {
            return deckCards;
        }
        else
        {
            switch (first)
            {
                case S_FirstEffectEnum.Spade: pickedCards = deckCards.Where(x => x.Suit == S_CardSuitEnum.Spade).ToList(); break;
                case S_FirstEffectEnum.Heart: pickedCards = deckCards.Where(x => x.Suit == S_CardSuitEnum.Heart).ToList(); break;
                case S_FirstEffectEnum.Diamond: pickedCards = deckCards.Where(x => x.Suit == S_CardSuitEnum.Diamond).ToList(); break;
                case S_FirstEffectEnum.Clover: pickedCards = deckCards.Where(x => x.Suit == S_CardSuitEnum.Clover).ToList(); break;
                case S_FirstEffectEnum.LeastSuit:
                    var grouped = deckCards.GroupBy(c => c.Suit)
                        .OrderBy(x => x.Count())
                        .ToList();

                    // 동률 대비 로직
                    int minCount = grouped.First().Count();
                    var leastSuitGroups = grouped.Where(g => g.Count() == minCount).ToList();

                    // 모든 동률 문양의 카드들을 합쳐서 반환
                    pickedCards = leastSuitGroups.SelectMany(g => g).ToList();
                    break;
                case S_FirstEffectEnum.One: pickedCards = deckCards.Where(x => x.Number == 1).ToList(); break;
                case S_FirstEffectEnum.Two: pickedCards = deckCards.Where(x => x.Number == 2).ToList(); break;
                case S_FirstEffectEnum.Three: pickedCards = deckCards.Where(x => x.Number == 3).ToList(); break;
                case S_FirstEffectEnum.Four: pickedCards = deckCards.Where(x => x.Number == 4).ToList(); break;
                case S_FirstEffectEnum.Five: pickedCards = deckCards.Where(x => x.Number == 5).ToList(); break;
                case S_FirstEffectEnum.Six: pickedCards = deckCards.Where(x => x.Number == 6).ToList(); break;
                case S_FirstEffectEnum.Seven: pickedCards = deckCards.Where(x => x.Number == 7).ToList(); break;
                case S_FirstEffectEnum.Eight: pickedCards = deckCards.Where(x => x.Number == 8).ToList(); break;
                case S_FirstEffectEnum.Nine: pickedCards = deckCards.Where(x => x.Number == 9).ToList(); break;
                case S_FirstEffectEnum.Ten: pickedCards = deckCards.Where(x => x.Number == 10).ToList(); break;
                case S_FirstEffectEnum.CleanHitNumber:
                    int diff = S_PlayerStat.Instance.CurrentLimit - S_PlayerStat.Instance.StackSum;
                    if (diff <= 0)
                    {
                        int num = 0;
                        pickedCards = deckCards.Where(x => x.Number == num).ToList();
                        while (pickedCards.Count <= 0)
                        {
                            num++;
                            pickedCards = deckCards.Where(x => x.Number == num).ToList();
                        }
                    }
                    else if (diff > 0 && diff <= 10)
                    {
                        pickedCards = deckCards.Where(x => x.Number == diff).ToList();
                    }
                    else if (diff > 10)
                    {
                        int num = 10;
                        pickedCards = deckCards.Where(x => x.Number == num).ToList();
                        while (pickedCards.Count <= 0)
                        {
                            num--;
                            pickedCards = deckCards.Where(x => x.Number == num).ToList();
                        }
                    }
                    break;
            }

            return pickedCards;
        }
    }
    public void InitCardsByStartTrial() // 시작 시 preDeck이랑 immediateDeck 채우기
    {
        preDeckCards = GetOriginPlayerDeckCards();
        immediateDeckCards = GetOriginPlayerDeckCards();
    }
    #endregion
    #region 히트 및 스택 계산 부분
    public void HitCardByDeckPre(S_Card hitCard) // 덱에서 히트. 스택과 덱 업데이트
    {
        preDeckCards.Remove(hitCard);
        preStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = false;
    }
    public void HitCardByDeckImmediate(S_Card hitCard) // 덱에서 히트. 스택과 덱 업데이트
    {
        immediateDeckCards.Remove(hitCard);
        immediateStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = false;
    }
    public void HitCardByIllusionPre(S_Card hitCard) // 새롭게 생성하여 히트. 스택과 덱 업데이트
    {
        preStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = true;
    }
    public void HitCardByIllusionImmediate(S_Card hitCard) // 새롭게 생성하여 히트. 스택과 덱 업데이트
    {
        immediateStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = true;
    }
    public void ExclusionCardByExclusionPre(S_Card exclusionCard) // 제외에 의한 덱 카드 제외
    {
        preDeckCards.Remove(exclusionCard);
        preExclusionTotalCards.Add(exclusionCard);
        preExclusionDeckCards.Add(exclusionCard);

        exclusionCard.IsInDeck = false;
        exclusionCard.IsCurrentTurnHit = true;
        exclusionCard.IsIllusion = false;
    }
    public void ExclusionCardByExclusionImmediate(S_Card exclusionCard) // 제외에 의한 덱 카드 제외
    {
        immediateDeckCards.Remove(exclusionCard);
        immediateExclusionTotalCards.Add(exclusionCard);
        immediateExclusionDeckCards.Add(exclusionCard);

        exclusionCard.IsInDeck = false;
        exclusionCard.IsCurrentTurnHit = true;
        exclusionCard.IsIllusion = false;
    }
    public void ResetCardsByTwist(out List<S_Card> stacks, out List<S_Card> exclusions) // 비틀기에 의한 카드 제외 및 제외된 카드 돌아오기
    {
        // 낸 카드 제외하기
        stacks = GetPreStackCards().Where(x => x.IsCurrentTurnHit).ToList();
        foreach (S_Card card in stacks)
        {
            preStackCards.Remove(card);
            immediateStackCards.Remove(card);

            preExclusionTotalCards.Add(card);
            immediateExclusionTotalCards.Add(card);

            if (!card.IsIllusion)
            {
                preExclusionDeckCards.Add(card);
                immediateExclusionDeckCards.Add(card);
            }

            card.IsCurrentTurnHit = false;
        }
        foreach (S_Card card in preStackCards)
        {
            card.IsCurrentTurnHit = false;
        }

        // 제외된 카드는 덱으로 되돌아오기
        exclusions = preExclusionDeckCards.ToList().Where(x => x.IsCurrentTurnHit).ToList();
        foreach (S_Card card in exclusions)
        {
            preDeckCards.Add(card);
            immediateDeckCards.Add(card);

            preExclusionTotalCards.Remove(card);
            immediateExclusionTotalCards.Remove(card);

            if (!card.IsIllusion)
            {
                preExclusionDeckCards.Remove(card);
                immediateExclusionDeckCards.Remove(card);
            }

            card.IsInDeck = true;
            card.IsCurrentTurnHit = false;
        }
        foreach (S_Card card in preExclusionTotalCards)
        {
            card.IsCurrentTurnHit = false;
        }

        // 덱, 스택 인포 업데이트
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
        S_StackInfoSystem.Instance.UpdateStackCardsState();
    }
    public void FixCardsByStand()
    {
        foreach (S_Card card in preStackCards)
        {
            card.IsCurrentTurnHit = false;
        }
        foreach (S_Card card in preExclusionTotalCards)
        {
            card.IsCurrentTurnHit = false;
        }

        // 덱, 스택 인포 업데이트
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
        S_StackInfoSystem.Instance.UpdateStackCardsState();
    }
    public void ResetCardsByEndTrial()
    {
        // 덱 카드 정상화
        foreach (S_Card card in originPlayerDeck)
        {
            card.IsInDeck = true;
            card.IsCurrentTurnHit = false;
            card.IsIllusion = false;
            card.IsCursed = false;
            card.IsMeetCondition = false;
        }

        preStackCards.Clear();
        immediateStackCards.Clear();
        preExclusionDeckCards.Clear();
        immediateExclusionDeckCards.Clear();
        preExclusionTotalCards.Clear();
        immediateExclusionTotalCards.Clear();
        InitCardsByStartTrial();

        // 덱 인포 업데이트
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
    }
    #endregion
    #region 보조 메서드
    public List<S_Card> GetOriginPlayerDeckCards()
    {
        return originPlayerDeck.ToList();
    }
    public List<S_Card> GetPreDeckCards()
    {
        return preDeckCards.ToList();
    }
    public List<S_Card> GetImmediateDeckCards()
    {
        return immediateDeckCards.ToList();
    }
    public List<S_Card> GetPreStackCards()
    {
        return preStackCards.ToList();
    }
    public List<S_Card> GetImmediateStackCards()
    {
        return immediateStackCards.ToList();
    }
    public List<S_Card> GetPreExclusionTotalCards()
    {
        return preExclusionTotalCards.ToList();
    }
    public List<S_Card> GetImmediateExclusionTotalCards()
    {
        return immediateExclusionTotalCards.ToList();
    }

    public void CheckCardMeetCondition(S_Card hitCard)
    {
        // 카드를 내는 순간 카드의 조건 체크
        foreach (S_Card card in GetPreStackCards())
        {
            if (card.BasicCondition != S_CardBasicConditionEnum.Unleash)
            {
                card.IsMeetCondition = S_EffectActivator.Instance.IsMeetAdditiveCondition(card, hitCard);
            }
            else
            {
                card.IsMeetCondition = false;
            }
        }

        // 방금 낸 카드가 발현이라면 별도로 체크하기
        if (hitCard.BasicCondition == S_CardBasicConditionEnum.Unleash)
        {
            if (S_EffectActivator.Instance.IsMeetAdditiveCondition(hitCard))
            {
                hitCard.IsMeetCondition = true;
            }
            else
            {
                hitCard.IsMeetCondition = false;
            }
        }

        S_StackInfoSystem.Instance.UpdateStackCardsState();
    }
    public void CheckCardMeetConditionAfterEffect(S_Card targetCard)
    {
        // 카드들의 초록불 체크(발현, 일부 메아리들은 꺼짐)
        targetCard.IsMeetCondition = S_EffectActivator.Instance.IsMeetAdditiveCondition(targetCard);

        if (targetCard.BasicCondition == S_CardBasicConditionEnum.Unleash)
        {
            targetCard.IsMeetCondition = false;
        }

        S_StackInfoSystem.Instance.UpdateStackCardsState();
    }
    #endregion
}
