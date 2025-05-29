using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerSkill : MonoBehaviour
{
    // 전리품 관련
    [HideInInspector] public List<S_Skill> OwnedSkills = new();
    public const int MAX_LOOT = 5;

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
    public void AddSkill(S_Skill loot)
    {
        if (CanAddSkill())
        {
            OwnedSkills.Add(loot);
            S_SkillInfoSystem.Instance.AddSkillObject(loot);
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("능력을 더이상 획득할 수 없습니다.");
        }
    }
    public void RemoveSkill(S_Skill loot)
    {
        OwnedSkills.Remove(loot);
        S_SkillInfoSystem.Instance.RemoveSkillObject(loot);
    }
    public bool CanAddSkill()
    {
        return OwnedSkills.Count < MAX_LOOT;
    }
    public List<S_Skill> GetPlayerOwnedSkills()
    {
        return OwnedSkills.ToList();
    }

    public void CalcSkillActivatedCountByHit(S_Card card)
    {
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Passive == S_SkillPassiveEnum.NeedActivatedCount)
            {
                s.ActivateCount(card);
            }
        }
    }
    public void SubtractSkillActivatedCountByTwist(List<S_Card> cards)
    {
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            foreach (S_Card card in cards)
            {
                if (s.Passive == S_SkillPassiveEnum.NeedActivatedCount)
                {
                    s.ActivateCount(card, true);
                }
            }

            // 만약 스택에 카드가 없다면, 즉 카드를 내서 ActivatedCount가 활성화된게 아니라면 그냥 0. 이거 안하면 ActivatedCount가 최대치로 갈 수 있다.
            if (S_PlayerCard.Instance.GetPreStackCards().Count <= 0)
            {
                s.ActivatedCount = 0;
            }
        }
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
            s.IsMeetCondition(card);
        }
    }
    public async Task ActivateStartTrialSkillsByStartTrial()
    {
        // 시련 시작 시만 효과 발동
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.StartTrial)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, null);
                s.IsMeetCondition();
                S_SkillInfoSystem.Instance.UpdateSkillObject();
            }
        }

        // 초록불 체크(일부 메아리류는 꺼짐)
        CheckSkillMeetCondition();
        S_SkillInfoSystem.Instance.UpdateSkillObject();
    }
    public async Task ActivateReverbSkillsByHitCard(S_Card hitCard)
    {
        // 메아리만 효과 발동
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.Reverb && s.CanActivateEffect)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, hitCard);
                s.IsMeetCondition();
                S_SkillInfoSystem.Instance.UpdateSkillObject();
            }
        }

        // 초록불 체크(일부 메아리류는 꺼짐)
        CheckSkillMeetCondition();
        S_SkillInfoSystem.Instance.UpdateSkillObject();

        // 능력에 의한 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(hitCard, S_StatHistoryTriggerEnum.Skill);
    }
    public async Task ActivateStandSkillsByStand()
    {
        // 스탠드만 효과 발동
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.Stand && s.CanActivateEffect)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, null);
                s.IsMeetCondition();
                S_SkillInfoSystem.Instance.UpdateSkillObject();
            }
        }

        // 초록불 체크(범람류 스탠드는 불꺼짐)
        CheckSkillMeetCondition();
        S_SkillInfoSystem.Instance.UpdateSkillObject();
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
    NeedActivatedCount,
}