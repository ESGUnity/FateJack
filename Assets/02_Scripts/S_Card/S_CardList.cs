using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_CardList
{
    public static List<S_CardBase> cards = new List<S_CardBase>()
    {
        new S_Stimulus(), new S_Stimulus_Flip(), new S_WrathStrike(), new S_WrathStrike_Flip(), new S_SinisterImpulse(), new S_SinisterImpulse_Flip(),
        new S_FlowingSin(), new S_FlowingSin_Flip(), new S_EngulfinFlames(), new S_EngulfinFlames_Flip(), new S_UntappedPower(), new S_UntappedPower_Flip(),
        new S_CalamityApproaches(), new S_CalamityApproaches_Flip(), new S_UnjustSacrifice(), new S_UnjustSacrifice_Flip(), new S_FinishingStrike(), new S_FinishingStrike_Flip(),
        new S_ZenithBreak(), new S_ZenithBreak_Flip(), new S_Grudge(), new S_Grudge_Flip(), new S_BindingForce(), new S_BindingForce_Flip(),

        new S_Focus(), new S_Focus_Flip(), new S_PreciseStrike(), new S_PreciseStrike_Flip(), new S_Drain(), new S_Drain_Flip(), 
        new S_Unshackle(), new S_Unshackle_Flip(), new S_SharpCut(), new S_SharpCut_Flip(), new S_Split(), new S_Split_Flip(),
        new S_WingsOfFreedom(), new S_WingsOfFreedom_Flip(), new S_Accept(), new S_Accept_Flip(), new S_PerfectForm(), new S_PerfectForm_Flip(),
        new S_DeepInsight(), new S_DeepInsight_Flip(), new S_Dissolute(), new S_Dissolute_Flip(), new S_Awakening(), new S_Awakening_Flip(),

        new S_Chance(), new S_Chance_Flip(), new S_SuddenStrike(), new S_SuddenStrike_Flip(), new S_Composure(), new S_Composure_Flip(),
        new S_SilentDomination(), new S_SilentDomination_Flip(),  new S_Grill(), new S_Grill_Flip(), new S_CriticalBlow(), new S_CriticalBlow_Flip(), 
        new S_Artifice(), new S_Artifice_Flip(), new S_ForcedTake(), new S_ForcedTake_Flip(), new S_AllForOne(), new S_AllForOne_Flip(), 
        new S_Disorder(), new S_Disorder_Flip(), new S_Shake(), new S_Shake_Flip(), new S_FatalBlow(), new S_FatalBlow_Flip(),

        new S_Plunder(), new S_Plunder_Flip(), new S_Inspiration(), new S_Inspiration_Flip(), new S_Trinity(), new S_Trinity_Flip(),
        new S_Corrupt(), new S_Corrupt_Flip(), new S_Resistance(), new S_Resistance_Flip(), new S_Adventure(), new S_Adventure_Flip(),
        new S_Imitate(), new S_Imitate_Flip(), new S_Undertow(), new S_Undertow_Flip(), new S_Blasphemy(), new S_Blasphemy_Flip(),
        new S_Realization(), new S_Realization_Flip(), new S_Berserk(), new S_Berserk_Flip(), new S_Balance(), new S_Balance_Flip(),
        new S_LastStruggle(), new S_LastStruggle_Flip(), new S_Carnage(), new S_Carnage_Flip(),
    };

    public static List<S_CardBase> GetInitCardsByStartGame() // 초기 카드
    {
        // 추출할 키 목록(시작 능력)
        HashSet<string> targetKeys = new() { "Stimulus", "WrathStrike", "Focus", "PreciseStrike", "Chance", "SuddenStrike", };

        // 특정 키를 가진 요소를 추출
        List<S_CardBase> initCards = cards.ToList().Where(r => targetKeys.Contains(r.Key)).ToList();

        // 클론하여 클래스 반환
        List<S_CardBase> clones = new();
        foreach (S_CardBase card in initCards)
        {
            S_CardBase clone = S_DeepCloner.DeepClone(card);
            clones.Add(clone);
        }

        return clones;
    }
    public static List<S_CardBase> PickRandomCards(int count, S_CardTypeEnum type)
    {
        // cards 리스트 복사
        List<S_CardBase> copyList = new();
        if (type != S_CardTypeEnum.None)
        {
            copyList = cards.Where(x => x.CardType == type).ToList();
        }
        else
        {
            copyList = cards.ToList();
        }

        // Fisher–Yates 셔플
        for (int i = copyList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (copyList[i], copyList[j]) = (copyList[j], copyList[i]);
        }

        // 앞에서 safeCount개 선택하고 깊은 복사해서 반환
        return copyList.Take(count).Select(card => S_DeepCloner.DeepClone(card)).ToList();
    }
}
