using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_SkillList : MonoBehaviour
{
    // ��� ����ǰ ����Ʈ
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

    // �̱���
    static S_SkillList instance;
    public static S_SkillList Instance { get { return instance; } }
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

    public List<S_Skill> GetInitSkillsByStartGame() // �ʱ� ����ǰ ����
    {
        // ������ Ű ���(���� �ɷ�)
        HashSet<string> targetKeys = new() { "Skill_FocuesBreath", "Skill_Cull" }; // ������ ȣ��, ����

        // Ư�� Ű�� ���� ��Ҹ� �����ϸ鼭 ���� ����Ʈ���� ����
        List<S_Skill> initSkills = skills.Where(r => targetKeys.Contains(r.Key)).Select(x => x.Clone()).ToList();

        return initSkills;
    }
    public List<S_Skill> PickRandomSkills(int count)
    {
        // �÷��̾ �������� ���� �ɷ� ����Ʈ �����
        List<S_Skill> pickAvailableSkills = skills.Where(l => !S_PlayerSkill.Instance.OwnedSkills.Any(o => o.Key == l.Key)).ToList();

        // count ������
        count = Mathf.Min(count, pickAvailableSkills.Count);

        // ����
        List<S_Skill> shuffledList = pickAvailableSkills.OrderBy(x => Random.value).ToList();

        // ������ �ɷ��� count��ŭ ����
        List<S_Skill> randomSkills = shuffledList.Take(count).ToList();

        return randomSkills;
    }
}