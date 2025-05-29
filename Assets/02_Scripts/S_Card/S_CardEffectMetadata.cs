using System.Collections.Generic;
using System.Linq;

public static class S_CardEffectMetadata
{
    public static readonly Dictionary<S_CardBasicConditionEnum, int> BasicConditionWeights = new()
    {
        { S_CardBasicConditionEnum.None, 0 },
        { S_CardBasicConditionEnum.Reverb, 1 },
        { S_CardBasicConditionEnum.Resolve, 2 },
        { S_CardBasicConditionEnum.Unleash, 3 },
    };

    public static readonly Dictionary<S_CardAdditiveConditionEnum, int> AllAdditiveConditionWeights = new()
    {
        { S_CardAdditiveConditionEnum.None, 0 },
        { S_CardAdditiveConditionEnum.Reverb_SameSuit, 1 },
        { S_CardAdditiveConditionEnum.Reverb_SameNumber, 3 },
        { S_CardAdditiveConditionEnum.Reverb_PlethoraNumber, 2 },
        { S_CardAdditiveConditionEnum.Reverb_CursedCard, 2 },
        { S_CardAdditiveConditionEnum.Legion_SameSuit, 3 },
        { S_CardAdditiveConditionEnum.GreatLegion_SameSuit, 4 },
        { S_CardAdditiveConditionEnum.Finale, 5 },
        { S_CardAdditiveConditionEnum.Finale_Climax, 8 },
        { S_CardAdditiveConditionEnum.Chaos, 2 },
        { S_CardAdditiveConditionEnum.Chaos_Anarchy, 4 },
        { S_CardAdditiveConditionEnum.GrandChaos_Anarchy, 6 },
        { S_CardAdditiveConditionEnum.Chaos_Overflow, 5 },
        { S_CardAdditiveConditionEnum.Offensive, 5 },
        { S_CardAdditiveConditionEnum.Offensive_SameSuit, 7 },
        { S_CardAdditiveConditionEnum.AllOutOffensive, 7 },
        { S_CardAdditiveConditionEnum.AllOutOffensive_SameSuit, 9 },
        { S_CardAdditiveConditionEnum.Offensive_Overflow, 7 },
        { S_CardAdditiveConditionEnum.Precision_SameSuit, 4 },
        { S_CardAdditiveConditionEnum.HyperPrecision_SameSuit, 7 },
        { S_CardAdditiveConditionEnum.Precision_SameNumber, 5 },
        { S_CardAdditiveConditionEnum.HyperPrecision_SameNumber, 9 },
        { S_CardAdditiveConditionEnum.Precision_PlethoraNumber, 4 },
        { S_CardAdditiveConditionEnum.HyperPrecision_PlethoraNumber, 7 },
        { S_CardAdditiveConditionEnum.Overflow, 3 },
        { S_CardAdditiveConditionEnum.Unity, 2 },
        { S_CardAdditiveConditionEnum.Unity_Drastic, 5 },
    };

    public static readonly Dictionary<S_CardDebuffConditionEnum, int> DebuffConditionWeights = new()
    {
        { S_CardDebuffConditionEnum.None, 0 },
        { S_CardDebuffConditionEnum.Breakdown, 5 },
        { S_CardDebuffConditionEnum.Paranoia, 1 },
        { S_CardDebuffConditionEnum.Spell, 3 },
        { S_CardDebuffConditionEnum.Rebel, 4 },
    };

