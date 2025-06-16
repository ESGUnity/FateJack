using System;
using System.Collections.Generic;
using System.Linq;

public static class S_CardEffectMetadata
{
    #region JSON
    public static readonly Dictionary<S_CardEffectEnum, S_CardProperty> CardEffectProperty = new()
{
    { S_CardEffectEnum.Str_Stimulus, new S_CardProperty(1, "자극", "힘을 2 얻습니다.") },
    { S_CardEffectEnum.Str_ZenithBreak, new S_CardProperty(10, "정점 돌파", "힘이 1.5배 증가합니다.") },
    { S_CardEffectEnum.Str_SinisterImpulse, new S_CardProperty(3, "불길한 충동", "망상을 얻습니다. 힘을 8 얻습니다.") },
    { S_CardEffectEnum.Str_CalamityApproaches, new S_CardProperty(8, "엄습하는 재앙", "스택에 무작위 카드를 2장 저주합니다. 힘이 1.5배 증가합니다.") },
    { S_CardEffectEnum.Str_UntappedPower, new S_CardProperty(7, "무한한 잠재력", "가장 높은 능력치를 모두 잃습니다. 힘이 3배 증가합니다.") },
    { S_CardEffectEnum.Str_UnjustSacrifice, new S_CardProperty(8, "불의의 희생", "정신력과 행운만큼 힘을 얻습니다. 정신력과 행운을 절반 잃습니다.") },
    { S_CardEffectEnum.Str_WrathStrike, new S_CardProperty(2, "분노의 타격", "힘의 3배만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Str_EngulfInFlames, new S_CardProperty(5, "불사르기", "버스트라면 이 카드를 저주합니다. 힘의 12배만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Str_FinishingStrike, new S_CardProperty(6, "마무리 일격", "오른쪽에 있는 모든 카드를 저주합니다. 힘의 24배만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Str_FlowingSin, new S_CardProperty(4, "흐르는 죄악", "힘의 4배만큼 피해를 줍니다. 저주받은 카드 장수만큼 반복합니다.") },
    { S_CardEffectEnum.Str_BindingForce, new S_CardProperty(18, "억압", "힘과 행운의 곱만큼 피해를 줍니다. 힘 카드 장수만큼 반복합니다.") },
    { S_CardEffectEnum.Str_Grudge, new S_CardProperty(9, "원한", "힘만큼 피해를 줍니다. 적 체력이 75% 이상이라면 대신 힘과 정신력을 곱한만큼 줍니다.") },

    { S_CardEffectEnum.Mind_Focus, new S_CardProperty(1, "집중", "정신력을 2 얻습니다.") },
    { S_CardEffectEnum.Mind_DeepInsight, new S_CardProperty(10, "통찰", "정신력이 1.5배 증가합니다.") },
    { S_CardEffectEnum.Mind_PerfectForm, new S_CardProperty(6, "무결점", "버스트가 아니라면 완벽이 될 때까지 무게를 얻습니다. 얻은 무게만큼 정신력을 얻습니다.") },
    { S_CardEffectEnum.Mind_Unshackle, new S_CardProperty(5, "해방", "정신력 카드 1장 당 정신력을 3 얻습니다.") },
    { S_CardEffectEnum.Mind_Drain, new S_CardProperty(4, "흡수", "힘과 행운을 최대 5만큼 잃습니다. 잃은 능력치의 2배만큼 정신력을 얻습니다.") },
    { S_CardEffectEnum.Mind_WingsOfFreedom, new S_CardProperty(7, "자유의 날개", "한계를 5 잃습니다. 정신력이 1.5배 증가합니다.") },
    { S_CardEffectEnum.Mind_PreciseStrike, new S_CardProperty(2, "정밀 타격", "정신력의 3배만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Mind_SharpCut, new S_CardProperty(3, "절삭", "정신력만큼 피해를 줍니다. 완벽이라면 대신 정신력의 12배만큼 줍니다.") },
    { S_CardEffectEnum.Mind_Split, new S_CardProperty(8, "분열", "정신력과 현재 무게를 곱한만큼 피해를 줍니다. 현재 무게만큼 정신력을 잃습니다.") },
    { S_CardEffectEnum.Mind_Accept, new S_CardProperty(6, "수긍", "정신력 10 당 한계를 1 얻습니다.") },
    { S_CardEffectEnum.Mind_Dissolute, new S_CardProperty(9, "무절제", "정신력과 힘을 곱한만큼 피해를 줍니다. 버스트라면 정신력을 절반 잃습니다.") },
    { S_CardEffectEnum.Mind_Awakening, new S_CardProperty(11, "각성", "정신력만큼 피해를 줍니다. 완벽이라면 대신 정신력과 행운을 곱한만큼 줍니다.") },

    { S_CardEffectEnum.Luck_Chance, new S_CardProperty(1, "기회", "행운을 2 얻습니다.") },
    { S_CardEffectEnum.Luck_Disorder, new S_CardProperty(10, "무질서", "행운이 1.5배 증가합니다.") },
    { S_CardEffectEnum.Luck_Composure, new S_CardProperty(2, "여유부리기", "오른쪽에 있는 카드 장수만큼 행운을 얻습니다.") },
    { S_CardEffectEnum.Luck_SilentDomination, new S_CardProperty(4, "침묵의 정복", "각인이 있는 무작위 카드를 1장 생성하고 카드의 무게만큼 행운을 얻습니다.") },
    { S_CardEffectEnum.Luck_Artifice, new S_CardProperty(5, "기교", "오른쪽 카드의 효과를 1번 더 발동합니다. 행운 카드가 아니었다면 행운을 12 얻습니다.") },
    { S_CardEffectEnum.Luck_AllForOne, new S_CardProperty(9, "하나를 위한 모두", "무작위 카드를 3장 생성합니다. 행운 카드가 아닌 카드를 생성할 때마다 행운이 1.5배 증가합니다.") },
    { S_CardEffectEnum.Luck_SuddenStrike, new S_CardProperty(2, "기습 타격", "행운의 3배만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Luck_CriticalBlow, new S_CardProperty(4, "치명타", "행운 1 당 1%의 확률로 행운의 16배만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Luck_ForcedTake, new S_CardProperty(8, "강탈", "행운의 절반만큼 피해를 줍니다. 피해량만큼 골드를 얻습니다.") },
    { S_CardEffectEnum.Luck_Grill, new S_CardProperty(6, "다그치기", "행운 10 당 오른쪽 카드를 1번 더 발동합니다.") },
    { S_CardEffectEnum.Luck_Shake, new S_CardProperty(4, "떠보기", "행운과 힘을 곱한만큼 피해를 줍니다. 단 0.25배의 피해를 줍니다.") },
    { S_CardEffectEnum.Luck_FatalBlow, new S_CardProperty(15, "결정타", "힘과 정신력을 곱한만큼 피해를 줍니다. 행운 10 당 1번 더 반복합니다.") },

    { S_CardEffectEnum.Common_Trinity, new S_CardProperty(3, "삼위일체", "모든 능력치를 3 얻습니다.") },
    { S_CardEffectEnum.Common_Balance, new S_CardProperty(11, "균형잡기", "무작위 능력치가 2배 증가합니다. 매 턴마다 능력치가 변경됩니다.") },
    { S_CardEffectEnum.Common_Berserk, new S_CardProperty(9, "광란", "무작위 능력치 2개를 곱한만큼 피해를 줍니다. 매 턴마다 능력치가 변경됩니다.") },
    { S_CardEffectEnum.Common_Carnage, new S_CardProperty(20, "대학살", "모든 능력치를 곱한만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Common_LastStruggle, new S_CardProperty(12, "최후의 발악", "모든 능력치를 15 잃습니다. 모든 능력치를 곱한만큼 피해를 줍니다.") },
    { S_CardEffectEnum.Common_Resistance, new S_CardProperty(4, "저항", "한계를 1 얻습니다.") },
    { S_CardEffectEnum.Common_Realization, new S_CardProperty(5, "깨달음", "왼쪽에 있는 카드 장수만큼 한계를 얻고 오른쪽에 있는 카드 장수만큼 무게를 얻습니다.") },
    { S_CardEffectEnum.Common_Corrupt, new S_CardProperty(3, "감염", "이 카드와 같은 각인이 있는 무작위 카드를 생성합니다.") },
    { S_CardEffectEnum.Common_Imitate, new S_CardProperty(5, "흉내", "오른쪽 카드를 복사하여 생성합니다.") },
    { S_CardEffectEnum.Common_Plunder, new S_CardProperty(1, "약탈", "골드를 2 얻습니다.") },
    { S_CardEffectEnum.Common_Undertow, new S_CardProperty(3, "역류", "오른쪽 카드를 1번 더 발동합니다.") },
    { S_CardEffectEnum.Common_Adventure, new S_CardProperty(3, "모험", "전개를 얻습니다.") },
    { S_CardEffectEnum.Common_Inspiration, new S_CardProperty(2, "영감", "우선을 얻습니다.") },
    { S_CardEffectEnum.Common_Repose, new S_CardProperty(7, "침착", "냉혈을 얻습니다.") },
};
    public static readonly Dictionary<S_EngravingEnum, S_CardProperty> EngravingEffectProperty = new()
{
    { S_EngravingEnum.None, new S_CardProperty(0, "없음", "없음") },
    { S_EngravingEnum.Reverb, new S_CardProperty(1, "메아리", "다른 카드를 히트할 때 효과 발동") },
    { S_EngravingEnum.Resolve, new S_CardProperty(2, "결의", "스탠드 시 효과 발동") },

    { S_EngravingEnum.Legion, new S_CardProperty(-3, "군단", "스택에 카드 무게 합이 30 이상이어야 효과가 발동합니다.") },
    { S_EngravingEnum.Legion_Flip, new S_CardProperty(-6, "군단(뒤집힘)", "스택에 카드 무게 합이 50 이상이어야 효과가 발동합니다.") },
    { S_EngravingEnum.AllOut, new S_CardProperty(6, "총공세", "스택에 카드 무게 합 15 당 효과가 1번 더 발동합니다.") },
    { S_EngravingEnum.AllOut_Flip, new S_CardProperty(3, "총공세(뒤집힘)", "스택에 카드 무게 합 20 당 효과가 1번 더 발동합니다.") },

    { S_EngravingEnum.Delicacy, new S_CardProperty(-3, "세심", "스택에 카드가 정확히 3의 배수만큼 있어야 효과가 발동합니다.") },
    { S_EngravingEnum.Delicacy_Flip, new S_CardProperty(-7, "세심(뒤집힘)", "스택에 카드가 정확히 6의 배수만큼 있어야 효과가 발동합니다.") },
    { S_EngravingEnum.Precision, new S_CardProperty(9, "정밀", "스택에 카드가 정확히 3의 배수만큼 있다면 효과가 2번 더 발동합니다.") },
    { S_EngravingEnum.Precision_Flip, new S_CardProperty(4, "정밀(뒤집힘)", "스택에 카드가 정확히 6의 배수만큼 있다면 효과가 2번 더 발동합니다.") },

    { S_EngravingEnum.Resection, new S_CardProperty(-2, "절제", "한 턴에 카드를 4장 이하만 내야 효과가 발동합니다.") },
    { S_EngravingEnum.Resection_Flip, new S_CardProperty(-4, "절제(뒤집힘)", "한 턴에 카드를 3장 이하만 내야 효과가 발동합니다.") },
    { S_EngravingEnum.Patience, new S_CardProperty(8, "인내", "한 턴에 카드를 4장 이하만 냈다면 효과가 2번 더 발동합니다.") },
    { S_EngravingEnum.Patience_Flip, new S_CardProperty(4, "인내(뒤집힘)", "한 턴에 카드를 3장 이하만 냈다면 효과가 2번 더 발동합니다.") },

    { S_EngravingEnum.Overflow, new S_CardProperty(-4, "범람", "한 턴에 카드를 5장 이상 내야 효과가 발동합니다.") },
    { S_EngravingEnum.Overflow_Flip, new S_CardProperty(-7, "범람(뒤집힘)", "한 턴에 카드를 6장 이상 내야 효과가 발동합니다.") },
    { S_EngravingEnum.Fierce, new S_CardProperty(7, "격렬", "한 턴에 카드를 5장 이상 냈다면 효과가 2번 더 발동합니다.") },
    { S_EngravingEnum.Fierce_Flip, new S_CardProperty(4, "격렬(뒤집힘)", "한 턴에 카드를 6장 이상 냈다면 효과가 2번 더 발동합니다.") },

    { S_EngravingEnum.GrandChaos, new S_CardProperty(-20, "대혼돈", "한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 1장씩 내야 효과가 발동합니다.") },
    { S_EngravingEnum.GrandChaos_Flip, new S_CardProperty(-9, "대혼돈(뒤집힘)", "한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 2장씩 내야 효과가 발동합니다.") },
    { S_EngravingEnum.Crush, new S_CardProperty(1, "파쇄", "한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 1장씩 냈다면 효과가 2번 더 발동합니다.") },
    { S_EngravingEnum.Crush_Flip, new S_CardProperty(0, "파쇄(뒤집힘)", "한 턴에 힘 카드, 정신력 카드, 행운 카드, 공용 카드를 2장씩 냈다면 효과가 4번 더 발동합니다.") },

    { S_EngravingEnum.Overdrive, new S_CardProperty(-2, "극한", "체력이 1이어야 효과가 발동합니다.") },
    { S_EngravingEnum.Immersion, new S_CardProperty(7, "몰두", "체력이 1이라면 효과가 2번 더 발동합니다.") },
    { S_EngravingEnum.Finale, new S_CardProperty(-8, "대미장식", "덱에 카드가 없어야 발동합니다.") },
    { S_EngravingEnum.Climax, new S_CardProperty(2, "대단원", "덱에 카드가 없다면 효과를 2번 더 발동합니다.") },
    { S_EngravingEnum.Immunity, new S_CardProperty(1, "면역", "저주에 걸리지 않습니다.") },
    { S_EngravingEnum.Omen, new S_CardProperty(1, "흉조", "시련이 종료될 때 이 카드가 스택에 있다면 골드를 3 얻습니다.") },
    { S_EngravingEnum.Greed, new S_CardProperty(1, "탐욕", "시련이 종료될 때 이 카드가 저주받았다면 골드를 3 얻습니다.") },
    { S_EngravingEnum.Unleash, new S_CardProperty(3, "발현", "이 카드는 낼 때에도 발동합니다.") },
    { S_EngravingEnum.Flexible, new S_CardProperty(6, "유연", "이 카드는 위치를 변경할 수 있습니다.") },
    { S_EngravingEnum.QuickAction, new S_CardProperty(3, "속전속결", "시련 시작 시 이 카드를 냅니다.") },
    { S_EngravingEnum.Spell, new S_CardProperty(-3, "주술", "효과 발동 시, 스택에서 카드 1장을 저주합니다.") },
    { S_EngravingEnum.DeepShadow, new S_CardProperty(-2, "깊은 그림자", "효과 발동 시, 망상을 얻습니다.") },
};
    public static readonly HashSet<S_EngravingEnum> CanFlipEngraving = new()
    {
        S_EngravingEnum.Legion, S_EngravingEnum.Legion_Flip, S_EngravingEnum.AllOut, S_EngravingEnum.AllOut_Flip,
        S_EngravingEnum.Delicacy, S_EngravingEnum.Delicacy_Flip, S_EngravingEnum.Precision, S_EngravingEnum.Precision_Flip,
        S_EngravingEnum.Resection, S_EngravingEnum.Resection_Flip, S_EngravingEnum.Patience, S_EngravingEnum.Patience_Flip,
        S_EngravingEnum.Overflow, S_EngravingEnum.Overflow_Flip, S_EngravingEnum.Fierce, S_EngravingEnum.Fierce_Flip,
        S_EngravingEnum.GrandChaos, S_EngravingEnum.GrandChaos_Flip, S_EngravingEnum.Crush, S_EngravingEnum.Crush_Flip,
    };
    #endregion
    #region 가중치 함수
    public static int GetWeights(S_CardEffectEnum cardEffect)
    {
        return CardEffectProperty.TryGetValue(cardEffect, out var property) ? property.Weights : 0;
    }
    public static int GetWeights(S_EngravingEnum engraving)
    {
        return EngravingEffectProperty.TryGetValue(engraving, out var property) ? property.Weights : 0;
    }
    #endregion
    #region 이름 함수
    public static string GetName(S_CardEffectEnum cardEffect)
    {
        return CardEffectProperty.TryGetValue(cardEffect, out var property) ? property.Name : string.Empty;
    }
    public static string GetName(S_EngravingEnum engraving)
    {
        return EngravingEffectProperty.TryGetValue(engraving, out var property) ? property.Name : string.Empty;
    }
    #endregion
    #region 설명 함수
    public static string GetDescription(S_CardEffectEnum condition)
    {
        return CardEffectProperty.TryGetValue(condition, out var property) ? property.Description : string.Empty;
    }
    public static string GetDescription(S_EngravingEnum condition)
    {
        return EngravingEffectProperty.TryGetValue(condition, out var property) ? property.Description : string.Empty;
    }
    #endregion
    #region 카드 타입 함수
    public static List<S_CardEffectEnum> GetCardEffectsByType(S_CardTypeEnum type) // 특정 타입의 효과를 모두 가져오기
    {
        if (type == default)
        {
            return Enum.GetValues(typeof(S_CardEffectEnum))
            .Cast<S_CardEffectEnum>()
            .Where(x => x != S_CardEffectEnum.None)
            .ToList();
        }

        string prefix = type.ToString() + "_";

        return Enum.GetValues(typeof(S_CardEffectEnum))
            .Cast<S_CardEffectEnum>()
            .Where(e => e.ToString().StartsWith(prefix))
            .ToList();
    }
    public static S_CardTypeEnum GetCardTypeFromEffect(S_CardEffectEnum effectEnum) // 카드 효과가 어떤 타입인지 반환
    {
        string effectName = effectEnum.ToString();

        int underscoreIndex = effectName.IndexOf('_');
        if (underscoreIndex < 0)
        {
            return S_CardTypeEnum.None; // '_'이 없으면 타입 판단 불가
        }

        string prefix = effectName.Substring(0, underscoreIndex);

        // 문자열을 enum으로 변환 시도
        if (Enum.TryParse<S_CardTypeEnum>(prefix, out S_CardTypeEnum result))
        {
            return result;
        }

        return S_CardTypeEnum.None; // 변환 실패 시 fallback
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