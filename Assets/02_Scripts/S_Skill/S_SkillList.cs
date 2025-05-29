using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_SkillList : MonoBehaviour
{
    // 모든 전리품 리스트
    List<S_Skill> skills = new List<S_Skill>()
    {
        new Skill_ReadyToBattle(), 
        new Skill_UnstableFusion(),
        new Skill_RoarCry(),
        new Skill_FocusBreath(), 
        new Skill_PowerBuild(),
        new Skill_Overwhelm(),
        new Skill_Cull(),
        new Skill_SavageStrike(),
        new Skill_Abracadabra(),
        new Skill_Pummel(),
        new Skill_QuadBlade(),
    };

    // 싱글턴
    static S_SkillList instance;
    public static S_SkillList Instance { get { return instance; } }
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

    public List<S_Skill> GetInitSkillsByStartGame() // 초기 전리품 선택
    {
        // 추출할 키 목록(시작 능력)
        HashSet<string> targetKeys = new() { "Skill_FocuesBreath", "Skill_Cull" }; // 전집중 호흡, 도륙

        // 특정 키를 가진 요소를 추출하면서 원본 리스트에서 제거
        List<S_Skill> initSkills = skills.Where(r => targetKeys.Contains(r.Key)).Select(x => x.Clone()).ToList();

        return initSkills;
    }
    public List<S_Skill> PickRandomSkills(int count)
    {
        // 플레이어가 보유하지 않은 능력 리스트 만들기
        List<S_Skill> pickAvailableSkills = skills.Where(l => !S_PlayerSkill.Instance.OwnedSkills.Any(o => o.Key == l.Key)).ToList();

        // count 방어로직
        count = Mathf.Min(count, pickAvailableSkills.Count);

        // 셔플
        List<S_Skill> shuffledList = pickAvailableSkills.OrderBy(x => Random.value).ToList();

        // 무작위 능력을 count만큼 선택
        List<S_Skill> randomSkills = shuffledList.Take(count).ToList();

        return randomSkills;
    }
}