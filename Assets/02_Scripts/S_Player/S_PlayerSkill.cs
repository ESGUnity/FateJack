using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerSkill : MonoBehaviour
{
    // ����ǰ ����
    [HideInInspector] public List<S_Skill> OwnedSkills = new();
    public const int MAX_LOOT = 5;

    // ������Ʈ
    S_PlayerCard playerCard;
    S_PlayerStat playerStat;

    // �̱���
    static S_PlayerSkill instance;
    public static S_PlayerSkill Instance { get { return instance; } }
    void Awake()
    {
        // ������Ʈ �Ҵ�
        playerCard = GetComponent<S_PlayerCard>();
        playerStat = GetComponent<S_PlayerStat>();

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
            S_InGameUISystem.Instance.CreateLog("�ɷ��� ���̻� ȹ���� �� �����ϴ�.");
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

            // ���� ���ÿ� ī�尡 ���ٸ�, �� ī�带 ���� ActivatedCount�� Ȱ��ȭ�Ȱ� �ƴ϶�� �׳� 0. �̰� ���ϸ� ActivatedCount�� �ִ�ġ�� �� �� �ִ�.
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
        // �÷� ���� �ø� ȿ�� �ߵ�
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.StartTrial)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, null);
                s.IsMeetCondition();
                S_SkillInfoSystem.Instance.UpdateSkillObject();
            }
        }

        // �ʷϺ� üũ(�Ϻ� �޾Ƹ����� ����)
        CheckSkillMeetCondition();
        S_SkillInfoSystem.Instance.UpdateSkillObject();
    }
    public async Task ActivateReverbSkillsByHitCard(S_Card hitCard)
    {
        // �޾Ƹ��� ȿ�� �ߵ�
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.Reverb && s.CanActivateEffect)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, hitCard);
                s.IsMeetCondition();
                S_SkillInfoSystem.Instance.UpdateSkillObject();
            }
        }

        // �ʷϺ� üũ(�Ϻ� �޾Ƹ����� ����)
        CheckSkillMeetCondition();
        S_SkillInfoSystem.Instance.UpdateSkillObject();

        // �ɷ¿� ���� �����丮 ����
        S_PlayerStat.Instance.SaveStatHistory(hitCard, S_StatHistoryTriggerEnum.Skill);
    }
    public async Task ActivateStandSkillsByStand()
    {
        // ���ĵ常 ȿ�� �ߵ�
        foreach (S_Skill s in GetPlayerOwnedSkills())
        {
            if (s.Condition == S_SkillConditionEnum.Stand && s.CanActivateEffect)
            {
                await s.ActiveSkill(S_EffectActivator.Instance, null);
                s.IsMeetCondition();
                S_SkillInfoSystem.Instance.UpdateSkillObject();
            }
        }

        // �ʷϺ� üũ(������ ���ĵ�� �Ҳ���)
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