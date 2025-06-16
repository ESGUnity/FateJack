using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class S_TrinketList
{
    // 모든 전리품 리스트
    static List<S_Trinket> skills = new List<S_Trinket>()
    {
        new S_BrokenBowl(), new S_Hatchet(), new S_RedAmulet()
    };

    public static List<S_Trinket> GetInitSkillsByStartGame() // 초기 전리품 선택
    {
        // 추출할 키 목록(시작 능력)
        HashSet<string> targetKeys = new() { "Skill_Cull" }; // 도륙

        // 특정 키를 가진 요소를 추출
        List<S_Trinket> initSkills = skills.ToList().Where(r => targetKeys.Contains(r.Key)).ToList();

        // 클론하여 클래스 반환
        List<S_Trinket> clones = new();
        foreach (S_Trinket skill in initSkills)
        {
            S_Trinket clone = skill.Clone();
            clone.SubscribeGameFlowManager();
            clones.Add(clone);
        }

        return clones;
    }
    public static List<S_Trinket> PickRandomSkills(int count)
    {
        // 플레이어가 보유하지 않은 능력 리스트 만들기
        List<S_Trinket> pickAvailableSkills = skills.Where(l => !S_PlayerTrinket.Instance.OwnedTrinketList.Any(o => o.Key == l.Key)).ToList();

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
            clone.SubscribeGameFlowManager();
            clones.Add(clone);
        }

        return clones;
    }
}