using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class S_EffectChecker : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerCard pCard;
    S_PlayerStat pStat;

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
    }

    #region 특수
    public bool IsSameType(S_CardTypeEnum type1, S_CardTypeEnum type2)
    {
        bool isSameEach = false;

        bool sameStrCommon = pCard.GetPersistCardsInField(S_PersistEnum.IsSameType_StrCommon).Count > 0;
        bool sameMIndLuck = pCard.GetPersistCardsInField(S_PersistEnum.IsSameType_MindLuck).Count > 0;

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
    public S_StatEnum GetHighestStats(out int value) // 가장 높은 능력치
    {
        var stats = new (S_StatEnum stat, int val)[]
        {
            (S_StatEnum.Str, pStat.CurrentStr),
            (S_StatEnum.Mind, pStat.CurrentMind),
            (S_StatEnum.Luck, pStat.CurrentLuck)
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
    #endregion
    #region 특정 카드 가져오기
    public List<S_CardBase> GetSameTypeCardsInField(S_CardTypeEnum type) // 필드에 있는 type과 같은 타입의 카드 반환
    {
        return pCard.GetFieldCards().Where(x => IsSameType(x.CardType, type)).ToList();
    }
    public List<S_CardBase> GetAllCursedCards() // 덱, 사용한 카드 더미, 필드 포함 모든 저주받은 카드 반환
    {
        List<S_CardBase> cards = new();

        cards.AddRange(pCard.GetDeckCards().Where(x => x.IsCursed).ToList());
        cards.AddRange(pCard.GetUsedCards().Where(x => x.IsCursed).ToList());
        cards.AddRange(pCard.GetFieldCards().Where(x => x.IsCursed).ToList());

        return cards;
    }
    public List<S_CardBase> GetCursedCardsInField() // 필드에 있는 저주받은 카드 반환(저주 해제 용)
    {
        List<S_CardBase> cards = new();

        cards.AddRange(pCard.GetFieldCards().Where(x => x.IsCursed).ToList());

        return cards;
    }
    public List<S_CardBase> GetUncursedCardsInField(int count) // 필드에 있는 저주받지 않은 카드 반환
    {
        List<S_CardBase> cards = pCard.GetFieldCards().Where(x => !x.IsCursed).ToList();

        return cards.OrderBy(x => Random.value).Take(count).ToList();
    }
    public List<S_CardBase> GetRightCardsInField(S_CardBase card)
    {
        List<S_CardBase> result = new();

        int startIndex = pCard.GetFieldCards().IndexOf(card);
        if (startIndex == -1) return result; // 안전장치: card가 없으면 빈 리스트 반환

        for (int i = startIndex + 1; i < pCard.GetFieldCards().Count; i++) // 오른쪽 카드만 추가
        {
            result.Add(pCard.GetFieldCards()[i]);
        }

        return result;
    }
    #endregion
}
