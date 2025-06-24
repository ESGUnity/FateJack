using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_TrinketList
{
    // 모든 전리품 리스트
    static List<S_Trinket> trinkets = new List<S_Trinket>()
    {
        new S_BrokenBowl(), new S_Hatchet(), new S_RedAmulet(),
        new S_SacredFeather(), new S_AzureGrail(), new S_YellowGrail(),
        new S_ShortWhip(), new S_GamblerDice(), new S_BrokenBugle(),

        new S_Booze(), new S_FruitLiquor(), new S_Bitters(), new S_RubbingAlcohol(),
        new S_Wine(), new S_Spirits(), new S_OakAgedWine(), new S_DarkBeer(), 
        new S_Dumbbell(), new S_Bookmark(), new S_Lotto(), 

        new S_ThornGripBornSaw(), new S_MomusBlade(), new S_GraveKeeperShovel(), new S_HeftyGreatSword(), new S_FrostSpike(), new S_TwinBlades(), new S_Abracadabra(),

        new S_Incinerator(), new S_OneiroiGrasp(), new S_DarkHold(),

        new S_FruitCandy(), new S_CoffeeFlavoredGum(), new S_BubbleGum(), new S_Xylitor(), new S_MintCandy(),
        new S_CokeFlavoredJelly(),
        new S_Soda(), new S_AlertnessGum(), new S_RooibosTea(), new S_ZeroSugaSoda(),

        new S_NovelBook(), new S_SomeoneDiary(), 
        new S_CartWheel(), new S_FadedPhoto(), new S_PlateArmor(), new S_NightfallHood(), 

        new S_ErisPuzzle(), new S_ThanatosPuzzle(), new S_Eraser(), new S_MinosMemento(),
    };

    public static List<S_Trinket> GetInitSkillsByStartGame() // 초기 전리품 선택
    {
        // 추출할 키 목록(시작 능력)
        HashSet<string> targetKeys = new() { "BrokenBowl" }; // 과일 사탕(전개)

        // 특정 키를 가진 요소를 추출
        List<S_Trinket> initTrinket = trinkets.ToList().Where(r => targetKeys.Contains(r.Key)).ToList();

        // 클론하여 클래스 반환
        List<S_Trinket> clones = new();
        foreach (S_Trinket tri in initTrinket)
        {
            S_Trinket clone = tri.Clone();
            clones.Add(clone);
        }

        return clones;
    }
    public static List<S_Trinket> PickRandomSkills(int count)
    {
        // 플레이어가 보유하지 않은 능력 리스트 만들기
        List<S_Trinket> pickAvailableSkills = trinkets.Where(l => !S_PlayerTrinket.Instance.OwnedTrinketList.Any(o => o.Key == l.Key)).ToList();

        // count 방어로직
        count = Mathf.Min(count, pickAvailableSkills.Count);

        // 셔플
        List<S_Trinket> shuffledList = pickAvailableSkills.OrderBy(x => Random.value).ToList();

        // 무작위 능력을 count만큼 선택
        List<S_Trinket> randomSkills = shuffledList.Take(count).ToList();

        List<S_Trinket> clones = new();
        foreach (S_Trinket s in randomSkills)
        {
            S_Trinket clone = s.Clone();
            clones.Add(clone);
        }

        return clones;
    }
}