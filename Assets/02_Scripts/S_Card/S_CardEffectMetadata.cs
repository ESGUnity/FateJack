using System.Collections.Generic;
using System.Linq;

public static class S_CardEffectMetadata
{
    #region JSON
    public static readonly Dictionary<S_CardBasicConditionEnum, S_CardProperty> BasicConditionProperty = new()
{
    { S_CardBasicConditionEnum.None, new S_CardProperty(0, "없음", "없음") },
    { S_CardBasicConditionEnum.Reverb, new S_CardProperty(1, "메아리", "다른 카드를 히트할 때 효과 발동") },
    { S_CardBasicConditionEnum.Resolve, new S_CardProperty(2, "결의", "스탠드 시 효과 발동") },
    { S_CardBasicConditionEnum.Unleash, new S_CardProperty(4, "발현", "이 카드가 히트될 때 효과 발동") },
};
    public static readonly Dictionary<S_CardAdditiveConditionEnum, S_CardProperty> AdditiveConditionProperty = new()
{
    { S_CardAdditiveConditionEnum.None, new S_CardProperty(0, "없음", "없음") },
    { S_CardAdditiveConditionEnum.Reverb_SameSuit, new S_CardProperty(2, "메아리 : 같은 문양", "히트한 카드의 문양과 같아야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Reverb_SameNumber, new S_CardProperty(4, "메아리 : 같은 숫자", "히트한 카드의 숫자와 같아야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Reverb_PlethoraNumber, new S_CardProperty(3, "메아리 : 과도한 숫자", "히트한 카드가 8 이상 숫자일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Reverb_CursedCard, new S_CardProperty(3, "메아리 : 저주 카드", "히트한 카드가 저주받았을 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Legion_SameSuit, new S_CardProperty(5, "군단 : 같은 문양", "스택에 이 카드와 같은 문양인 카드의 숫자 합이 40 이상일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.GreatLegion_SameSuit, new S_CardProperty(8, "대군단 : 같은 문양", "스택에 이 카드와 같은 문양의 카드의 숫자 합이 60 이상일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Finale, new S_CardProperty(8, "대미장식", "덱에 문양이 1개 이상 없어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Finale_Climax, new S_CardProperty(14, "대미장식 : 대단원", "덱에 문양이 2개 이상 없어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Chaos, new S_CardProperty(5, "혼돈", "스택에 모든 문양의 카드가 각각 1장 이상 있을 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Chaos_Anarchy, new S_CardProperty(8, "혼돈 : 무질서", "스택에 모든 문양의 카드가 각각 2장 이상 있을 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.GrandChaos_Anarchy, new S_CardProperty(11, "대혼돈 : 무질서", "스택에 모든 문양의 카드가 각각 3장 이상 있을 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Chaos_Overflow, new S_CardProperty(8, "혼돈 : 범람", "한 턴에 모든 문양의 카드를 각각 1장 이상 냈어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Offensive, new S_CardProperty(6, "공세", "스택에 연속되는 숫자의 최대 길이가 4 이상일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Offensive_SameSuit, new S_CardProperty(10, "공세 : 같은 문양", "스택에 같은 문양이면서 연속되는 숫자의 최대 길이가 4 이상일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.AllOutOffensive, new S_CardProperty(10, "총공세", "스택에 연속되는 숫자의 최대 길이가 8 이상일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.AllOutOffensive_SameSuit, new S_CardProperty(14, "총공세 : 같은 문양", "스택에 같은 문양이면서 연속되는 숫자의 최대 길이가 8 이상일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Offensive_Overflow, new S_CardProperty(10, "공세 : 범람", "한 턴에 낸 카드의 연속되는 숫자 길이가 4 이상일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Precision_SameSuit, new S_CardProperty(4, "정밀 : 같은 문양", "스택에 이 카드와 같은 문양의 카드가 정확히 3의 배수만큼 있어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.HyperPrecision_SameSuit, new S_CardProperty(7, "초정밀 : 같은 문양", "스택에 이 카드와 같은 문양의 카드가 정확히 6의 배수만큼 있어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Precision_SameNumber, new S_CardProperty(10, "정밀 : 같은 숫자", "스택에 이 카드와 같은 숫자의 카드가 정확히 3의 배수만큼 있어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.HyperPrecision_SameNumber, new S_CardProperty(15, "초정밀 : 같은 숫자", "스택에 이 카드와 같은 숫자의 카드가 정확히 6의 배수만큼 있어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Precision_PlethoraNumber, new S_CardProperty(4, "정밀 : 과도한 숫자", "스택에 8 이상 숫자의 카드가 정확히 3의 배수만큼 있어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.HyperPrecision_PlethoraNumber, new S_CardProperty(7, "초정밀 : 과도한 숫자", "스택에 8 이상 숫자의 카드가 정확히 6의 배수만큼 있어야만 조건 작용") },
    { S_CardAdditiveConditionEnum.Overflow, new S_CardProperty(5, "범람", "한 턴에 카드를 4장 이상 냈을 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Unity, new S_CardProperty(2, "단합", "스택에 문양이 2개 이하일 때만 조건 작용") },
    { S_CardAdditiveConditionEnum.Unity_Drastic, new S_CardProperty(5, "단합 : 급진", "스택에 문양이 1개만 있을 때만 조건 작용") },
};
    public static readonly Dictionary<S_CardDebuffConditionEnum, S_CardProperty> DebuffConditionProperty = new()
{
    { S_CardDebuffConditionEnum.None, new S_CardProperty(0, "없음", "없음") },
    { S_CardDebuffConditionEnum.Breakdown, new S_CardProperty(5, "붕괴", "효과 발동 시, 덱에서 무작위 카드 1장을 제외") },
    { S_CardDebuffConditionEnum.Delusion, new S_CardProperty(1, "망상", "효과 발동 시, 다음 히트 카드가 저주받습니다.") },
    { S_CardDebuffConditionEnum.Spell, new S_CardProperty(3, "주술", "효과 발동 시, 덱과 스택에서 각각 무작위 카드 1장을 저주") },
    { S_CardDebuffConditionEnum.Rebel, new S_CardProperty(4, "반발", "효과 발동 시, 한계 1 감소") },
};
    public static readonly Dictionary<S_CardBasicEffectEnum, S_CardProperty> BasicEffectProperty = new()
{
    { S_CardBasicEffectEnum.None, new S_CardProperty(0, "없음", "없음") },
    { S_CardBasicEffectEnum.Growth_Strength, new S_CardProperty(1, "성장 : 힘", "힘을 3 얻습니다.") },
    { S_CardBasicEffectEnum.Growth_Mind, new S_CardProperty(1, "성장 : 정신력", "정신력을 3 얻습니다.") },
    { S_CardBasicEffectEnum.Growth_Luck, new S_CardProperty(1, "성장 : 행운", "행운을 3 얻습니다.") },
    { S_CardBasicEffectEnum.Growth_AllStat, new S_CardProperty(3, "성장 : 모든 능력치", "모든 능력치를 3 얻습니다.") },
    { S_CardBasicEffectEnum.Break_MostStat, new S_CardProperty(7, "돌파 : 가장 높은 능력치", "가장 높은 능력치가 1.5배 증가합니다.") },
    { S_CardBasicEffectEnum.Break_RandomStat, new S_CardProperty(6, "돌파 : 무작위 능력치", "무작위 능력치가 1.5배 증가합니다.") },
    { S_CardBasicEffectEnum.Manipulation, new S_CardProperty(1, "조작", "숫자 합이 2 감소합니다.") },
    { S_CardBasicEffectEnum.Manipulation_CardNumber, new S_CardProperty(3, "조작 : 카드숫자", "이 카드의 숫자만큼 숫자 합이 감소합니다.") },
    { S_CardBasicEffectEnum.Manipulation_CleanHit, new S_CardProperty(4, "조작 : 클린히트", "클린히트가 되도록 숫자 합이 조정됩니다.") },
    { S_CardBasicEffectEnum.Resistance, new S_CardProperty(3, "저항", "한계를 1 얻습니다.") },
    { S_CardBasicEffectEnum.Resistance_CardNumber, new S_CardProperty(10, "저항 : 카드숫자", "이 카드의 숫자만큼 한계를 얻습니다.") },
    { S_CardBasicEffectEnum.Harm_Strength, new S_CardProperty(2, "피해 : 힘", "힘만큼 적에게 피해를 줍니다.") },
    { S_CardBasicEffectEnum.Harm_Mind, new S_CardProperty(2, "피해 : 정신력", "정신력만큼 적에게 피해를 줍니다.") },
    { S_CardBasicEffectEnum.Harm_Luck, new S_CardProperty(2, "피해 : 행운", "행운만큼 적에게 피해를 줍니다.") },
    { S_CardBasicEffectEnum.Harm_StrengthAndMind, new S_CardProperty(8, "피해 : 힘과 정신력", "힘과 정신력의 곱만큼 적에게 피해를 줍니다.") },
    { S_CardBasicEffectEnum.Harm_StrengthAndLuck, new S_CardProperty(8, "피해 : 힘과 행운", "힘과 행운의 곱만큼 적에게 피해를 줍니다.") },
    { S_CardBasicEffectEnum.Harm_MindAndLuck, new S_CardProperty(8, "피해 : 정신력과 행운", "정신력과 행운의 곱만큼 적에게 피해를 줍니다.") },
    { S_CardBasicEffectEnum.Harm_Carnage, new S_CardProperty(12, "피해 : 대학살", "모든 능력치의 곱만큼 적에게 피해를 줍니다.") },
    { S_CardBasicEffectEnum.Tempering, new S_CardProperty(5, "단련", "의지를 1 얻습니다.") },
    { S_CardBasicEffectEnum.Plunder, new S_CardProperty(1, "약탈", "골드를 2 얻습니다.") },
    { S_CardBasicEffectEnum.Plunder_Break, new S_CardProperty(8, "약탈 : 돌파", "골드가 1.5배 증가합니다.") },
    { S_CardBasicEffectEnum.Creation_Random, new S_CardProperty(4, "생성 : 무작위", "무작위 냉혈 카드를 생성하고 히트합니다.") },
    { S_CardBasicEffectEnum.Creation_SameSuit, new S_CardProperty(5, "생성 : 같은 문양", "이 카드와 문양이 같은 무작위 냉혈 카드를 생성하고 히트합니다.") },
    { S_CardBasicEffectEnum.Creation_SameNumber, new S_CardProperty(7, "생성 : 같은 숫자", "이 카드와 숫자가 같은 무작위 냉혈 카드를 생성하고 히트합니다.") },
    { S_CardBasicEffectEnum.Creation_PlethoraNumber, new S_CardProperty(5, "생성 : 과도한 숫자", "숫자가 8 이상인 무작위 냉혈 카드를 생성하고 히트합니다.") },
    { S_CardBasicEffectEnum.Expansion, new S_CardProperty(2, "전개", "다음 히트 시 추가 보기 카드 2장이 주어집니다.") },
    { S_CardBasicEffectEnum.First_SameSuit, new S_CardProperty(1, "우선 : 같은 문양", "다음 히트 시 이 카드와 같은 문양인 카드를 우선하여 히트합니다.") },
    { S_CardBasicEffectEnum.First_LeastSuit, new S_CardProperty(2, "우선 : 가장 적은 문양", "다음 히트 시 덱에서 가장 적은 문양의 카드를 우선하여 히트합니다.") },
    { S_CardBasicEffectEnum.First_SameNumber, new S_CardProperty(3, "우선 : 같은 숫자", "다음 히트 시 이 카드와 같은 숫자인 카드를 우선하여 히트합니다.") },
    { S_CardBasicEffectEnum.First_CleanHitNumber, new S_CardProperty(4, "우선 : 클린히트 숫자", "다음 히트 시 클린히트가 되는 카드를 우선하여 히트합니다.") },
    { S_CardBasicEffectEnum.Undertow, new S_CardProperty(5, "역류", "역류를 제외한 스택에 있는 무작위 카드 2장의 효과를 발동합니다.") },
    { S_CardBasicEffectEnum.Guidance_LeastSuit, new S_CardProperty(7, "인도 : 가장 적은 문양", "덱에서 가장 적은 문양의 카드를 모두 히트합니다.") },
    { S_CardBasicEffectEnum.Guidance_LeastNumber, new S_CardProperty(8, "인도 : 가장 적은 숫자", "덱에서 가장 적은 숫자의 카드를 모두 히트합니다.") },
};
    public static readonly Dictionary<S_CardAdditiveEffectEnum, S_CardProperty> AdditiveEffectProperty = new()
{
    { S_CardAdditiveEffectEnum.None, new S_CardProperty(0, "없음", "없음") },
    { S_CardAdditiveEffectEnum.Reflux_Subtle, new S_CardProperty(3, "환류 : 미묘", "효과 발동 시, 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Violent, new S_CardProperty(5, "환류 : 격렬", "효과 발동 시, 효과를 2번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Shatter, new S_CardProperty(7, "환류 : 파쇄", "효과 발동 시, 효과를 3번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Stack, new S_CardProperty(4, "환류 : 스택", "효과 발동 시, 스택에 있는 카드 4장 당 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_PlethoraNumber, new S_CardProperty(4, "환류 : 과도한 숫자", "효과 발동 시, 스택에 있는 8 이상 숫자 카드 3장 당 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Deck, new S_CardProperty(6, "환류 : 덱", "효과 발동 시, 덱에 있는 카드 6장 당 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Chaos, new S_CardProperty(7, "환류 : 혼돈", "효과 발동 시, 스택에 있는 문양 1개 당 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Offensive, new S_CardProperty(3, "환류 : 공세", "효과 발동 시, 스택에 연속되는 숫자의 최대 길이 2 당 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Curse, new S_CardProperty(2, "환류 : 저주", "효과 발동 시, 덱과 스택에 있는 저주받은 카드 3장 당 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Exclusion, new S_CardProperty(2, "환류 : 제외", "효과 발동 시, 제외된 카드 3장 당 효과를 1번 더 발동") },
    { S_CardAdditiveEffectEnum.Reflux_Overdrive, new S_CardProperty(3, "환류 : 극한", "효과 발동 시, 체력이 1이라면 효과를 2번 더 발동") },
    { S_CardAdditiveEffectEnum.ColdBlood, new S_CardProperty(1, "냉혈", "숫자에 의한 능력치와 숫자 합을 증가시키지 않습니다.") },
    { S_CardAdditiveEffectEnum.Immunity, new S_CardProperty(1, "면역", "저주에 걸리지 않습니다.") },
    { S_CardAdditiveEffectEnum.Omen, new S_CardProperty(2, "흉조", "시련이 종료될 때 이 카드가 덱에 있다면 골드 2 획득") },
    { S_CardAdditiveEffectEnum.Robbery, new S_CardProperty(1, "강도", "시련이 종료될 때 이 카드가 스택에 있다면 골드 2 획득") },
    { S_CardAdditiveEffectEnum.Greed, new S_CardProperty(1, "탐욕", "시련이 종료될 때 이 카드가 저주받았다면 골드 2 획득") },
};
    #endregion
    #region 가중치 함수
    public static int GetWeights(S_CardBasicConditionEnum condition)
    {
        return BasicConditionProperty.TryGetValue(condition, out var property) ? property.Weights : 0;
    }
    public static int GetWeights(S_CardAdditiveConditionEnum condition)
    {
        return AdditiveConditionProperty.TryGetValue(condition, out var property) ? property.Weights : 0;
    }
    public static int GetWeights(S_CardDebuffConditionEnum condition)
    {
        return DebuffConditionProperty.TryGetValue(condition, out var property) ? property.Weights : 0;
    }
    public static int GetWeights(S_CardBasicEffectEnum condition)
    {
        return BasicEffectProperty.TryGetValue(condition, out var property) ? property.Weights : 0;
    }
    public static int GetWeights(S_CardAdditiveEffectEnum condition)
    {
        return AdditiveEffectProperty.TryGetValue(condition, out var property) ? property.Weights : 0;
    }
    #endregion
    #region 이름 함수
    public static string GetName(S_CardBasicConditionEnum condition)
    {
        return BasicConditionProperty.TryGetValue(condition, out var property) ? property.Name : string.Empty;
    }
    public static string GetName(S_CardAdditiveConditionEnum condition)
    {
        return AdditiveConditionProperty.TryGetValue(condition, out var property) ? property.Name : string.Empty;
    }
    public static string GetName(S_CardDebuffConditionEnum condition)
    {
        return DebuffConditionProperty.TryGetValue(condition, out var property) ? property.Name : string.Empty;
    }
    public static string GetName(S_CardBasicEffectEnum condition)
    {
        return BasicEffectProperty.TryGetValue(condition, out var property) ? property.Name : string.Empty;
    }
    public static string GetName(S_CardAdditiveEffectEnum condition)
    {
        return AdditiveEffectProperty.TryGetValue(condition, out var property) ? property.Name : string.Empty;
    }
    #endregion
    #region 설명 함수
    public static string GetDescription(S_CardBasicConditionEnum condition)
    {
        return BasicConditionProperty.TryGetValue(condition, out var property) ? property.Description : string.Empty;
    }
    public static string GetDescription(S_CardAdditiveConditionEnum condition)
    {
        return AdditiveConditionProperty.TryGetValue(condition, out var property) ? property.Description : string.Empty;
    }
    public static string GetDescription(S_CardDebuffConditionEnum condition)
    {
        return DebuffConditionProperty.TryGetValue(condition, out var property) ? property.Description : string.Empty;
    }
    public static string GetDescription(S_CardBasicEffectEnum condition)
    {
        return BasicEffectProperty.TryGetValue(condition, out var property) ? property.Description : string.Empty;
    }
    public static string GetDescription(S_CardAdditiveEffectEnum condition)
    {
        return AdditiveEffectProperty.TryGetValue(condition, out var property) ? property.Description : string.Empty;
    }
    #endregion
}

public struct S_CardProperty
{
    public int Weights;
    public string Name;
    public string Description;

    public S_CardProperty(int weights, string name, string description)
    {
        Weights = weights;
        Name = name;
        Description = description;
    }
}