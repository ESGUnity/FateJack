using System.Threading.Tasks;

public class S_Foe
{
    public string Key;
    public string Name;
    public string AbilityDescription;
    public S_FoeTypeEnum FoeType;

    public S_FoeAbilityConditionEnum Condition;
    public S_FoePassiveEnum Passive;
    public int ActivatedCount;
    public bool CanActivateEffect;

    public S_Foe(string key, string name, string abilityDescription, S_FoeTypeEnum foeType, S_FoeAbilityConditionEnum condition, S_FoePassiveEnum passive)
    {
        Key = key;
        Name = name;
        AbilityDescription = abilityDescription;
        FoeType = foeType;
        Condition = condition;
        Passive = passive;
        CanActivateEffect = false;

        S_GameFlowManager.Instance.OnNewTurn += StartNewTurn;
    }
    public virtual bool IsMeetCondition(S_Card card = null)
    {
        CanActivateEffect = false;
        return CanActivateEffect;
    }
    public virtual async Task ActiveFoeAbility(S_EffectActivator eA, S_Card hitCard)
    {
        await Task.Delay(1);
    }
    public virtual void ActivateCount(S_Card card, bool isTwist = false)
    {

    }
    public virtual void StartNewTurn(int currentTrial)
    {

    }
    public virtual string GetDescription()
    {
        return AbilityDescription;
    }
    public virtual S_Foe Clone()
    {
        return new S_Foe(Key, Name, AbilityDescription, FoeType, Condition, Passive);
    }
}