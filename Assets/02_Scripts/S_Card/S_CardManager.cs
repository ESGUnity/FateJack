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

    public List<S_Card> GenerateCardByStartGame() // 게임 시작 시 최초 덱 생성
    {
        List<S_Card> cardList = new();

        // 최초 덱은 1부터 10까지의 숫자, 각 문양이 10장씩 존재, 발현에 각 무작위 능력치 증가가 붙은 카드들
        for (int j = 1; j < 5; j++)
        {
            for (int i = 1; i < 11; i++)
            {
                S_CardSuitEnum suit = (S_CardSuitEnum)j;
                S_BattleStatEnum stat = GetNumberStatValue(suit);

                List<S_CardBasicEffectEnum> basic = new() { S_CardBasicEffectEnum.Increase_Strength, S_CardBasicEffectEnum.Increase_Mind, S_CardBasicEffectEnum.Increase_Luck };
                S_Card card = new S_Card(idCount, i, suit, stat, S_CardBasicConditionEnum.Unleash, S_CardAdditiveConditionEnum.None, S_CardDebuffConditionEnum.None, basic[Random.Range(0, basic.Count)], S_CardAdditiveEffectEnum.None);
                cardList.Add(card);

                idCount++;
            }
        }
        
        return cardList;
    }
    public S_Card GenerateRandomCard(int cardNumber = -1, S_CardSuitEnum cardSuit = S_CardSuitEnum.Random) // 무작위 카드 생성
    {
        int number;
        if (cardNumber == -1) number = Random.Range(1, 11);
        else number = cardNumber;

        S_CardSuitEnum suit;
        if (cardSuit == S_CardSuitEnum.Random) suit = GetRandomSuit();
        else suit = cardSuit;

        S_BattleStatEnum stat = GetNumberStatValue(suit); // 클로버일 경우 무작위 스탯으로 정하기

        S_CardBasicConditionEnum basicCondition = GetRandomBasicCondition();
        S_CardAdditiveConditionEnum additiveCondition = GetRandomAdditiveCondition(basicCondition);
        S_CardDebuffConditionEnum debuffCondition = GetRandomDebuffCondition(basicCondition);
        S_CardBasicEffectEnum basicEffect = GetEffectByConditionWeights(basicCondition, additiveCondition, debuffCondition, out S_CardAdditiveEffectEnum additiveEffect);
        S_Card card = new S_Card(idCount, number, suit, stat, basicCondition, additiveCondition, debuffCondition, basicEffect, additiveEffect);

        idCount++;

        return card;
    }
    S_CardSuitEnum GetRandomSuit() // 무작위 문양 설정
    {
        List<S_CardSuitEnum> list = new List<S_CardSuitEnum> { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };

        int randomIndex = Random.Range(0, list.Count);

        return list[randomIndex];
    }
    S_BattleStatEnum GetNumberStatValue(S_CardSuitEnum suit) // 문양에 따른 전투 능력치 설정
    {
        S_BattleStatEnum stat = S_BattleStatEnum.Strength;

        switch (suit)
        {
            case S_CardSuitEnum.Spade: stat = S_BattleStatEnum.Strength; break;
            case S_CardSuitEnum.Heart: stat = S_BattleStatEnum.Mind; break;
            case S_CardSuitEnum.Diamond: stat = S_BattleStatEnum.Luck; break;
            case S_CardSuitEnum.Clover: stat = S_BattleStatEnum.Random; break;
                //float randomValue = Random.Range(0f, 1f);
                //if (randomValue < 0.33333f) // 힘 확률
                //{
                //    stat = S_BattleStatEnum.Strength; break;
                //}
                //else if (randomValue < 0.66666f) // 정신력 확률
                //{
                //    stat = S_BattleStatEnum.Mind; break;
                //}
                //else // 행운 확률
                //{
                //    stat = S_BattleStatEnum.Luck; break;
                //}
        }

        return stat;
    }
    S_CardBasicConditionEnum GetRandomBasicCondition() // 기본 조건 설정
    {
        S_CardBasicConditionEnum[] array = System.Enum.GetValues(typeof(S_CardBasicConditionEnum))
            .Cast<S_CardBasicConditionEnum>()
            .Where(x => x != S_CardBasicConditionEnum.None)
            .ToArray();

        int randomIndex = Random.Range(0, array.Length);

        return array[randomIndex];
    }
    S_CardAdditiveConditionEnum GetRandomAdditiveCondition(S_CardBasicConditionEnum basicCondition) // 추가 조건 설정
    {
        float randomF = Random.Range(0f, 1f);

        if (randomF > 0.5f) // 50% 확률로 추가 조건 안 붙음.
        {
            return S_CardAdditiveConditionEnum.None;
        }
        else // 50% 확률로 기본 조건에 따라 추가 조건이 붙음.
        {
            S_CardAdditiveConditionEnum[] array = System.Enum.GetValues(typeof(S_CardAdditiveConditionEnum))
              .Cast<S_CardAdditiveConditionEnum>()
              .Where(x => x != S_CardAdditiveConditionEnum.None)
              .ToArray();

            switch (basicCondition)
            {
                case S_CardBasicConditionEnum.Reverb:
                    break;
                case S_CardBasicConditionEnum.Resolve:
                    array = array
                        .Where( x=>
                        x != S_CardAdditiveConditionEnum.Reverb_SameSuit &&
                        x != S_CardAdditiveConditionEnum.Reverb_SameNumber &&
                        x != S_CardAdditiveConditionEnum.Reverb_PlethoraNumber &&
                        x != S_CardAdditiveConditionEnum.Reverb_CursedCard)
                        .ToArray();
                    break;
                case S_CardBasicConditionEnum.Unleash:
                    array = array
                        .Where(x =>
                        x != S_CardAdditiveConditionEnum.Reverb_SameSuit &&
                        x != S_CardAdditiveConditionEnum.Reverb_SameNumber &&
                        x != S_CardAdditiveConditionEnum.Reverb_PlethoraNumber &&
                        x != S_CardAdditiveConditionEnum.Reverb_CursedCard)
                        .ToArray();
                    break;
                default:
                    break;
            }

            int randomIndex = Random.Range(0, array.Length);
            return array[randomIndex];
        }
    }
    S_CardDebuffConditionEnum GetRandomDebuffCondition(S_CardBasicConditionEnum basicCondition) // 디버프 설정
    {
        float randomF = Random.Range(0f, 1f);

        if (randomF > 0.25f) // 75% 확률로 디버프 안 붙음.
        {
            return S_CardDebuffConditionEnum.None;
        }
        else // 25% 확률로 기본 조건에 따른 디버프 붙음.
        {
            S_CardDebuffConditionEnum[] array = System.Enum.GetValues(typeof(S_CardDebuffConditionEnum))
              .Cast<S_CardDebuffConditionEnum>()
              .Where(x => x != S_CardDebuffConditionEnum.None)
              .ToArray();

            int randomIndex = Random.Range(0, array.Length);
            return array[randomIndex];
        }
    }
    S_CardBasicEffectEnum GetEffectByConditionWeights(S_CardBasicConditionEnum basicCondition, S_CardAdditiveConditionEnum additiveCondition, S_CardDebuffConditionEnum debuffCondition, out S_CardAdditiveEffectEnum additiveEffect) // 효과 설정
    {
        int conditionWeights = S_CardEffectMetadata.GetWeights(basicCondition) + S_CardEffectMetadata.GetWeights(additiveCondition) + S_CardEffectMetadata.GetWeights(debuffCondition);

        // 기본 효과 먼저 가져오기
        List<S_CardBasicEffectEnum> effects = new();
        for (int i = 1; i <= conditionWeights; i++)
        {
            effects.AddRange(S_CardEffectMetadata.GetBasicEffect(i));
        }
        
        // 결의라면 조작류는 붙지 않게 빼주기. 창조랑 인도도 구현 안될 거 같으면 빼기
        if (basicCondition == S_CardBasicConditionEnum.Resolve)
        {
            effects = effects.Where(x => 
                x != S_CardBasicEffectEnum.Manipulation && 
                x != S_CardBasicEffectEnum.Manipulation_Cheat)
                .ToList();
        }

        float randomF = Random.Range(0f, 1f);
        if (randomF > 0.7f) // 30% 확률로 추가 효과 없는 경우
        {
            // 효과를 가중치 순서로 정렬
            effects = effects
                .OrderByDescending(x => S_CardEffectMetadata.GetWeights(x))
                .ThenBy(x => new System.Random().Next())
                .ToList();

            // 가중치가 높은 순서로 효과 3개 뽑기
            List<S_CardBasicEffectEnum> e = new();
            if (effects.Count >= 3)
            {
                for (int i = 0; i < 5; i++)
                {
                    e.Add(effects[0]);
                }
                for (int i = 0; i < 3; i++)
                {
                    e.Add(effects[1]);
                }
                for (int i = 0; i < 2; i++)
                {
                    e.Add(effects[2]);
                }
            }

            // 추가 효과는 디폴트 처리
            additiveEffect = default;

            return e[Random.Range(0, effects.Count)];
        }
        else // 70% 확률로 추가 효과 달리는 경우
        {
            S_CardBasicEffectEnum basic = effects[Random.Range(0, effects.Count)];

            // 추가 효과를 위한 가중치 인수 별도로 생성
            int newWeights = conditionWeights - S_CardEffectMetadata.GetWeights(basic);

            List<S_CardAdditiveEffectEnum> additives = new();
            for (int i = 1; i <= newWeights; i++)
            {
                additives.AddRange(S_CardEffectMetadata.GetAdditiveEffect(i));
            }

            if (additives.Count > 0) // 추가 효과가 있다면
            {
                additiveEffect = additives[Random.Range(0, additives.Count)];
            }
            else // 없다면 추가 효과는 디폴트 처리
            {
                additiveEffect = default;
            }

            return basic;
        }
    }
}

