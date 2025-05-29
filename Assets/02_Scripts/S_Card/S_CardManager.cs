using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_CardManager : MonoBehaviour
{
    [Header("ī�� ���� ����")]
    int idCount = 0;

    // �̱���
    static S_CardManager instance;
    public static S_CardManager Instance { get { return instance; } }

    void Awake()
    {
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

    public List<S_Card> GenerateCardByStartGame() // ���� ���� �� ���� �� ����
    {
        List<S_Card> cardList = new();

        // ���� ���� 1���� 10������ ����, �� ������ 10�徿 ����, ������ �� ������ �ɷ�ġ ������ ���� ī���
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
    public S_Card GenerateRandomCard(int cardNumber = -1, S_CardSuitEnum cardSuit = S_CardSuitEnum.Random) // ������ ī�� ����
    {
        int number;
        if (cardNumber == -1) number = Random.Range(1, 11);
        else number = cardNumber;

        S_CardSuitEnum suit;
        if (cardSuit == S_CardSuitEnum.Random) suit = GetRandomSuit();
        else suit = cardSuit;

        S_BattleStatEnum stat = GetNumberStatValue(suit); // Ŭ�ι��� ��� ������ �������� ���ϱ�

        S_CardBasicConditionEnum basicCondition = GetRandomBasicCondition();
        S_CardAdditiveConditionEnum additiveCondition = GetRandomAdditiveCondition(basicCondition);
        S_CardDebuffConditionEnum debuffCondition = GetRandomDebuffCondition(basicCondition);
        S_CardBasicEffectEnum basicEffect = GetEffectByConditionWeights(basicCondition, additiveCondition, debuffCondition, out S_CardAdditiveEffectEnum additiveEffect);
        S_Card card = new S_Card(idCount, number, suit, stat, basicCondition, additiveCondition, debuffCondition, basicEffect, additiveEffect);

        idCount++;

        return card;
    }
    S_CardSuitEnum GetRandomSuit() // ������ ���� ����
    {
        List<S_CardSuitEnum> list = new List<S_CardSuitEnum> { S_CardSuitEnum.Spade, S_CardSuitEnum.Heart, S_CardSuitEnum.Diamond, S_CardSuitEnum.Clover };

        int randomIndex = Random.Range(0, list.Count);

        return list[randomIndex];
    }
    S_BattleStatEnum GetNumberStatValue(S_CardSuitEnum suit) // ���翡 ���� ���� �ɷ�ġ ����
    {
        S_BattleStatEnum stat = S_BattleStatEnum.Strength;

        switch (suit)
        {
            case S_CardSuitEnum.Spade: stat = S_BattleStatEnum.Strength; break;
            case S_CardSuitEnum.Heart: stat = S_BattleStatEnum.Mind; break;
            case S_CardSuitEnum.Diamond: stat = S_BattleStatEnum.Luck; break;
            case S_CardSuitEnum.Clover: stat = S_BattleStatEnum.Random; break;
                //float randomValue = Random.Range(0f, 1f);
                //if (randomValue < 0.33333f) // �� Ȯ��
                //{
                //    stat = S_BattleStatEnum.Strength; break;
                //}
                //else if (randomValue < 0.66666f) // ���ŷ� Ȯ��
                //{
                //    stat = S_BattleStatEnum.Mind; break;
                //}
                //else // ��� Ȯ��
                //{
                //    stat = S_BattleStatEnum.Luck; break;
                //}
        }

        return stat;
    }
    S_CardBasicConditionEnum GetRandomBasicCondition() // �⺻ ���� ����
    {
        S_CardBasicConditionEnum[] array = System.Enum.GetValues(typeof(S_CardBasicConditionEnum))
            .Cast<S_CardBasicConditionEnum>()
            .Where(x => x != S_CardBasicConditionEnum.None)
            .ToArray();

        int randomIndex = Random.Range(0, array.Length);

        return array[randomIndex];
    }
    S_CardAdditiveConditionEnum GetRandomAdditiveCondition(S_CardBasicConditionEnum basicCondition) // �߰� ���� ����
    {
        float randomF = Random.Range(0f, 1f);

        if (randomF > 0.5f) // 50% Ȯ���� �߰� ���� �� ����.
        {
            return S_CardAdditiveConditionEnum.None;
        }
        else // 50% Ȯ���� �⺻ ���ǿ� ���� �߰� ������ ����.
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
    S_CardDebuffConditionEnum GetRandomDebuffCondition(S_CardBasicConditionEnum basicCondition) // ����� ����
    {
        float randomF = Random.Range(0f, 1f);

        if (randomF > 0.25f) // 75% Ȯ���� ����� �� ����.
        {
            return S_CardDebuffConditionEnum.None;
        }
        else // 25% Ȯ���� �⺻ ���ǿ� ���� ����� ����.
        {
            S_CardDebuffConditionEnum[] array = System.Enum.GetValues(typeof(S_CardDebuffConditionEnum))
              .Cast<S_CardDebuffConditionEnum>()
              .Where(x => x != S_CardDebuffConditionEnum.None)
              .ToArray();

            int randomIndex = Random.Range(0, array.Length);
            return array[randomIndex];
        }
    }
    S_CardBasicEffectEnum GetEffectByConditionWeights(S_CardBasicConditionEnum basicCondition, S_CardAdditiveConditionEnum additiveCondition, S_CardDebuffConditionEnum debuffCondition, out S_CardAdditiveEffectEnum additiveEffect) // ȿ�� ����
    {
        int conditionWeights = S_CardEffectMetadata.GetWeights(basicCondition) + S_CardEffectMetadata.GetWeights(additiveCondition) + S_CardEffectMetadata.GetWeights(debuffCondition);

        // �⺻ ȿ�� ���� ��������
        List<S_CardBasicEffectEnum> effects = new();
        for (int i = 1; i <= conditionWeights; i++)
        {
            effects.AddRange(S_CardEffectMetadata.GetBasicEffect(i));
        }
        
        // ���Ƕ�� ���۷��� ���� �ʰ� ���ֱ�. â���� �ε��� ���� �ȵ� �� ������ ����
        if (basicCondition == S_CardBasicConditionEnum.Resolve)
        {
            effects = effects.Where(x => 
                x != S_CardBasicEffectEnum.Manipulation && 
                x != S_CardBasicEffectEnum.Manipulation_Cheat)
                .ToList();
        }

        float randomF = Random.Range(0f, 1f);
        if (randomF > 0.7f) // 30% Ȯ���� �߰� ȿ�� ���� ���
        {
            // ȿ���� ����ġ ������ ����
            effects = effects
                .OrderByDescending(x => S_CardEffectMetadata.GetWeights(x))
                .ThenBy(x => new System.Random().Next())
                .ToList();

            // ����ġ�� ���� ������ ȿ�� 3�� �̱�
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

            // �߰� ȿ���� ����Ʈ ó��
            additiveEffect = default;

            return e[Random.Range(0, effects.Count)];
        }
        else // 70% Ȯ���� �߰� ȿ�� �޸��� ���
        {
            S_CardBasicEffectEnum basic = effects[Random.Range(0, effects.Count)];

            // �߰� ȿ���� ���� ����ġ �μ� ������ ����
            int newWeights = conditionWeights - S_CardEffectMetadata.GetWeights(basic);

            List<S_CardAdditiveEffectEnum> additives = new();
            for (int i = 1; i <= newWeights; i++)
            {
                additives.AddRange(S_CardEffectMetadata.GetAdditiveEffect(i));
            }

            if (additives.Count > 0) // �߰� ȿ���� �ִٸ�
            {
                additiveEffect = additives[Random.Range(0, additives.Count)];
            }
            else // ���ٸ� �߰� ȿ���� ����Ʈ ó��
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