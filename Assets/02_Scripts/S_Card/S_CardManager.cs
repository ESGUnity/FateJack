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
                S_BattleStatEnum stat = GetBattleStat(suit);

                List<S_CardBasicEffectEnum> basic = new() { S_CardBasicEffectEnum.Growth_Strength, S_CardBasicEffectEnum.Growth_Mind, S_CardBasicEffectEnum.Growth_Luck };
                S_Card card = new S_Card(idCount, i, suit, stat, S_CardBasicConditionEnum.Unleash, S_CardAdditiveConditionEnum.None, S_CardDebuffConditionEnum.None, basic[Random.Range(0, basic.Count)], S_CardAdditiveEffectEnum.None);
                cardList.Add(card);

                idCount++;
                //cardList.Add(GenerateRandomCard());
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

        S_BattleStatEnum stat = GetBattleStat(suit); // 클로버일 경우 무작위 스탯으로 정하기

        S_CardBasicConditionEnum basicCondition = GetRandomBasicCondition();
        S_CardAdditiveConditionEnum additiveCondition = GetRandomAdditiveCondition(basicCondition);
        S_CardDebuffConditionEnum debuffCondition = GetRandomDebuffCondition();
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
    S_BattleStatEnum GetBattleStat(S_CardSuitEnum suit) // 문양에 따른 전투 능력치 설정
    {
        S_BattleStatEnum stat = S_BattleStatEnum.Strength;

        switch (suit)
        {
            case S_CardSuitEnum.Spade: stat = S_BattleStatEnum.Strength; break;
            case S_CardSuitEnum.Heart: stat = S_BattleStatEnum.Mind; break;
            case S_CardSuitEnum.Diamond: stat = S_BattleStatEnum.Luck; break;
            case S_CardSuitEnum.Clover: stat = S_BattleStatEnum.Random; break;
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
    S_CardDebuffConditionEnum GetRandomDebuffCondition() // 디버프 설정
    {
        float randomF = Random.Range(0f, 1f);

        if (randomF > 0.25f) // 75% 확률로 디버프 안 붙음.
        {
            return S_CardDebuffConditionEnum.None;
        }
        else // 25% 확률로 디버프 붙음.
        {
            S_CardDebuffConditionEnum[] array = System.Enum.GetValues(typeof(S_CardDebuffConditionEnum))
              .Cast<S_CardDebuffConditionEnum>()
              .Where(x => x != S_CardDebuffConditionEnum.None)
              .ToArray();

            int randomIndex = Random.Range(0, array.Length);
            return array[randomIndex];
        }
    }
    S_CardBasicEffectEnum GetEffectByConditionWeights(S_CardBasicConditionEnum basicCondition, S_CardAdditiveConditionEnum additiveCondition, S_CardDebuffConditionEnum debuffCondition, out S_CardAdditiveEffectEnum additiveEffect)
    {
        additiveEffect = S_CardAdditiveEffectEnum.None;

        // 조건 가중치
        int conditionWeights = S_CardEffectMetadata.GetWeights(basicCondition) + S_CardEffectMetadata.GetWeights(additiveCondition) + S_CardEffectMetadata.GetWeights(debuffCondition);

        // 만족하는 후보들
        var validPairs = new List<(S_CardBasicEffectEnum basic, S_CardAdditiveEffectEnum additive, int totalWeight)>();

        // 밸류
        var basicValues = (S_CardBasicEffectEnum[])System.Enum.GetValues(typeof(S_CardBasicEffectEnum));
        var additiveValues = (S_CardAdditiveEffectEnum[])System.Enum.GetValues(typeof(S_CardAdditiveEffectEnum));

        foreach (S_CardBasicEffectEnum basic in basicValues)
        {
            if (basic == S_CardBasicEffectEnum.None) continue;

            // 결의 조건이면 조작류 제외
            if (basicCondition == S_CardBasicConditionEnum.Resolve &&
                (basic == S_CardBasicEffectEnum.Manipulation || basic == S_CardBasicEffectEnum.Manipulation_CardNumber))
            {
                continue;
            }

            // 메아리 조건이면 생성 및 인도류 제외
            if (basicCondition == S_CardBasicConditionEnum.Reverb)
            {
                if (basic == S_CardBasicEffectEnum.Creation_Random || basic == S_CardBasicEffectEnum.Creation_SameSuit || basic == S_CardBasicEffectEnum.Creation_SameNumber || basic == S_CardBasicEffectEnum.Creation_PlethoraNumber ||
                    basic == S_CardBasicEffectEnum.Guidance_LeastSuit || basic == S_CardBasicEffectEnum.Guidance_LeastNumber)
                {
                    continue;
                }
            }

            int basicWeight = S_CardEffectMetadata.GetWeights(basic);

            if (Random.value < 0.3f) // 확률 분기 (추가 효과 없음)
            {
                if (basicWeight <= conditionWeights)
                {
                    validPairs.Add((basic, S_CardAdditiveEffectEnum.None, basicWeight));
                }
            }
            else // 70% 확률로 추가 효과 있음
            {
                foreach (S_CardAdditiveEffectEnum additive in additiveValues)
                {
                    int additiveWeight = S_CardEffectMetadata.GetWeights(additive);
                    int total = basicWeight + additiveWeight;

                    if (total <= conditionWeights)
                    {
                        validPairs.Add((basic, additive, total));
                    }
                }
            }
        }

        // fallback 시
        if (validPairs.Count == 0)
        {
            foreach (var basic in basicValues)
            {
                if (basic == S_CardBasicEffectEnum.None) continue;

                if (basicCondition == S_CardBasicConditionEnum.Resolve &&
                    (basic == S_CardBasicEffectEnum.Manipulation || basic == S_CardBasicEffectEnum.Manipulation_CardNumber))
                {
                    continue;
                }

                int basicWeight = S_CardEffectMetadata.GetWeights(basic);
                validPairs.Add((basic, S_CardAdditiveEffectEnum.None, basicWeight));
            }
        }

        // 가중치 계산 및 선택
        int totalWeightSum = 0;
        var weightedPairs = new List<(S_CardBasicEffectEnum basic, S_CardAdditiveEffectEnum additive, int weight)>();

        foreach (var pair in validPairs)
        {
            int weight = Mathf.Max(1, 100 - (conditionWeights - pair.totalWeight) * 10);
            weightedPairs.Add((pair.basic, pair.additive, weight));
            totalWeightSum += weight;
        }

        int rand = Random.Range(0, totalWeightSum);
        int current = 0;
        foreach (var item in weightedPairs)
        {
            current += item.weight;
            if (rand < current)
            {
                additiveEffect = item.additive;
                return item.basic;
            }
        }

        // 최악의 fallback (이론상 발생 안함)
        var first = weightedPairs[0];
        additiveEffect = first.additive;
        return first.basic;
    }
    #region 상품에 의한 카드 효과 변경
    public void ChangeAllProperty(S_Card card) // 카드의 모든 속성을 바꿉니다.
    {
        ChangeAnotherSuit(card);
        card.StatValue = GetBattleStat(card.Suit);
        ChangeAnotherNumber(card);
        card.BasicCondition = GetRandomBasicCondition();
        card.AdditiveCondition = GetRandomAdditiveCondition(card.BasicCondition);
        card.Debuff = GetRandomDebuffCondition();
        card.BasicEffect = GetEffectByConditionWeights(card.BasicCondition, card.AdditiveCondition, card.Debuff, out S_CardAdditiveEffectEnum additiveEffect);
        card.AdditiveEffect = additiveEffect;
    }
    public void ChangeAnotherSuit(S_Card card)
    {
        List<S_CardSuitEnum> list = new List<S_CardSuitEnum> { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };
        list = list.Where(x => x != card.Suit).ToList();

        int randomIndex = Random.Range(0, list.Count);

        card.Suit = list[randomIndex];
        card.StatValue = GetBattleStat(card.Suit);
    }
    public void ChangeAnotherNumber(S_Card card)
    {
        List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        list = list.Where(x => x != card.Number).ToList();

        int randomIndex = Random.Range(0, list.Count);

        card.Number = list[randomIndex];
    }
    public void ChangeAllCondition(S_Card card) // 조건 가중치가 효과 가중치보다 같거나 크게 조정됨.
    {
        // 1. 기본 조건 무작위 선택
        S_CardBasicConditionEnum[] basicOptions = System.Enum.GetValues(typeof(S_CardBasicConditionEnum))
            .Cast<S_CardBasicConditionEnum>()
            .Where(x => x != S_CardBasicConditionEnum.None)
            .ToArray();

        // 특정 효과 조합에 따른 제외 조건
        if (card.BasicEffect == S_CardBasicEffectEnum.Manipulation || card.BasicEffect == S_CardBasicEffectEnum.Manipulation_CardNumber)
        {
            basicOptions = basicOptions.Where(x => x != S_CardBasicConditionEnum.Resolve).ToArray();
        }
        // 생성 및 인도류 효과면 메아리 제외
        if (card.BasicEffect == S_CardBasicEffectEnum.Creation_Random || card.BasicEffect == S_CardBasicEffectEnum.Creation_SameSuit || card.BasicEffect == S_CardBasicEffectEnum.Creation_SameNumber || card.BasicEffect == S_CardBasicEffectEnum.Creation_PlethoraNumber ||
                card.BasicEffect == S_CardBasicEffectEnum.Guidance_LeastSuit || card.BasicEffect == S_CardBasicEffectEnum.Guidance_LeastNumber)
        {
            basicOptions = basicOptions.Where(x => x != S_CardBasicConditionEnum.Reverb).ToArray();
        }

        card.BasicCondition = basicOptions[Random.Range(0, basicOptions.Length)];
        int basicWeight = S_CardEffectMetadata.GetWeights(card.BasicCondition);
        int effectWeight = S_CardEffectMetadata.GetWeights(card.BasicEffect) + S_CardEffectMetadata.GetWeights(card.AdditiveEffect);

        // 2. 후보 생성
        var additiveOptions = System.Enum.GetValues(typeof(S_CardAdditiveConditionEnum)).Cast<S_CardAdditiveConditionEnum>();
        var debuffOptions = System.Enum.GetValues(typeof(S_CardDebuffConditionEnum)).Cast<S_CardDebuffConditionEnum>();

        if (card.BasicCondition != S_CardBasicConditionEnum.Reverb)
        {
            additiveOptions = additiveOptions.Where(x =>
                x != S_CardAdditiveConditionEnum.Reverb_SameSuit &&
                x != S_CardAdditiveConditionEnum.Reverb_SameNumber &&
                x != S_CardAdditiveConditionEnum.Reverb_PlethoraNumber &&
                x != S_CardAdditiveConditionEnum.Reverb_CursedCard
            );
        }

        bool hasAdditive = Random.value < 0.5f;
        bool hasDebuff = Random.value < 0.25f;

        var additiveCandidates = hasAdditive ? additiveOptions : new[] { S_CardAdditiveConditionEnum.None };
        var debuffCandidates = hasDebuff ? debuffOptions : new[] { S_CardDebuffConditionEnum.None };

        List<(S_CardAdditiveConditionEnum additive, S_CardDebuffConditionEnum debuff, int totalWeight)> validCandidates = new();

        foreach (var add in additiveCandidates)
        {
            int addWeight = S_CardEffectMetadata.GetWeights(add);
            foreach (var deb in debuffCandidates)
            {
                int debWeight = S_CardEffectMetadata.GetWeights(deb);
                int total = basicWeight + addWeight + debWeight;

                if (total >= effectWeight)
                {
                    validCandidates.Add((add, deb, total));
                }
            }
        }

        // 3. 예외 처리: 조건 가중치를 만족하는 조합이 없을 경우 → 강제로 조합 생성
        if (validCandidates.Count == 0)
        {
            // 모든 조합 중 조건 가중치가 가장 높은 것 선택
            var forcedCandidates = new List<(S_CardAdditiveConditionEnum additive, S_CardDebuffConditionEnum debuff, int totalWeight)>();

            foreach (var add in additiveOptions)
            {
                int addWeight = S_CardEffectMetadata.GetWeights(add);
                foreach (var deb in debuffOptions)
                {
                    int debWeight = S_CardEffectMetadata.GetWeights(deb);
                    int total = basicWeight + addWeight + debWeight;

                    if (total >= effectWeight)
                    {
                        forcedCandidates.Add((add, deb, total));
                    }
                }
            }

            // 가중치 차이 최소 조합 선택
            var closest = forcedCandidates.OrderBy(x => Mathf.Abs(x.totalWeight - effectWeight)).First();
            card.AdditiveCondition = closest.additive;
            card.Debuff = closest.debuff;
            return;
        }

        // 4. 가중치 기반 선택
        var weightedList = validCandidates.Select(x => new
        {
            x.additive,
            x.debuff,
            weight = Mathf.Max(1, 100 - (x.totalWeight - effectWeight) * 10)
        }).ToList();

        int totalWeightSum = weightedList.Sum(x => x.weight);
        int rand = Random.Range(0, totalWeightSum);
        int current = 0;

        foreach (var item in weightedList)
        {
            current += item.weight;
            if (rand < current)
            {
                card.AdditiveCondition = item.additive;
                card.Debuff = item.debuff;
                return;
            }
        }

        // fallback
        card.AdditiveCondition = weightedList[0].additive;
        card.Debuff = weightedList[0].debuff;
    }
    public void ChangeAllEffect(S_Card card) // 효과 가중치가 조건 가중치보다 같거나 낮게 조정됨
    {
        card.BasicEffect = GetEffectByConditionWeights(card.BasicCondition, card.AdditiveCondition, card.Debuff, out card.AdditiveEffect);
    }
    public void ChangeBasicCondition(S_Card card)
    {
        card.BasicCondition = GetRandomBasicCondition();
    }
    public void ChangeAdditiveCondition(S_Card card) // 마지막에 총 효과를 바꾸니 굳이 결의나 메아리에 따른 조건 제약 제거 불필요
    {
        S_CardAdditiveConditionEnum[] array = System.Enum.GetValues(typeof(S_CardAdditiveConditionEnum))
            .Cast<S_CardAdditiveConditionEnum>()
            //.Where(x => x != S_CardAdditiveConditionEnum.None)
            .ToArray();

        int randomIndex = Random.Range(0, array.Length);
        card.AdditiveCondition = array[randomIndex];
    }
    public void ChangedDebuffCondition(S_Card card)
    {
        S_CardDebuffConditionEnum[] array = System.Enum.GetValues(typeof(S_CardDebuffConditionEnum))
            .Cast<S_CardDebuffConditionEnum>()
            .ToArray();

        int randomIndex = Random.Range(0, array.Length);
        card.Debuff = array[randomIndex];
    }
    public void AddDebuffCondition(S_Card card)
    {
        S_CardDebuffConditionEnum[] array = System.Enum.GetValues(typeof(S_CardDebuffConditionEnum))
            .Cast<S_CardDebuffConditionEnum>()
            .Where(x => x != S_CardDebuffConditionEnum.None)
            .ToArray();

        int randomIndex = Random.Range(0, array.Length);
        card.Debuff = array[randomIndex];
    }
    public void DeleteDebuffCondition(S_Card card)
    {
        card.Debuff = S_CardDebuffConditionEnum.None;
    }
    public void ChangeBasicEffect(S_Card card) // 마지막에 총 컨디션을 바꾸니 굳이 결의나 메아리에 따른 효과 제거 불필요
    {
        S_CardBasicEffectEnum[] array = System.Enum.GetValues(typeof(S_CardBasicEffectEnum))
            .Cast<S_CardBasicEffectEnum>()
            .Where(x => x != S_CardBasicEffectEnum.None)
            .ToArray();

        card.BasicEffect = array[Random.Range(0, array.Length)];
    }
    public void ChangeAdditiveEffect(S_Card card)
    {
        S_CardAdditiveEffectEnum[] array = System.Enum.GetValues(typeof(S_CardAdditiveEffectEnum))
            .Cast<S_CardAdditiveEffectEnum>()
            //.Where(x => x != S_CardAdditiveEffectEnum.None)
            .ToArray();

        card.AdditiveEffect = array[Random.Range(0, array.Length)];
    }
    #endregion
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
    Delusion,
    Spell,
    Rebel,
}
public enum S_CardBasicEffectEnum
{
    None,
    Growth_Strength,
    Growth_Mind,
    Growth_Luck,
    Growth_AllStat,
    Break_MostStat,
    Break_RandomStat,
    Manipulation,
    Manipulation_CardNumber,
    Manipulation_CleanHit,
    Resistance,
    Resistance_CardNumber,
    Harm_Strength,
    Harm_Mind,
    Harm_Luck,
    Harm_StrengthAndMind,
    Harm_StrengthAndLuck,
    Harm_MindAndLuck,
    Harm_Carnage,
    Tempering,
    Plunder,
    Plunder_Break,
    Creation_Random,
    Creation_SameSuit,
    Creation_SameNumber,
    Creation_PlethoraNumber,
    Expansion,
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