public enum S_CardSuitEnum
{
    None,
    Spade,
    Heart,
    Diamond,
    Clover,
    Random
}
public enum S_BattleStatEnum
{
    None,
    Strength,
    Mind,
    Luck,
    Random,
    Strength_Mind,
    Strength_Luck,
    Mind_Luck,
    AllStat
}
public enum S_CardBasicConditionEnum
{
    None,
    Reverb,
    Resolve,
    Unleash,
}
public enum S_CardAdditiveConditionEnum
{
    None,
    Reverb_SameSuit,
    Reverb_SameNumber,
    Reverb_PlethoraNumber,
    Reverb_CursedCard,
    Legion_SameSuit,
    GreatLegion_SameSuit,
    Finale,
    Finale_Climax,
    Chaos,
    Chaos_Anarchy,
    GrandChaos_Anarchy,
    Chaos_Overflow,
    Offensive,
    Offensive_SameSuit,
    AllOutOffensive,
    AllOutOffensive_SameSuit,
    Offensive_Overflow,
    Precision_SameSuit,
    HyperPrecision_SameSuit,
    Precision_SameNumber,
    HyperPrecision_SameNumber,
    Precision_PlethoraNumber,
    HyperPrecision_PlethoraNumber,
    Overflow,
    Unity,
    Unity_Drastic
}
public enum S_CardDebuffConditionEnum
{
    None,
    Breakdown,
    Paranoia,
    Spell,
    Rebel,
}
public enum S_CardBasicEffectEnum
{
    None,
    Increase_Strength,
    Increase_Mind,
    Increase_Luck,
    Increase_AllStat,
    Break_Zenith,
    Break_Genesis,
    Manipulation,
    Manipulation_Cheat,
    Manipulation_Judge,
    Resistance,
    Resistance_Indomitable,
    Harm_Strength,
    Harm_Mind,
    Harm_Luck,
    Harm_StrengthAndMind,
    Harm_StrengthAndLuck,
    Harm_MindAndLuck,
    Harm_Carnage,
    Tempering,
    Plunder,
    Plunder_Raid,
    Creation,
    Creation_SameSuit,
    Creation_SameNumber,
    Creation_PlethoraNumber,
    AreaExpansion,
    First_SameSuit,
    First_LeastSuit,
    First_SameNumber,
    First_CleanHitNumber,
    Undertow,
    Guidance_LeastSuit,
    Guidance_LeastNumber
}
public enum S_CardAdditiveEffectEnum
{
    None,
    Reflux_Subtle,
    Reflux_Violent,
    Reflux_Shatter,
    Reflux_Stack,
    Reflux_PlethoraNumber,
    Reflux_Deck,
    Reflux_Chaos,
    Reflux_Offensive,
    Reflux_Curse,
    Reflux_Exclusion,
    Reflux_Overdrive,
    ColdBlood,
    Immunity,
    Omen,
    Robbery,
    Greed
}