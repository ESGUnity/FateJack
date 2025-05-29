using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class S_EffectChecker : MonoBehaviour
{
    [Header("������Ʈ")]
    S_PlayerCard pCard;
    S_PlayerStat pStat;
    S_PlayerSkill pSkill;

    // �̱���
    static S_EffectChecker instance;
    public static S_EffectChecker Instance { get { return instance; } }

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

    // ���� ����, ���� �������� üũ�ϴ� �޼���
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

    // ���� üũ
    public int GetSameSuitSumInStack(S_CardSuitEnum suit) // ���ÿ��� ���� ������ �� 
    {
        return pCard.GetPreStackCards().Where(x => IsSameSuit(x.Suit, suit)).Sum(x => x.Number);
    }
    // ȥ�� üũ(����)
    public int GetSuitCountGreaterThanAmountInStack(int amount) // ���ÿ��� ī�尡 amount �̻� �����ϴ� ���� ����
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
    public int GetSuitCountGreaterThanAmountInStackInCurrentTurn(int amount) // ���ÿ��� ī�尡 amount �̻� �����ϴ� ���� ����(�� �� �Ͽ�)
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
    // ���� üũ
    public int GetContinueNumMaxLengthInStack() // ���ÿ��� ���ӵǴ� ���� �ִ� ����
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
    public int GetContinueNumMaxLengthInStackInCurrentTurn()  // ���ÿ��� ���ӵǴ� ���� �ִ� ����(�� �� �Ͽ�)
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
    public int GetContinueNumSameSuitMaxLengthInStack()  // ���ÿ��� ���� �����̸鼭 ���ӵǴ� ���� �ִ� ����
    {
        List<S_Card> cards = pCard.GetPreStackCards();
        if (cards.Count == 0) return 0;

        int maxLen = 1;

        List<S_CardSuitEnum> list = new List<S_CardSuitEnum> { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };
        foreach (var suit in list)
        {
            // baseSuit�� ���� �׷����� ���ֵǴ� ī��� ����
            List<int> numbers = new List<int>();

            for (int j = 0; j < cards.Count; j++)
            {
                if (IsSameSuit(suit, cards[j].Suit))
                {
                    numbers.Add(cards[j].Number);
                }
            }

            // �ߺ� ���� �� ����
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
    // ���� üũ
    public List<S_Card> GetSameSuitCardsInStack(S_CardSuitEnum suit) // ���ÿ��� suit�� ���� ������ ī��
    {
        return pCard.GetPreStackCards().Where(x => IsSameSuit(x.Suit, suit)).ToList();
    }
    public List<S_Card> GetSameSuitCardsInStackInCurrentTurn(S_CardSuitEnum suit) // ���ÿ��� suit�� ���� ������ ī��(�� �� �Ͽ�)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Where(x => IsSameSuit(x.Suit, suit)).ToList();
    }
    public List<S_Card> GetSameNumberCardsInStack(int num) // ���ÿ��� num�� ���� ������ ī��
    {
        return pCard.GetPreStackCards().Where(x => IsSameNumber(x.Number, num)).ToList();
    }
    public List<S_Card> GetSameNumberCardsInStackInCurrentTurn(int num) // ���ÿ��� num�� ���� ������ ī��(�� �� �Ͽ�)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Where(x => IsSameNumber(x.Number, num)).ToList();
    }
    public List<S_Card> GetPlethoraNumberCardsInStack() // ���ÿ��� 8 �̻� ������ ī��
    {
        return pCard.GetPreStackCards().Where(x => IsPlethoraNumber(x.Number)).ToList();
    }
    public List<S_Card> GetPlethoraNumberCardsInStackInCurrentTurn() // ���ÿ��� 8 �̻� ������ ī��(�� �� �Ͽ�)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).Where(x => IsPlethoraNumber(x.Number)).ToList();
    }
    // ���� üũ
    public List<S_Card> GetCardsInStackInCurrentTurn() // ������ ī��(�� �� �Ͽ�)
    {
        return pCard.GetPreStackCards().Where(x => x.IsCurrentTurnHit).ToList();
    }

    public S_BattleStatEnum GetHighestStats(out int value) // ���� ���� �ɷ�ġ
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

        // ���� ����ִٸ� �⺻���� �� ����Ʈ ��ȯ
        if (decks.Count == 0)
        {
            leastSuit = S_CardSuitEnum.Spade; // �⺻��
            return new List<S_Card>();
        }

        // ���纰 �׷��� �� ���� ���� �� ����
        var grouped = decks
            .GroupBy(x => x.Suit)
            .OrderBy(g => g.Count())
            .ToList();

        int minCount = grouped.First().Count();

        // �ּ� ���� �׷� �� ������ ����
        var leastSuitGroups = grouped.Where(g => g.Count() == minCount).ToList();
        var chosenGroup = leastSuitGroups[UnityEngine.Random.Range(0, leastSuitGroups.Count)];

        leastSuit = chosenGroup.Key;
        return chosenGroup.ToList();
    }
    public List<S_Card> GetLeastSuitCardsInStack(out S_CardSuitEnum leastSuit)
    {
        List<S_Card> stacks = pCard.GetPreStackCards();

        // ���� ����ִٸ� �⺻���� �� ����Ʈ ��ȯ
        if (stacks.Count == 0)
        {
            leastSuit = S_CardSuitEnum.Spade; // �⺻��
            return new List<S_Card>();
        }

        // ���纰 �׷��� �� ���� ���� �� ����
        var grouped = stacks
            .GroupBy(x => x.Suit)
            .OrderBy(g => g.Count())
            .ToList();

        int minCount = grouped.First().Count();

        // �ּ� ���� �׷� �� ������ ����
        var leastSuitGroups = grouped.Where(g => g.Count() == minCount).ToList();
        var chosenGroup = leastSuitGroups[UnityEngine.Random.Range(0, leastSuitGroups.Count)];

        leastSuit = chosenGroup.Key;
        return chosenGroup.ToList();
    }
    public List<S_Card> GetLeastNumberCardsInDeck(out int leastNumber)
    {
        List<S_Card> decks = pCard.GetPreDeckCards();

        // ���� ����ִٸ� �⺻���� �� ����Ʈ ��ȯ
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
    public S_CardSuitEnum GetRandomSuit() // ������ ����
    {
        List<S_CardSuitEnum> list = new List<S_CardSuitEnum> { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };

        int randomIndex = Random.Range(0, list.Count);

        return list[randomIndex];
    }
    public S_BattleStatEnum GetRandomStat(out int value) // ������ �ɷ�ġ
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

    // ȿ�� �� ���� �ִ� ī�� ���ܿ� ��Ʈ(�ε�)�� ����� �� ī�� �������� 
    public List<S_Card> GetRandomCardsInImmediateDeck(int count = 999, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1) // ���� ���� ��� ī��� count�� 999�� ������ �ȴ�.
    {
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
    public List<S_Card> GetRandomCardsInImmediateStack(int count = 999, S_CardSuitEnum suit = S_CardSuitEnum.None, int num = -1) // ���� ���� ��� ī��� count�� 999�� ������ �ȴ�.
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
