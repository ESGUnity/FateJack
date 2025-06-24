using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class S_EffectChecker : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerCard pCard;
    S_PlayerStat pStat;
    S_PlayerTrinket pSkill;

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
        pSkill = S_PlayerTrinket.Instance;
    }

    // 같은 타입인지 체크하는 메서드
    public bool IsSameType(S_CardTypeEnum type1, S_CardTypeEnum type2)
    {
        bool isSameEach = false;

        bool sameStrCommon = false;
        bool sameMIndLuck = false;

        foreach (S_Trinket tri in pSkill.GetPlayerOwnedTrinkets())
        {
            if (tri.Passive == S_TrinketPassiveEnum.SameStrCommon)
            {
                sameStrCommon = true;
            }
            if (tri.Passive == S_TrinketPassiveEnum.SameMindLuck)
            {
                sameMIndLuck = true;
            }
        }

        switch (type1)
        {
            case S_CardTypeEnum.Str:
                if (sameStrCommon)
                {
                    isSameEach = type2 == S_CardTypeEnum.Str || type2 == S_CardTypeEnum.Common;
                }
                else
                {
                    isSameEach = type2 == type1;
                }
                break;
            case S_CardTypeEnum.Mind:
                if (sameMIndLuck)
                {
                    isSameEach = type2 == S_CardTypeEnum.Mind || type2 == S_CardTypeEnum.Luck;
                }
                else
                {
                    isSameEach = type2 == type1;
                }
                break;
            case S_CardTypeEnum.Luck:
                if (sameMIndLuck)
                {
                    isSameEach = type2 == S_CardTypeEnum.Mind || type2 == S_CardTypeEnum.Luck;
                }
                else
                {
                    isSameEach = type2 == type1;
                }
                break;
            case S_CardTypeEnum.Common:
                if (sameStrCommon)
                {
                    isSameEach = type2 == S_CardTypeEnum.Str || type2 == S_CardTypeEnum.Common;
                }
                else
                {
                    isSameEach = type2 == type1;
                }
                break;
        }

        return isSameEach;
    }
    #region 특정 카드 가져오기
    public List<S_Card> GetSameTypeCardsInStack(S_CardTypeEnum type) // 스택에서 type과 같은 타입의 카드 리스트 반환 메서드
    {
        return pCard.GetStackCards().Where(x => IsSameType(x.CardType, type)).ToList();
    }
    public List<S_Card> GetSameTypeCardsInStackInCurrentTurn(S_CardTypeEnum type) // 스택에서 type과 같은 타입의 카드 리스트 반환 메서드(단 한 턴에)
    {
        return pCard.GetStackCards().Where(x => IsSameType(x.CardType, type)).Where(x => x.IsCurrentTurn).ToList();
    }
    public List<S_Card> GetCardsInStack() // 스택에서 suit와 같은 문양의 카드
    {
        return pCard.GetStackCards().ToList();
    }
    public List<S_Card> GetCardsInStackInCurrentTurn() // 스택의 카드(단 한 턴에)
    {
        return pCard.GetStackCards().Where(x => x.IsCurrentTurn).ToList();
    }
    public List<S_Card> GetCardsInDeck() // 덱에서 suit와 같은 문양의 카드
    {
        return pCard.GetDeckCards().ToList();
    }
    public List<S_Card> GetCursedCardsInDeckAndStack() // 모든 저주받은 카드 가져오기
    {
        List<S_Card> cards = new List<S_Card>();

        cards.AddRange(pCard.GetDeckCards().Where(x => x.IsCursed).ToList());
        cards.AddRange(pCard.GetStackCards().Where(x => x.IsCursed).ToList());

        return cards;
    }
    public List<S_Card> GetRandomCardsInDeck(int count = 999, S_CardTypeEnum type = S_CardTypeEnum.None)
    {
        List<S_Card> cards = pCard.GetDeckCards();

        if (type != S_CardTypeEnum.None)
        {
            cards = cards.Where(x => x.CardType == type).ToList();
        }

        return cards.OrderBy(x => Random.value).Take(count).ToList();
    }
    public List<S_Card> GetRandomCardsInStack(int count = 999, S_CardTypeEnum type = S_CardTypeEnum.None)
    {
        List<S_Card> cards = pCard.GetStackCards();

        if (type != S_CardTypeEnum.None)
        {
            cards = cards.Where(x => x.CardType == type).ToList();
        }

        return cards.OrderBy(x => Random.value).Take(count).ToList();
    }
    #endregion
    #region 특정 카드 무게 합 반환
    public int GetSameTypeSumInStack(S_CardTypeEnum suit) // 같은 타입 카드의 무게 합 반환 메서드
    {
        return pCard.GetStackCards().Where(x => IsSameType(x.CardType, suit)).Sum(x => x.Num);
    }
    public int GetSumInStack() // 카드의 무게 합 반환 메서드
    {
        return pCard.GetStackCards().Sum(x => x.Num);
    }
    #endregion
    #region 특정 카드 타입 반환
    public List<S_CardTypeEnum> GetGrandChaosInStack(int amount) // 대혼돈 값 반환 메서드
    {
        List<S_Card> cards = pCard.GetStackCards();

        List<S_CardTypeEnum> results = new();
        List<S_CardTypeEnum> types = new() { S_CardTypeEnum.Str, S_CardTypeEnum.Mind, S_CardTypeEnum.Luck, S_CardTypeEnum.Common };

        foreach (var type in types)
        {
            if (cards.Where(x => IsSameType(x.CardType, type)).Count() >= amount)
            {
                results.Add(type);
            }
        }

        return results;
    }
    public List<S_CardTypeEnum> GetGrandChaosInStackInCurrentTurn(int amount) // 대혼돈 값 반환 메서드(단 한 턴에)
    {
        List<S_Card> cards = pCard.GetStackCards();

        List<S_CardTypeEnum> results = new();
        List<S_CardTypeEnum> types = new() { S_CardTypeEnum.Str, S_CardTypeEnum.Mind, S_CardTypeEnum.Luck, S_CardTypeEnum.Common };

        foreach (var type in types)
        {
            if (cards.Where(x => x.IsCurrentTurn).Where(x => IsSameType(x.CardType, type)).Count() >= amount)
            {
                results.Add(type);
            }
        }

        return results;
    }
    #endregion
    #region 부가 메서드
    public List<S_Card> GetLeastTypeCardsInDeck(out S_CardTypeEnum type)
    {
        List<S_Card> decks = pCard.GetDeckCards();

        // 덱이 비어있다면 기본값과 빈 리스트 반환
        if (decks.Count == 0)
        {
            type = S_CardTypeEnum.None; // 기본값
            return new List<S_Card>();
        }

        // 문양별 그룹핑 후 개수 적은 순 정렬
        var grouped = decks
            .GroupBy(x => x.CardType)
            .OrderBy(g => g.Count())
            .ToList();

        int minCount = grouped.First().Count();

        // 최소 개수 그룹 중 무작위 선택
        var leastTypeGroups = grouped.Where(g => g.Count() == minCount).ToList();
        var chosenGroup = leastTypeGroups[Random.Range(0, leastTypeGroups.Count)];

        type = chosenGroup.Key;
        return chosenGroup.ToList();
    }
    public List<S_Card> GetLeastTypeCardsInStack(out S_CardTypeEnum type)
    {
        List<S_Card> stacks = pCard.GetStackCards();

        // 덱이 비어있다면 기본값과 빈 리스트 반환
        if (stacks.Count == 0)
        {
            type = S_CardTypeEnum.None; // 기본값
            return new List<S_Card>();
        }

        // 문양별 그룹핑 후 개수 적은 순 정렬
        var grouped = stacks
            .GroupBy(x => x.CardType)
            .OrderBy(g => g.Count())
            .ToList();

        int minCount = grouped.First().Count();

        // 최소 개수 그룹 중 무작위 선택
        var leastTypeGroups = grouped.Where(g => g.Count() == minCount).ToList();
        var chosenGroup = leastTypeGroups[Random.Range(0, leastTypeGroups.Count)];

        type = chosenGroup.Key;
        return chosenGroup.ToList();
    }
    public List<S_Card> GetLeastNumberCardsInDeck(out int leastNumber)
    {
        List<S_Card> decks = pCard.GetDeckCards();

        // 덱이 비어있다면 기본값과 빈 리스트 반환
        if (decks.Count == 0)
        {
            leastNumber = 1;
            return new List<S_Card>();
        }

        var grouped = decks
            .GroupBy(x => x.Num)
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
    public S_BattleStatEnum GetHighestStats(out int value) // 가장 높은 능력치
    {
        var stats = new (S_BattleStatEnum stat, int val)[]
        {
            (S_BattleStatEnum.Str, pStat.CurrentStr),
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
    public S_BattleStatEnum GetRandomStat(out int value) // 무작위 능력치
    {
        var stats = new (S_BattleStatEnum stat, int val)[]
        {
            (S_BattleStatEnum.Str, pStat.CurrentStr),
            (S_BattleStatEnum.Mind, pStat.CurrentMind),
            (S_BattleStatEnum.Luck, pStat.CurrentLuck)
        };

        var selected = stats[Random.Range(0, stats.Length)];

        value = selected.val;
        return selected.stat;
    }
    #endregion
}