    public static readonly Dictionary<S_CardBasicEffectEnum, int> BasicEffectWeights = new()
    {
        { S_CardBasicEffectEnum.None, 0 },
        { S_CardBasicEffectEnum.Increase_Strength, 1 },
        { S_CardBasicEffectEnum.Increase_Mind, 1 },
        { S_CardBasicEffectEnum.Increase_Luck, 1 },
        { S_CardBasicEffectEnum.Increase_AllStat, 3 },
        { S_CardBasicEffectEnum.Break_Zenith, 6 },
        { S_CardBasicEffectEnum.Break_Genesis, 6 },
        { S_CardBasicEffectEnum.Manipulation, 1 },
        { S_CardBasicEffectEnum.Manipulation_Cheat, 3 },
        { S_CardBasicEffectEnum.Manipulation_Judge, 4 },
        { S_CardBasicEffectEnum.Resistance, 3 },
        { S_CardBasicEffectEnum.Resistance_Indomitable, 10 },
        { S_CardBasicEffectEnum.Harm_Strength, 2 },
        { S_CardBasicEffectEnum.Harm_Mind, 2 },
        { S_CardBasicEffectEnum.Harm_Luck, 2 },
        { S_CardBasicEffectEnum.Harm_StrengthAndMind, 8 },
        { S_CardBasicEffectEnum.Harm_StrengthAndLuck, 8 },
        { S_CardBasicEffectEnum.Harm_MindAndLuck, 8 },
        { S_CardBasicEffectEnum.Harm_Carnage, 12 },
        { S_CardBasicEffectEnum.Tempering, 5 },
        { S_CardBasicEffectEnum.Plunder, 1 },
        { S_CardBasicEffectEnum.Plunder_Raid, 8 },
        { S_CardBasicEffectEnum.Creation, 4 },
        { S_CardBasicEffectEnum.Creation_SameSuit, 5 },
        { S_CardBasicEffectEnum.Creation_SameNumber, 7 },
        { S_CardBasicEffectEnum.Creation_PlethoraNumber, 5 },
        { S_CardBasicEffectEnum.AreaExpansion, 2 },
        { S_CardBasicEffectEnum.First_SameSuit, 1 },
        { S_CardBasicEffectEnum.First_LeastSuit, 2 },
        { S_CardBasicEffectEnum.First_SameNumber, 3 },
        { S_CardBasicEffectEnum.First_CleanHitNumber, 4 },
        { S_CardBasicEffectEnum.Undertow, 5 },
        { S_CardBasicEffectEnum.Guidance_LeastSuit, 7 },
        { S_CardBasicEffectEnum.Guidance_LeastNumber, 8 },
    };

    public static readonly Dictionary<S_CardAdditiveEffectEnum, int> AdditiveEffectWeights = new()
    {
        { S_CardAdditiveEffectEnum.None, 0 },
        { S_CardAdditiveEffectEnum.Reflux_Subtle, 3 },
        { S_CardAdditiveEffectEnum.Reflux_Violent, 5 },
        { S_CardAdditiveEffectEnum.Reflux_Shatter, 7 },
        { S_CardAdditiveEffectEnum.Reflux_Stack, 4 },
        { S_CardAdditiveEffectEnum.Reflux_PlethoraNumber, 4 },
        { S_CardAdditiveEffectEnum.Reflux_Deck, 6 },
        { S_CardAdditiveEffectEnum.Reflux_Chaos, 7 },
        { S_CardAdditiveEffectEnum.Reflux_Offensive, 3 },
        { S_CardAdditiveEffectEnum.Reflux_Curse, 2 },
        { S_CardAdditiveEffectEnum.Reflux_Exclusion, 2 },
        { S_CardAdditiveEffectEnum.Reflux_Overdrive, 3 },
        { S_CardAdditiveEffectEnum.ColdBlood, 1 },
        { S_CardAdditiveEffectEnum.Immunity, 1 },
        { S_CardAdditiveEffectEnum.Omen, 2 },
        { S_CardAdditiveEffectEnum.Robbery, 1 },
        { S_CardAdditiveEffectEnum.Greed, 1 }
    };

    public static int GetWeights(S_CardBasicConditionEnum condition)
    {
        return BasicConditionWeights.TryGetValue(condition, out var weight) ? weight : 0;
    }
    public static int GetWeights(S_CardAdditiveConditionEnum condition)
    {
        return AllAdditiveConditionWeights.TryGetValue(condition, out var weight) ? weight : 0;
    }
    public static int GetWeights(S_CardDebuffConditionEnum condition)
    {
        return DebuffConditionWeights.TryGetValue(condition, out var weight) ? weight: 0;
    }
    public static int GetWeights(S_CardBasicEffectEnum condition)
    {
        return BasicEffectWeights.TryGetValue(condition, out var weight) ? weight : 0;
    }
    public static int GetWeights(S_CardAdditiveEffectEnum condition)
    {
        return AdditiveEffectWeights.TryGetValue(condition, out var weight) ? weight : 0;
    }
    public static List<S_CardBasicEffectEnum> GetBasicEffect(int weights)
    {
        List<S_CardBasicEffectEnum> list = BasicEffectWeights
            .Where(kv => kv.Value == weights)
            .Select(kv => kv.Key)
            .ToList();

        return list;
    }
    public static List<S_CardAdditiveEffectEnum> GetAdditiveEffect(int weights)
    {
        List<S_CardAdditiveEffectEnum> list = AdditiveEffectWeights
            .Where(kv => kv.Value == weights)
            .Select(kv => kv.Key)
            .ToList();

        return list;
    }
}
