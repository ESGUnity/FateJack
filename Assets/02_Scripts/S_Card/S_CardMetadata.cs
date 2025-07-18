using System;
using System.Collections.Generic;
using System.Linq;

public static class S_CardMetadata
{
    #region JSON
    public static readonly Dictionary<string, S_TextProperty> CardDatas = new()
    {
        // 힘
        { "Stimulus", new S_TextProperty(
            "자극", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 1 얻습니다.", 
            "Stimulus", "") },
        { "Stimulus_Flip", new S_TextProperty(
            "뒤집힌 자극", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>망상<sprite name=Delusion></Accent_Basic>을 1 얻고 <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 4 얻습니다.", 
            "Flip Stimulus", "") },
        { "WrathStrike", new S_TextProperty(
            "분노의 타격", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>힘<sprite name=Str></Accent_Basic>의 6배만큼 피해를 줍니다.",
            "WrathStrike", "") },
        { "WrathStrike_Flip", new S_TextProperty(
            "뒤집힌 분노의 타격", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>힘<sprite name=Str></Accent_Basic>의 20배만큼 피해를 줍니다.",
            "Flip WrathStrike", "") },
        { "SinisterImpulse", new S_TextProperty(
            "불길한 충동", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>체력<sprite name=Health></Accent_Basic>을 1 잃고 <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 8 얻습니다.",
            "SinisterImpulse", "") },
        { "SinisterImpulse_Flip", new S_TextProperty(
            "뒤집힌 불길한 충동", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>속전속결</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>망상<sprite name=Delusion></Accent_Basic>을 1 얻습니다. <Accent_Basic>지속</Accent_Basic> : 모든 피해가 2배 증가합니다.",
            "Flip SinisterImpulse", "") },
        { "FlowingSin", new S_TextProperty(
            "흐르는 죄악", "<Accent_Basic>발현</Accent_Basic> : 모든 <Accent_Basic>저주받은 카드</Accent_Basic> 1장 당 <Accent_Basic>힘<sprite name=Str></Accent_Basic>의 5배만큼 피해를 줍니다.",
            "FlowingSin", "") },
        { "FlowingSin_Flip", new S_TextProperty(
            "뒤집힌 흐르는 죄악", "<Accent_Basic>면역</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : 카드가 <Accent_Basic>저주</Accent_Basic>받을 때마다 <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 2 얻습니다.",
            "Flip FlowingSin", "") },
        { "EngulfinFlames", new S_TextProperty(
            "불사르기", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>힘<sprite name=Str></Accent_Basic> 10 당 필드에 있는 카드 1장의 <Accent_Basic>저주</Accent_Basic>를 <Accent_Basic>해제</Accent_Basic>합니다.",
            "EngulfinFlames", "") },
        { "EngulfinFlames_Flip", new S_TextProperty(
            "뒤집힌 불사르기", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>힘<sprite name=Str></Accent_Basic>의 35배만큼 피해를 주고 <Accent_Basic>버스트<sprite name=Burst></Accent_Basic>라면 필드에 있는 모든 카드를 <Accent_Basic>저주</Accent_Basic>합니다.",
            "Flip EngulfinFlames", "") },
        { "UntappedPower", new S_TextProperty(
            "무한한 잠재력", "<Accent_Basic>발현</Accent_Basic> : 가장 높은 능력치를 모두 잃지만 <Accent_Basic>힘<sprite name=Str></Accent_Basic>이 3배 증가합니다.",
            "UntappedPower", "") },
        { "UntappedPower_Flip", new S_TextProperty(
            "뒤집힌 무한한 잠재력", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>힘 카드</Accent_Basic> 1장 당 <Accent_Basic>힘<sprite name=Str></Accent_Basic>의 10배만큼 피해를 줍니다.",
            "Flip UntappedPower", "") },
        { "CalamityApproaches", new S_TextProperty(
            "엄습하는 재앙", "<Accent_Basic>지속</Accent_Basic> : <Accent_Basic>상태</Accent_Basic>를 얻을 때마다 필드에 있는 카드 1장의 <Accent_Basic>저주</Accent_Basic>를 <Accent_Basic>해제</Accent_Basic>합니다.",
            "CalamityApproaches", "") },
        { "CalamityApproaches_Flip", new S_TextProperty(
            "뒤집힌 엄습하는 재앙", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>망상<sprite name=Delusion></Accent_Basic>을 2 얻고 필드에 있는 모든 카드의 <Accent_Basic>저주</Accent_Basic>를 <Accent_Basic>해제</Accent_Basic>합니다.",
            "Flip CalamityApproaches", "") },
        { "UnjustSacrifice", new S_TextProperty(
            "불의의 희생", "<Accent_Basic>지속</Accent_Basic> : 카드가 <Accent_Basic>저주</Accent_Basic>받을 때마다 <Accent_Basic>전개<sprite name=Expansion></Accent_Basic>, <Accent_Basic>우선<sprite name=First></Accent_Basic>, <Accent_Basic>냉혈<sprite name=ColdBlood></Accent_Basic> 중 하나를 1 얻습니다.",
            "UnjustSacrifice", "") },
        { "UnjustSacrifice_Flip", new S_TextProperty(
            "뒤집힌 불의의 희생", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>힘 카드</Accent_Basic>를 낼 때마다 즉시 <Accent_Basic>저주</Accent_Basic>하지만 <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 5 얻습니다.",
            "Flip UnjustSacrifice", "") },
        { "FinishingStrike", new S_TextProperty(
            "마무리 일격", "<Accent_Basic>발현</Accent_Basic> : 오른쪽에 있는 모든 카드를 <Accent_Basic>저주</Accent_Basic>하고 <Accent_Basic>힘<sprite name=Str></Accent_Basic>의 50배만큼 피해를 줍니다.",
            "FinishingStrike", "") },
        { "FinishingStrike_Flip", new S_TextProperty(
            "뒤집힌 마무리 일격", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>상태</Accent_Basic> 1 당 <Accent_Basic>힘<sprite name=Str></Accent_Basic>의 10배만큼 피해를 주고 <Accent_Basic>상태</Accent_Basic>를 모두 잃습니다.",
            "Flip FinishingStrike", "") },
        { "ZenithBreak", new S_TextProperty(
            "정점 돌파", "<Accent_Basic>지속</Accent_Basic> : 얻는 <Accent_Basic>힘<sprite name=Str></Accent_Basic>이 3배 증가합니다.",
            "ZenithBreak", "") },
        { "ZenithBreak_Flip", new S_TextProperty(
            "뒤집힌 정점 돌파", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>망상<sprite name=Delusion></Accent_Basic>을 2 얻습니다. <Accent_Basic>지속</Accent_Basic> : 얻는 <Accent_Basic>힘<sprite name=Str></Accent_Basic>이 4배 증가합니다.",
            "Flip ZenithBreak", "") },
        { "Grudge", new S_TextProperty(
            "원한", "<Accent_Basic>발현</Accent_Basic> : 모든 <Accent_Basic>저주받은 카드</Accent_Basic> 3장 당 <Accent_Basic>힘<sprite name=Str></Accent_Basic>과 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Grudge", "") },
        { "Grudge_Flip", new S_TextProperty(
            "뒤집힌 원한", "<Accent_Basic>발현</Accent_Basic> : 모든 <Accent_Basic>저주받은 카드</Accent_Basic> 3장 당 <Accent_Basic>힘<sprite name=Str></Accent_Basic>과 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Flip Grudge", "") },
        { "BindingForce", new S_TextProperty(
            "억압", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>힘 카드</Accent_Basic> 3장 당 <Accent_Basic>힘<sprite name=Str></Accent_Basic>과 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "BindingForce", "") },
        { "BindingForce_Flip", new S_TextProperty(
            "뒤집힌 억압", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>힘 카드</Accent_Basic> 3장 당 <Accent_Basic>힘<sprite name=Str></Accent_Basic>과 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Flip BindingForce", "") },

        // 정신력
        { "Focus", new S_TextProperty(
            "집중", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 1 얻습니다.",
            "Focus", "") },
        { "Focus_Flip", new S_TextProperty(
            "뒤집힌 집중", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>정신력 카드</Accent_Basic>를 낼 때마다 <Accent_Basic>무게</Accent_Basic>가 1 감소합니다.",
            "Flip Focus", "") },
        { "PreciseStrike", new S_TextProperty(
            "정밀 타격", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>의 6배만큼 피해를 줍니다.",
            "PreciseStrike", "") },
        { "PreciseStrike_Flip", new S_TextProperty(
            "뒤집힌 정밀 타격", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>의 12배만큼 피해를 줍니다. <Accent_Basic>완벽<sprite name=Perfect></Accent_Basic>이 아니라면 <Accent_Basic>과부하</Accent_Basic>됩니다.",
            "Flip PreciseStrike", "") },
        { "Drain", new S_TextProperty(
            "흡수", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>힘<sprite name=Str></Accent_Basic>과 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 최대 5 잃지만 잃은 능력치의 2배만큼 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 얻습니다.",
            "Drain", "") },
        { "Drain_Flip", new S_TextProperty(
            "뒤집힌 흡수", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 최대 10 잃지만 잃은 능력치만큼 <Accent_Basic>힘<sprite name=Str></Accent_Basic>과 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 얻습니다.",
            "Flip Drain", "") },
        { "Unshackle", new S_TextProperty(
            "해방", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>정신력 카드</Accent_Basic> 1장 당 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 2 얻습니다.",
            "Unshackle", "") },
        { "Unshackle_Flip", new S_TextProperty(
            "뒤집힌 해방", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : 카드가 <Accent_Basic>저주</Accent_Basic>받을 때마다 <Accent_Basic>무게</Accent_Basic>가 3 감소합니다.",
            "Flip Unshackle", "") },
        { "SharpCut", new S_TextProperty(
            "절삭", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : 필드에 있는 <Accent_Basic>저주받은 카드</Accent_Basic>는 <Accent_Basic>고정</Accent_Basic>됩니다.",
            "SharpCut", "") },
        { "SharpCut_Flip", new S_TextProperty(
            "뒤집힌 절삭", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>의 <Accent_MultiPerHitCount>4</Accent_MultiPerHitCount>배만큼 피해를 줍니다. <Accent_Basic>지속</Accent_Basic> : 이 카드를 낼 때마다 피해량이 2배 증가합니다.",
            "Flip SharpCut", "") },
        { "Split", new S_TextProperty(
            "분열", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>정신력 카드</Accent_Basic> 1장 당 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>의 10배만큼 피해를 줍니다.",
            "Split", "") },
        { "Split_Flip", new S_TextProperty(
            "뒤집힌 분열", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>정신력 카드</Accent_Basic>를 낼 때마다 <Accent_Basic>우선<sprite name=First></Accent_Basic>을 1 얻습니다.",
            "Flip Split", "") },
        { "WingsOfFreedom", new S_TextProperty(
            "자유의 날개", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>무게</Accent_Basic>를 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>와 같을 때까지 조정하고 조정한 <Accent_Basic>무게</Accent_Basic>만큼 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 얻습니다.",
            "WingsOfFreedom", "") },
        { "WingsOfFreedom_Flip", new S_TextProperty(
            "뒤집힌 자유의 날개", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>무게</Accent_Basic>가 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>에서 1만큼 차이나도 <Accent_Basic>완벽<sprite name=Perfect></Accent_Basic>을 얻을 수 있습니다.",
            "Flip WingsOfFreedom", "") },
        { "Accept", new S_TextProperty(
            "수긍", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic> 10 당 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>를 1 얻습니다. <Accent_Basic>완벽<sprite name=Perfect></Accent_Basic>이 아니라면 <Accent_Basic>과부하</Accent_Basic>됩니다.",
            "Accept", "") },
        { "Accept_Flip", new S_TextProperty(
            "뒤집힌 수긍", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>완벽<sprite name=Perfect></Accent_Basic>을 얻을 때마다 <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 8 얻습니다.",
            "Flip Accept", "") },
        { "PerfectForm", new S_TextProperty(
            "무결점", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>메아리</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>무게</Accent_Basic>가 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>보다 작다면 <Accent_Basic>무게</Accent_Basic>를 1 얻습니다.",
            "PerfectForm", "") },
        { "PerfectForm_Flip", new S_TextProperty(
            "뒤집힌 무결점", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>완벽<sprite name=Perfect></Accent_Basic>을 얻을 때마다 <Accent_Basic>냉혈<sprite name=ColdBlood></Accent_Basic>도 1 얻습니다.",
            "Flip PerfectForm", "") },
        { "DeepInsight", new S_TextProperty(
            "통찰", "<Accent_Basic>지속</Accent_Basic> : 얻는 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>이 3배 증가합니다.",
            "DeepInsight", "") },
        { "DeepInsight_Flip", new S_TextProperty(
            "뒤집힌 통찰", "<Accent_Basic>지속</Accent_Basic> : 적에게 피해를 줄 수 없지만 얻는 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>이 4배 증가합니다.",
            "Flip DeepInsight", "") },
        { "Dissolute", new S_TextProperty(
            "무절제", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>과 <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 곱한만큼 피해를 줍니다. <Accent_Basic>완벽<sprite name=Perfect></Accent_Basic>이 아니라면 <Accent_Basic>과부하</Accent_Basic>됩니다.",
            "Dissolute", "") },
        { "Dissolute_Flip", new S_TextProperty(
            "뒤집힌 무절제", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>과 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 곱한만큼 피해를 줍니다. <Accent_Basic>완벽<sprite name=Perfect></Accent_Basic>이 아니라면 <Accent_Basic>과부하</Accent_Basic>됩니다.",
            "Flip Dissolute", "") },
        { "Awakening", new S_TextProperty(
            "각성", "<Accent_Basic>발현</Accent_Basic> : 필드에 <Accent_Basic>정신력 카드</Accent_Basic> 3장 당 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>과 <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Awakening", "") },
        { "Awakening_Flip", new S_TextProperty(
            "뒤집힌 각성", "<Accent_Basic>발현</Accent_Basic> : 필드에 <Accent_Basic>정신력 카드</Accent_Basic> 3장 당 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>과 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Flip Awakening", "") },

        // 행운
        { "Chance", new S_TextProperty(
            "기회", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 1 얻습니다.",
            "Chance", "") },
        { "Chance_Flip", new S_TextProperty(
            "뒤집힌 기회", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>도약</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 2 얻습니다.",
            "Flip Chance", "") },
        { "SuddenStrike", new S_TextProperty(
            "기습 타격", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>행운<sprite name=Luck></Accent_Basic>의 6배만큼 피해를 줍니다.",
            "SuddenStrike", "") },
        { "SuddenStrike_Flip", new S_TextProperty(
            "뒤집힌 기습 타격", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>상태</Accent_Basic> 1 당 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>의 4배만큼 피해를 줍니다.",
            "Flip SuddenStrike", "") },
        { "Composure", new S_TextProperty(
            "여유부리기", "<Accent_Basic>지속</Accent_Basic> : <Accent_Basic>행운 카드</Accent_Basic>를 낼 때마다 <Accent_Basic>전개<sprite name=Expansion></Accent_Basic>를 1 얻습니다.",
            "Composure", "") },
        { "Composure_Flip", new S_TextProperty(
            "뒤집힌 여유부리기", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>상태</Accent_Basic> 1 당 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 3 얻습니다.",
            "Flip Composure", "") },
        { "SilentDomination", new S_TextProperty(
            "침묵의 정복", "<Accent_Basic>지속</Accent_Basic> : 오른쪽 카드 1장에 <Accent_Basic>행운<sprite name=Luck></Accent_Basic> 10 당 <Accent_Basic>메아리</Accent_Basic>를 1번 적용합니다.",
            "SilentDomination", "") },
        { "SilentDomination_Flip", new S_TextProperty(
            "뒤집힌 침묵의 정복", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>행운 카드</Accent_Basic> 1장 당 <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 2 얻습니다.",
            "Flip SilentDomination", "") },
        { "Grill", new S_TextProperty(
            "다그치기", "<Accent_Basic>지속</Accent_Basic> : <Accent_Basic>상태</Accent_Basic>를 얻을 수 없습니다.",
            "Grill", "") },
        { "Grill_Flip", new S_TextProperty(
            "뒤집힌 다그치기", "<Accent_Basic>지속</Accent_Basic> : 오른쪽에 있는 모든 카드에 <Accent_Basic>메아리</Accent_Basic>를 2번 적용하고 <Accent_Basic>과부하</Accent_Basic>도 적용합니다.",
            "Flip Grill", "") },
        { "CriticalBlow", new S_TextProperty(
            "치명타", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>행운 카드</Accent_Basic> 1장 당 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>의 10배만큼 피해를 줍니다.",
            "CriticalBlow", "") },
        { "CriticalBlow_Flip", new S_TextProperty(
            "뒤집힌 치명타", " <Accent_Basic>메아리</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>행운<sprite name=Luck></Accent_Basic>의 <Accent_AddPerRebound>4</Accent_AddPerRebound>배만큼 피해를 줍니다. <Accent_Basic>지속</Accent_Basic> : 이 카드의 발현 효과가 발동할 때마다 피해량이 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>의 4배만큼 증가합니다.",
            "Flip CriticalBlow", "") },
        { "Artifice", new S_TextProperty(
            "기교", "<Accent_Basic>지속</Accent_Basic> : 왼쪽에 있는 모든 <Accent_Basic>행운 카드</Accent_Basic>에 <Accent_Basic>메아리</Accent_Basic>를 1번 적용합니다.",
            "Artifice", "") },
        { "Artifice_Flip", new S_TextProperty(
            "뒤집힌 기교", "<Accent_Basic>지속</Accent_Basic> : <Accent_Basic>발현</Accent_Basic> 효과가 발동할 때마다 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>을 1 얻습니다.",
            "Flip Artifice", "") },
        { "ForcedTake", new S_TextProperty(
            "강탈", "<Accent_Basic>지속</Accent_Basic> : <Accent_Basic>발현</Accent_Basic> 효과가 발동할 때마다 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>의 5배만큼 피해를 줍니다.",
            "ForcedTake", "") },
        { "ForcedTake_Flip", new S_TextProperty(
            "뒤집힌 강탈", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>행운<sprite name=Luck></Accent_Basic> 10 당 <Accent_Basic>전개<sprite name=Expansion></Accent_Basic>, <Accent_Basic>우선<sprite name=First></Accent_Basic>, <Accent_Basic>냉혈<sprite name=ColdBlood></Accent_Basic> 중 하나를 1 얻습니다.",
            "Flip ForcedTake", "") },
        { "AllForOne", new S_TextProperty(
            "하나를 위한 모두", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 <Accent_AddPerRebound>1</Accent_AddPerRebound> 얻습니다. <Accent_Basic>지속</Accent_Basic> : 이 카드의 발현 효과가 발동할 때마다 능력치 획득량이 1 증가합니다.",
            "AllForOne", "") },
        { "AllForOne_Flip", new S_TextProperty(
            "뒤집힌 하나를 위한 모두", "<Accent_Basic>지속</Accent_Basic> : 얻는 <Accent_Basic>상태</Accent_Basic>가 2배 증가합니다.",
            "Flip AllForOne", "") },
        { "Disorder", new S_TextProperty(
            "무질서", "<Accent_Basic>지속</Accent_Basic> : 얻는 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>이 3배 증가합니다.",
            "Disorder", "") },
        { "Disorder_Flip", new S_TextProperty(
            "뒤집힌 무질서", "<Accent_Basic>지속</Accent_Basic> : 적에게 피해를 줄 수 없지만 얻는 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>이 4배 증가합니다.",
            "Flip Disorder", "") },
        { "Shake", new S_TextProperty(
            "떠보기", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>상태</Accent_Basic> 2 당 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>과 <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Shake", "") },
        { "Shake_Flip", new S_TextProperty(
            "뒤집힌 떠보기", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>상태</Accent_Basic> 2 당 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>과 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Flip Shake", "") },
        { "FatalBlow", new S_TextProperty(
            "결정타", "<Accent_Basic>발현</Accent_Basic> : 필드에 <Accent_Basic>행운 카드</Accent_Basic> 3장 당 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>과 <Accent_Basic>힘<sprite name=Str></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "FatalBlow", "") },
        { "FatalBlow_Flip", new S_TextProperty(
            "뒤집힌 결정타", "<Accent_Basic>발현</Accent_Basic> : 필드에 <Accent_Basic>행운 카드</Accent_Basic> 3장 당 <Accent_Basic>행운<sprite name=Luck></Accent_Basic>과 <Accent_Basic>정신력<sprite name=Mind></Accent_Basic>을 곱한만큼 피해를 줍니다.",
            "Flip FatalBlow", "") },

        // 공용
        { "Plunder", new S_TextProperty(
            "약탈", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 1 얻습니다.",
            "Plunder", "") },
        { "Plunder_Flip", new S_TextProperty(
            "뒤집힌 약탈", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>망상<sprite name=Delusion></Accent_Basic>을 1 얻고 <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 2 얻습니다.",
            "Flip Plunder", "") },
        { "Inspiration", new S_TextProperty(
            "영감", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>전개<sprite name=Expansion></Accent_Basic>를 1 얻습니다.",
            "Inspiration", "") },
        { "Inspiration_Flip", new S_TextProperty(
            "뒤집힌 영감", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : 유리한 쪽으로 <Accent_Basic>무게</Accent_Basic>를 1 또는 11 얻습니다.",
            "Flip Inspiration", "") },
        { "Trinity", new S_TextProperty(
            "삼위일체", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>힘 카드</Accent_Basic>와 <Accent_Basic>공용 카드</Accent_Basic>를 같은 카드로 취급합니다.",
            "Trinity", "") },
        { "Trinity_Flip", new S_TextProperty(
            "뒤집힌 삼위일체", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>정신력 카드</Accent_Basic>와 <Accent_Basic>행운 카드</Accent_Basic>를 같은 카드로 취급합니다.",
            "Flip Trinity", "") },
        { "Corrupt", new S_TextProperty(
            "감염", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>속전속결</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>체력<sprite name=Health></Accent_Basic>을 2 잃습니다. <Accent_Basic>지속</Accent_Basic> : 모든 피해가 2배 증가합니다.",
            "Corrupt", "") },
        { "Corrupt_Flip", new S_TextProperty(
            "뒤집힌 감염", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : 왼쪽에 있는 모든 카드에 <Accent_Basic>면역</Accent_Basic>을 적용합니다.",
            "Flip Corrupt", "") },
        { "Resistance", new S_TextProperty(
            "저항", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>한계<sprite name=Limit></Accent_Basic>를 1 얻습니다.",
            "Resistance", "") },
        { "Resistance_Flip", new S_TextProperty(
            "뒤집힌 저항", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>속전속결</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>한계<sprite name=Limit></Accent_Basic>를 2 잃습니다. <Accent_Basic>지속</Accent_Basic> : 모든 피해가 2배 증가합니다.",
            "Flip Resistance", "") },
        { "Adventure", new S_TextProperty(
            "모험", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>우선<sprite name=First></Accent_Basic>을 1 얻습니다.",
            "Adventure", "") },
        { "Adventure_Flip", new S_TextProperty(
            "뒤집힌 모험", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 2 얻습니다.",
            "Flip Adventure", "") },
        { "Imitate", new S_TextProperty(
            "흉내", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : 이번 턴에 처음 낸 카드에 <Accent_Basic>메아리</Accent_Basic>를 2번 적용합니다.",
            "Imitate", "") },
        { "Imitate_Flip", new S_TextProperty(
            "뒤집힌 흉내", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>공용 카드</Accent_Basic> 1장 당 <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 1 얻습니다.",
            "Flip Imitate", "") },
        { "Undertow", new S_TextProperty(
            "역류", "<Accent_Basic>지속</Accent_Basic> : 능력치를 얻을 수 없지만 필드에 있는 모든 카드에 <Accent_Basic>메아리</Accent_Basic>를 1번 적용합니다.",
            "Undertow", "") },
        { "Undertow_Flip", new S_TextProperty(
            "뒤집힌 역류", "<Accent_Basic>지속</Accent_Basic> : 적에게 피해를 줄 수 없지만 필드에 있는 모든 카드에 <Accent_Basic>메아리</Accent_Basic>를 1번 적용합니다.",
            "Flip Undertow", "") },
        { "Blasphemy", new S_TextProperty(
            "모독", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>냉혈<sprite name=ColdBlood></Accent_Basic>을 1 얻습니다.",
            "Blasphemy", "") },
        { "Blasphemy_Flip", new S_TextProperty(
            "뒤집힌 모독", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : 필드에 있는 카드가 3장 이하라면 모든 카드에 <Accent_Basic>메아리</Accent_Basic>를 2번 적용합니다.",
            "Flip Blasphemy", "") },
        { "Realization", new S_TextProperty(
            "깨달음", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : 왼쪽에 있는 카드 1장 당 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>를 1 얻고 오른쪽에 있는 카드 1장 당 <Accent_Basic>무게</Accent_Basic>를 2 얻습니다.",
            "Realization", "") },
        { "Realization_Flip", new S_TextProperty(
            "뒤집힌 깨달음", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 4 얻습니다.",
            "Flip Realization", "") },
        { "Berserk", new S_TextProperty(
            "광란", "<Accent_Basic>지속</Accent_Basic> : 능력치를 얻을 수 없지만 모든 피해가 2배 증가합니다.",
            "Berserk", "") },
        { "Berserk_Flip", new S_TextProperty(
            "뒤집힌 광란", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>속전속결</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : 카드를 낼 때마다 <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>를 1 잃지만 모든 피해가 2배 증가합니다.",
            "Flip Berserk", "") },
        { "Balance", new S_TextProperty(
            "균형잡기", "<Accent_Basic>발현</Accent_Basic> : 필드에 있는 <Accent_Basic>저주받은 카드</Accent_Basic>를 모두 <Accent_Basic>저주 해제</Accent_Basic>하고 <Accent_Basic>저주받지 않은 카드</Accent_Basic>를 모두 <Accent_Basic>저주</Accent_Basic>합니다.",
            "Balance", "") },
        { "Balance_Flip", new S_TextProperty(
            "뒤집힌 균형잡기", "<Accent_Basic>지속</Accent_Basic> : <Accent_Basic>저주받은 카드</Accent_Basic>를 낼 때 고정적으로 <Accent_Basic>무게</Accent_Basic>를 2만 얻습니다.",
            "Flip Balance", "") },
        { "LastStruggle", new S_TextProperty(
            "최후의 발악", "<Accent_Basic>과부하</Accent_Basic>. <Accent_Basic>발현</Accent_Basic> : <Accent_Basic>전개<sprite name=Expansion></Accent_Basic>, <Accent_Basic>우선<sprite name=First></Accent_Basic>, <Accent_Basic>냉혈<sprite name=ColdBlood></Accent_Basic>을 2 얻습니다.",
            "LastStruggle", "") },
        { "LastStruggle_Flip", new S_TextProperty(
            "뒤집힌 최후의 발악", "<Accent_Basic>고정</Accent_Basic>. <Accent_Basic>속전속결</Accent_Basic>. <Accent_Basic>지속</Accent_Basic> : <Accent_Basic>버스트</Accent_Basic> 시 피해 감소를 무시하지만 <Accent_Basic>완벽</Accent_Basic> 시 피해 증가도 무시합니다.",
            "Flip LastStruggle", "") },
        { "Carnage", new S_TextProperty(
            "대학살", "<Accent_Basic>지속</Accent_Basic> : 능력치를 얻을 수 없지만 모든 피해가 3배 증가합니다.",
            "Carnage", "") },
        { "Carnage_Flip", new S_TextProperty(
            "뒤집힌 대학살", "<Accent_Basic>지속</Accent_Basic> : 적에게 피해를 줄 수 없지만 얻는 <Accent_Basic>모든 능력치<sprite name=Str><sprite name=Mind><sprite name=Luck></Accent_Basic>가 3배 증가합니다.",
            "Flip Carnage", "") },

        // 적
        { "Condemnation", new S_TextProperty(
            "단죄", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>체력<sprite name=Health></Accent_Basic>을 1 잃습니다.",
            "Condemnation", "") },
        { "Execution", new S_TextProperty(
            "처형", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>체력<sprite name=Health></Accent_Basic>을 2 잃습니다.",
            "Execution", "") },
        { "Judgement", new S_TextProperty(
            "심판", "<Accent_Basic>발현</Accent_Basic> : <Accent_Basic>한계<sprite name=Limit></Accent_Basic>와 <Accent_Basic>무게</Accent_Basic>의 차이만큼 <Accent_Basic>체력<sprite name=Health></Accent_Basic>을 잃습니다.",
            "Judgement", "") },
        { "Retribution", new S_TextProperty(
            "응징", "<Accent_Basic>발현</Accent_Basic> : 버스트라면 모든 <Accent_Basic>체력<sprite name=Health></Accent_Basic>을 잃습니다.",
            "Retribution", "") },
    };
    public static readonly Dictionary<string, S_TextProperty> TermDatas = new()
    {
        { "Perfect", new S_TextProperty("완벽<sprite name=Perfect>", "<Accent_Basic>무게</Accent_Basic>가 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>와 같아 내가 가하는 피해량이 2배 증가합니다. 또한 카드를 낼 수 없습니다.", "Perfect", "") },
        { "Burst", new S_TextProperty("버스트<sprite name=Burst>", "<Accent_Basic>무게</Accent_Basic>가 <Accent_Basic>한계<sprite name=Limit></Accent_Basic>를 초과하여 내가 가하는 피해량이 0.5배가 되고 내가 받는 피해량이 2배 증가합니다. 또한 카드를 낼 수 없습니다.", "Burst", "") },
        { "CursedCard", new S_TextProperty("저주받은 카드", "이 카드는 <Accent_Basic>발현</Accent_Basic> 및 <Accent_Basic>지속</Accent_Basic> 효과가 발동하지 않습니다.", "CursedCard", "") },
        { "Curse", new S_TextProperty("저주", "<Accent_Basic>발현</Accent_Basic> 및 <Accent_Basic>지속</Accent_Basic> 효과가 발동하지 않습니다.", "Curse", "") },
        { "Delusion", new S_TextProperty("망상<sprite name=Delusion>", "다음에 내는 카드가 <Accent_Basic>저주</Accent_Basic>받는 해로운 <Accent_Basic>상태</Accent_Basic>입니다.", "Delusion", "") },
        { "Expansion", new S_TextProperty("전개<sprite name=Expansion>", "카드를 낼 때 카드 보기가 2장 증가하는 이로운 <Accent_Basic>상태</Accent_Basic>입니다.", "Expansion", "") },
        { "First", new S_TextProperty("우선<sprite name=First>", "카드를 낼 때 최대한 <Accent_Basic>완벽</Accent_Basic>이 되는 <Accent_Basic>무게</Accent_Basic>의 카드를 낼 수 있는 이로운 <Accent_Basic>상태</Accent_Basic>입니다.", "First", "") },
        { "ColdBlood", new S_TextProperty("냉혈<sprite name=ColdBlood>", "다음에 내는 카드가 <Accent_Basic>무게</Accent_Basic>를 증가시키지 않는 이로운 <Accent_Basic>상태</Accent_Basic>입니다.", "ColdBlood", "") },
        { "Persist", new S_TextProperty("지속", "필드에 있을 때 항상 적용되는 효과입니다.", "Persist", "") },
        { "Unleash", new S_TextProperty("발현", "<Accent_Basic>스탠드</Accent_Basic> 시 효과가 발동합니다.", "Unleash", "") },
        { "Overload", new S_TextProperty("과부하", "턴 종료 시 <Accent_Basic>저주</Accent_Basic>받습니다.", "Overload", "") },
        { "Immunity", new S_TextProperty("면역", "<Accent_Basic>저주</Accent_Basic>에 걸리지 않습니다.", "Immunity", "") },
        { "QuickAction", new S_TextProperty("속전속결", "시련 시작 시 이 카드를 냅니다.", "QuickAction", "") },
        { "Rebound", new S_TextProperty("메아리", "<Accent_Basic>발현</Accent_Basic> 효과가 1번 더 발동합니다.", "Rebound", "") },
        { "Fix", new S_TextProperty("고정", "턴 종료 후 사용한 카드 더미로 돌아가지 않고 필드에 남습니다.", "Fix", "") },
        { "Flexible", new S_TextProperty("유연", "<Accent_Basic>스탠드</Accent_Basic> 시 필드의 가장 오른쪽으로 이동합니다.", "Flexible", "") },
        { "Leap", new S_TextProperty("도약", "<Accent_Basic>스탠드</Accent_Basic> 시 필드의 가장 왼쪽으로 이동합니다.", "Leap", "") },
        { "Rebirth", new S_TextProperty("윤회", "턴 종료 후 사용한 카드 더미 대신 덱으로 돌아갑니다.", "Rebirth", "") },
        { "Dismantle", new S_TextProperty("분해", "이 카드의 <Accent_Basic>무게</Accent_Basic>가 0이 됩니다.", "Dismantle", "") },
        { "Mask", new S_TextProperty("가면", "이 카드는 어떤 <Accent_Basic>타입</Accent_Basic>이든 될 수 있습니다.", "Mask", "") },
    };
    #endregion
}
public struct S_TextProperty
{
    public string Name_Korean;
    public string Description_Korean;
    public string Name_English;
    public string Description_English;

    public S_TextProperty(string name_Korean, string description_Korean, string name_English, string description_English)
    {
        Name_Korean = name_Korean;
        Description_Korean = description_Korean;
        Name_English = name_English;
        Description_English = description_English;
    }
}