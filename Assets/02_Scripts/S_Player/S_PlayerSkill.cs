using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerSkill : MonoBehaviour
{
    // 전리품 관련
    [HideInInspector] public List<S_Skill> OwnedSkills = new();
    public const int MAX_LOOT = 6;

    // 컴포넌트
    S_PlayerCard playerCard;
    S_PlayerStat playerStat;

    // 싱글턴
    static S_PlayerSkill instance;
    public static S_PlayerSkill Instance { get { return instance; } }

    void Awake()
    {
        // 컴포넌트 할당
        playerCard = GetComponent<S_PlayerCard>();
        playerStat = GetComponent<S_PlayerStat>();

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

    public void InitSkillsByStartGame()
    {
        foreach (S_Skill loot in S_SkillList.Instance.GetInitSkillsByStartGame())
        {
            AddSkill(loot);
        }
    }
    public void AddSkill(S_Skill skill)
    {
        if (CanAddSkill())
        {
            OwnedSkills.Add(skill);
            S_SkillInfoSystem.Instance.AddSkillObject(skill);
            SetSiblingUISkills();
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("능력을 더이상 획득할 수 없습니다.");
        }
    }
    public void RemoveSkill(S_Skill skill)
    {
        OwnedSkills.Remove(skill);
        S_SkillInfoSystem.Instance.RemoveSkillObject(skill);
    }
    public void ChangeSkillIndexToFirst(S_Skill skill)
    {
        var s = OwnedSkills[0];
        int index = OwnedSkills.IndexOf(skill);
        OwnedSkills[0] = skill;
        OwnedSkills[index] = s;

        SetSiblingUISkills();
    }
    public void SetSiblingUISkills()
    {
        for (int i = 0; i < OwnedSkills.Count; i++)
        {
            var targetSkill = OwnedSkills[i];

            // 자식들 중에서 SkillInfo가 이 Skill인 거 찾음
            var t = S_SkillInfoSystem.Instance.layoutGroup_SkillInfoBase.transform
                .Cast<Transform>()
                .FirstOrDefault(tr => tr.GetComponent<S_UISkill>()?.SkillInfo == targetSkill);

            if (t != null) t.SetSiblingIndex(i);
        }
    }
    public bool CanAddSkill()
    {
        return OwnedSkills.Count < MAX_LOOT;
    }
    public List<S_Skill> GetPlayerOwnedSkills()
    {
        return OwnedSkills.ToList();
    }

    public void ResetSkillActivatedCountByEndTrial()
    {
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Passive == S_SkillPassiveEnum.NeedActivatedCount)
            {
                s.ActivatedCount = 0;
            }
        }
    }
    public void CheckSkillMeetCondition(S_Card card = null)
    {
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Passive == S_SkillPassiveEnum.NeedActivatedCount)
            {
                s.CheckMeetConditionByActivatedCount(card);
            }
            else
            {
                s.CheckMeetConditionByBasic(card);
            }
        }

        S_SkillInfoSystem.Instance.UpdateSkillObject();
    }
    public void CheckSkillMeetConditionAfterEffect(S_Skill skill)
    {
        //if (skill.Passive == S_SkillPassiveEnum.NeedActivatedCount)
        //{
        //    skill.CheckMeetConditionByActivatedCount();
        //}
        //else
        //{
        //    skill.CheckMeetConditionByBasic();
        //}
        skill.CheckMeetConditionByBasic();

        S_SkillInfoSystem.Instance.UpdateSkillObject();
    }
    public async Task ActivateStartTrialSkillsByStartTrial()
    {
        // 시련 시작 시만 효과 발동
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.StartTrial)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, null);

                CheckSkillMeetConditionAfterEffect(s);
            }
        }
    }
    public async Task ActivateReverbSkillsByHitCard(S_Card hitCard)
    {
        // 메아리만 효과 발동
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.Reverb && s.IsMeetCondition)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, hitCard);

                CheckSkillMeetConditionAfterEffect(s);
            }
        }

        // 능력에 의한 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(hitCard, S_StatHistoryTriggerEnum.Skill);
    }
    public async Task ActivateStandSkillsByStand()
    {
        // 스탠드만 효과 발동
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.Stand && s.IsMeetCondition)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, null);

                CheckSkillMeetConditionAfterEffect(s);
            }
        }
    }
}

public enum S_SkillConditionEnum
{
    None,
    StartTrial,
    Reverb,
    Stand
}
public enum S_SkillPassiveEnum
{
    None,
    Strength_MultiToHarm,
    Mind_MultiToHarm,
    Luck_MultiToHarm,
    SameBlack,
    SameRed,
    Spade_MindAndLuck,
    Clover_Strength,
    Clover_Mind,
    Clover_Luck,
    AddProductCount,
    NeedActivatedCount,
}