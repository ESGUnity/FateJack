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
    public int AccumulateValue;
    public bool CanActivateEffect;

    public S_Skill(string key, string name, string description, S_SkillConditionEnum condition, S_SkillPassiveEnum passive, bool isAccumulate)
    {
        Key = key;
        Name = name;
        Description = description;
        Condition = condition;
        Passive = passive;
        IsAccumulate = isAccumulate;
        CanActivateEffect = false;

        S_GameFlowManager.Instance.OnNewTurn += StartNewTurn;
    }

    public virtual bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = false;
        return CanActivateEffect;
    }
    public virtual async Task ActiveSkill(S_EffectActivator eA, S_Card hitCard)
    {
        await Task.Delay(1);
    }
    public virtual void StartNewTurn(int currentTrial)
    {
        
    }
    public virtual void ActivateCount(S_Card card, bool isTwist = false)
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