using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class S_EffectChecker : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerCard pCard;
    S_PlayerStat pStat;
    S_PlayerSkill pSkill;

    // 싱글턴
    static S_EffectChecker instance;
    public static S_EffectChecker Instance { get { return instance; } }

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

    // 같은 문양, 같은 숫자인지 체크하는 메서드
    public bool IsSameSuit(S_CardSuitEnum suit1, S_CardSuitEnum suit2)
    {
        bool isSameEach = false;

        bool sameBlack = false;
        bool sameRed = false;
        foreach (S_Skill skill in pSkill.GetPlayerOwnedSkills())
        {
            if (skill.Passive == S_SkillPassiveEnum.SameBlack)
            {
                sameBlack = true;
            }
            if (skill.Passive == S_SkillPassiveEnum.SameRed)
            {
                sameRed = true;
            }
        }

        switch (suit1)
        {
            case S_CardSuitEnum.Spade:
                if (sameBlack)
                {
                    isSameEach = suit2 == S_CardSuitEnum.Spade || suit2 == S_CardSuitEnum.Clover;
                }
                else
                {
                    isSameEach = suit2 == suit1;
                }
                break;
            case S_CardSuitEnum.Heart:
                if (sameRed)
                {
                    isSameEach = suit2 == S_CardSuitEnum.Heart || suit2 == S_CardSuitEnum.Diamond;
                }
                else
                {
                    isSameEach = suit2 == suit1;
                }
                break;
            case S_CardSuitEnum.Diamond:
                if (sameRed)
                {
                    isSameEach = suit2 == S_CardSuitEnum.Heart || suit2 == S_CardSuitEnum.Diamond;
                }
                else
                {
                    isSameEach = suit2 == suit1;
                }
                break;
            case S_CardSuitEnum.Clover:
                if (sameBlack)
                {
                    isSameEach = suit2 == S_CardSuitEnum.Spade || suit2 == S_CardSuitEnum.Clover;
                }
                else
                {
                    isSameEach = suit2 == suit1;
                }
                break;
        }

        return isSameEach;
    }
    public bool IsSameNumber(int num1, int num2)
    {
        return num1 == num2;
    }
    public bool IsPlethoraNumber(int num)
    {
        return num >= 8;
    }

    // 군단 체크
    public int GetSameSuitSumInStack(S_CardSuitEnum suit) // 스택에서 같은 문양의 합 
    {
        return pCard.GetPreStackCards().Where(x => IsSameSuit(x.Suit, suit)).Sum(x => x.Number);
    }
    // 혼돈 체크(단합)
    public int GetSuitCountGreaterThanAmountInStack(int amount) // 스택에서 카드가 amount 이상 존재하는 문양 개수
    {
        List<S_Card> cards = pCard.GetPreStackCards();

        int suitCount = 0;
        List<S_CardSuitEnum> suits = new() { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };

        foreach (var s in suits)
        {
            if (cards.Where(x => IsSameSuit(x.Suit, s)).Count() >= amount)
            {
                suitCount++;
            }
        }

        return suitCount;
    }
    public int GetSuitCountGreaterThanAmountInStackInCurrentTurn(int amount) // 스택에서 카드가 amount 이상 존재하는 문양 개수(단 한 턴에)
    {
        List<S_Card> cards = pCard.GetPreStackCards();

        int suitCount = 0;
        List<S_CardSuitEnum> suits = new() { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };

        foreach (var s in suits)
        {
            if (cards.Where(x => x.IsCurrentTurnHit).Where(x => IsSameSuit(x.Suit, s)).Count() >= amount)
            {
                suitCount++;
            }
        }

        return suitCount;
    }
    // 공세 체크
    public int GetContinueNumMaxLengthInStack() // 스택에서 연속되는 숫자 최대 길이
    {
        List<S_Card> cards = pCard.GetPreStackCards();

        var numbers = cards.Select(x => x.Number).Distinct().OrderBy(n => n).ToList();

        if (numbers.Count == 0)
        {
            return 0;
        }

        int maxLen = 1;
        int currentLen = 1;

        for (int i = 1; i < numbers.Count; i++)
        {
            if (numbers[i] == numbers[i - 1] + 1)
            {
                currentLen++;
                maxLen = System.Math.Max(maxLen, currentLen);
            }
            else
            {
                currentLen = 1;
            }
        }

        return maxLen;
    }
    public int GetContinueNumMaxLengthInStackInCurrentTurn()  // 스택에서 연속되는 숫자 최대 길이(단 한 턴에)
    {
        List<S_Card> cards = pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).ToList();

        var numbers = cards.Select(x => x.Number).Distinct().OrderBy(n => n).ToList();

        if (numbers.Count == 0)
        {
            return 0;
        }

        int maxLen = 1;
        int currentLen = 1;

        for (int i = 1; i < numbers.Count; i++)
        {
            if (numbers[i] == numbers[i - 1] + 1)
            {
                currentLen++;
                maxLen = System.Math.Max(maxLen, currentLen);
            }
            else
            {
                currentLen = 1;
            }
        }

        return maxLen;
    }
    public int GetContinueNumSameSuitMaxLengthInStack()  // 스택에서 같은 문양이면서 연속되는 숫자 최대 길이
    {
        List<S_Card> cards = pCard.GetPreStackCards();
        if (cards.Count == 0) return 0;

        int maxLen = 1;

        List<S_CardSuitEnum> list = new List<S_CardSuitEnum> { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };
        foreach (var suit in list)
        {
            // baseSuit와 같은 그룹으로 간주되는 카드들 모음
            List<int> numbers = new List<int>();

            for (int j = 0; j < cards.Count; j++)
            {
                if (IsSameSuit(suit, cards[j].Suit))
                {
                    numbers.Add(cards[j].Number);
                }
            }

            // 중복 제거 및 정렬
            var sortedNumbers = numbers.Distinct().OrderBy(n => n).ToList();
            int currentLen = 1;

            for (int k = 1; k < sortedNumbers.Count; k++)
            {
                if (sortedNumbers[k] == sortedNumbers[k - 1] + 1)
                {
                    currentLen++;
                    maxLen = Mathf.Max(maxLen, currentLen);
                }
                else
                {
                    currentLen = 1;
                }
            }
        }

        return maxLen;
    }
    // 정밀 체크
    public List<S_Card> GetSameSuitCardsInStack(S_CardSuitEnum suit) // 스택에서 suit와 같은 문양의 카드
    {
        return pCard.GetPreStackCards().Where(x => IsSameSuit(x.Suit, suit)).ToList();
    }
    public List<S_Card> GetSameSuitCardsInStackInCurrentTurn(S_CardSuitEnum suit) // 스택에서 suit와 같은 문양의 카드(단 한 턴에)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Where(x => IsSameSuit(x.Suit, suit)).ToList();
    }
    public List<S_Card> GetSameNumberCardsInStack(int num) // 스택에서 num과 같은 숫자의 카드
    {
        return pCard.GetPreStackCards().Where(x => IsSameNumber(x.Number, num)).ToList();
    }
    public List<S_Card> GetSameNumberCardsInStackInCurrentTurn(int num) // 스택에서 num과 같은 숫자의 카드(단 한 턴에)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Where(x => IsSameNumber(x.Number, num)).ToList();
    }
    public List<S_Card> GetPlethoraNumberCardsInStack() // 스택에서 8 이상 숫자의 카드
    {
        return pCard.GetPreStackCards().Where(x => IsPlethoraNumber(x.Number)).ToList();
    }
    public List<S_Card> GetPlethoraNumberCardsInStackInCurrentTurn() // 스택에서 8 이상 숫자의 카드(단 한 턴에)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Where(x => IsPlethoraNumber(x.Number)).ToList();
    }
    // 범람 체크
    public List<S_Card> GetCardsInStack() // 스택에서 suit와 같은 문양의 카드
    {
        return pCard.GetPreStackCards().ToList();
    }
    public List<S_Card> GetCardsInStackInCurrentTurn() // 스택의 카드(단 한 턴에)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).ToList();
    }
    public List<S_Card> GetCardsInDeck() // 덱에서 suit와 같은 문양의 카드
    {
        return pCard.GetPreDeckCards().ToList();
    }
    // 추가 효과용
    public List<S_Card> GetCursedCardsInDeckAndStack()
    {
        List<S_Card> cards = new List<S_Card>();

        cards.AddRange(pCard.GetPreDeckCards().Where(x => x.IsCursed).ToList());
        cards.AddRange(pCard.GetPreStackCards().Where(x => x.IsCursed).ToList());

        return cards;
    }


    public S_BattleStatEnum GetHighestStats(out int value) // 가장 높은 능력치
    {
        var stats = new (S_BattleStatEnum stat, int val)[]
        {
            (S_BattleStatEnum.Strength, pStat.CurrentStrength),
            (S_BattleStatEnum.Mind, pStat.CurrentMind),
            (S_BattleStatEnum.Luck, pStat.CurrentLuck)
        };

        var max = stats[0];
        foreach (var stat in stats)
        {
            if (stat.val > max.val)
            {
                max = stat;
            }
        }

        value = max.val;
        return max.stat;
    }
    public List<S_Card> GetLeastSuitCardsInDeck(out S_CardSuitEnum leastSuit)
    {
        List<S_Card> decks = pCard.GetPreDeckCards();

        // 덱이 비어있다면 기본값과 빈 리스트 반환
        if (decks.Count == 0)
        {
            leastSuit = S_CardSuitEnum.Spade; // 기본값
            return new List<S_Card>();
        }

        // 문양별 그룹핑 후 개수 적은 순 정렬
        var grouped = decks
            .GroupBy(x => x.Suit)
            .OrderBy(g => g.Count())
            .ToList();

        int minCount = grouped.First().Count();

        // 최소 개수 그룹 중 무작위 선택
        var leastSuitGroups = grouped.Where(g => g.Count() == minCount).ToList();
        var chosenGroup = leastSuitGroups[UnityEngine.Random.Range(0, leastSuitGroups.Count)];

        leastSuit = chosenGroup.Key;
        return chosenGroup.ToList();
    }
    public List<S_Card> GetLeastSuitCardsInStack(out S_CardSuitEnum leastSuit)
    {
        List<S_Card> stacks = pCard.GetPreStackCards();

        // 덱이 비어있다면 기본값과 빈 리스트 반환
        if (stacks.Count == 0)
        {
            leastSuit = S_CardSuitEnum.Spade; // 기본값
            return new List<S_Card>();
        }

        // 문양별 그룹핑 후 개수 적은 순 정렬
        var grouped = stacks
            .GroupBy(x => x.Suit)
            .OrderBy(g => g.Count())
            .ToList();

        int minCount = grouped.First().Count();

        // 최소 개수 그룹 중 무작위 선택
        var leastSuitGroups = grouped.Where(g => g.Count() == minCount).ToList();
        var chosenGroup = leastSuitGroups[UnityEngine.Random.Range(0, leastSuitGroups.Count)];

        leastSuit = chosenGroup.Key;
        return chosenGroup.ToList();
    }
    public List<S_Card> GetLeastNumberCardsInDeck(out int leastNumber)
    {
        List<S_Card> decks = pCard.GetPreDeckCards();

        // 덱이 비어있다면 기본값과 빈 리스트 반환
        if (decks.Count == 0)
        {
            leastNumber = 1;
            return new List<S_Card>();
        }

        var grouped = decks
            .GroupBy(x => x.Number)
            .OrderBy(g => g.Count())
            .ToList();

        int minCount = grouped.First().Count();

        var leastNumberGroups = grouped
            .Where(g => g.Count() == minCount)
            .ToList();

        var chosenGroup = leastNumberGroups[UnityEngine.Random.Range(0, leastNumberGroups.Count)];

        leastNumber = chosenGroup.Key;
        return chosenGroup.ToList();
    }
    public List<S_Card> GetSpecificNumberCardsByDeck(int number)
    {
        return pCard.GetPreDeckCards().Where(x => IsSameNumber(x.Number, number)).ToList();
    }
    public S_Card GetRandomCardByDeck(bool onlyNotCursed = false)
    {
        List<S_Card> cards = pCard.GetPreDeckCards();

        if (onlyNotCursed)
        {
            cards = cards.Where(x => !x.IsCursed).ToList();
        }

        if (cards.Count > 0)
        {
            return cards[Random.Range(0, cards.Count)];
        }
        else
        {
            return null;
        }
    }
    public S_Card GetRandomCardByStack(bool onlyNotCursed = false)
    {
        List<S_Card> cards = pCard.GetPreStackCards();

        if (onlyNotCursed)
        {
            cards = cards.Where(x => !x.IsCursed).ToList();
        }

        if (cards.Count > 0)
        {
            return cards[Random.Range(0, cards.Count)];
        }
        else
        {
            return null;
        }
    }
    public int GetSuitCountByStack()
    {
        List<S_Card> cards = pCard.GetPreStackCards();

        int suitCounts = cards
            .Select(x => x.Suit)
            .Distinct()
            .Count();

        return suitCounts;
    }
    public int GetDeckSuitCount()
    {
        List<S_Card> cards = pCard.GetPreDeckCards();

        return cards.Select(x => x.Suit).Distinct().Count();
    }
    public S_CardSuitEnum GetRandomSuit() // 무작위 문양
    {
        List<S_CardSuitEnum> list = new List<S_CardSuitEnum> { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };

        int randomIndex = Random.Range(0, list.Count);

        return list[randomIndex];
    }
    public S_BattleStatEnum GetRandomStat(out int value) // 무작위 능력치
    {
        var stats = new (S_BattleStatEnum stat, int val)[]
        {
            (S_BattleStatEnum.Strength, pStat.CurrentStrength),
            (S_BattleStatEnum.Mind, pStat.CurrentMind),
            (S_BattleStatEnum.Luck, pStat.CurrentLuck)
        };

        var selected = stats[Random.Range(0, stats.Length)];

        value = selected.val;
        return selected.stat;
    }

    // 효과 중 덱에 있는 카드 제외와 히트(인도)에 대비한 덱 카드 가져오기 
    public List<S_Card> GetRandomCardsInImmediateDeck(int count = 999, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1) // 조건 만족 모든 카드는 count에 999을 넣으면 된다.
    {
        if (pCard == null)
        {
            Debug.LogError("pCard is null in GetRandomCardsInImmediateDeck");
        }
        List<S_Card> cards = pCard.GetImmediateDeckCards();
        if (suit != S_CardSuitEnum.None)
        {
            cards = cards.Where(x => x.Suit == suit).ToList();
        }
        if (num != -1)
        {
            cards = cards.Where(x => x.Number == num).ToList();
        }

        return cards.OrderBy(x => Random.value).Take(count).ToList();
    }
    public List<S_Card> GetRandomCardsInImmediateStack(int count = 999, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1) // 조건 만족 모든 카드는 count에 999을 넣으면 된다.
    {
        List<S_Card> cards = pCard.GetImmediateStackCards();
        if (suit != S_CardSuitEnum.None)
        {
            cards = cards.Where(x => x.Suit == suit).ToList();
        }
        if (num != -1)
        {
            cards = cards.Where(x => x.Number == num).ToList();
        }

        return cards.OrderBy(x => Random.value).Take(count).ToList();
    }
}
