using System.Threading.Tasks;

public class S_Skill
{
    public string Key;
    public string Name;
    public string Description;
    public S_SkillConditionEnum Condition;
    public S_SkillPassiveEnum Passive;
    public int ActivatedCount;

    public bool IsAccumulate; // 성장형이냐?
    public int CurrentAccumulateValue;
    public int TrialAccumulateValue;
    public bool IsMeetCondition;

    public S_Skill(string key, string name, string description, S_SkillConditionEnum condition, S_SkillPassiveEnum passive, bool isAccumulate)
    {
        Key = key;
        Name = name;
        Description = description;
        Condition = condition;
        Passive = passive;
        IsAccumulate = isAccumulate;
        IsMeetCondition = false;
    }
    public void SubscribeGameFlowManager() // 생성 시 반드시 클론과 사용
    {
        S_GameFlowManager.Instance.OnNewTurn += StartNewTurn;
    }
    public virtual async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        await Task.Delay(1);
    }
    public virtual void CheckMeetConditionByBasic(S_Card card = null)
    {

    }
    public virtual void CheckMeetConditionByActivatedCount(S_Card card = null)
    {

    }
    public virtual void StartNewTurn(int currentTrial)
    {

    }
    public virtual string GetDescription()
    {
        return Description;
    }
    public virtual S_Skill Clone()
    {
        return new S_Skill(Key, Name, Description, Condition, Passive, IsAccumulate);
    }
}