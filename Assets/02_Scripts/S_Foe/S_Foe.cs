using System.Threading.Tasks;

public class S_Foe
{
    public string Key;
    public string Name;
    public string Description;
    public S_FoeTypeEnum FoeType; // 적만 존재
    public int IntValue;
    public float FloatValue;

    public S_TrinketConditionEnum Condition;
    public S_TrinketModifyEnum Modify;
    public S_TrinketPassiveEnum Passive;
    public S_TrinketEffectEnum Effect;
    public S_BattleStatEnum Stat;
    public bool IsNeedActivatedCount;
    public int ActivatedCount;
    public int ExpectedValue;

    public int CurrentAccumulateValue;
    public int TotalTrialAccumulateValue;

    public bool IsMeetCondition;

    public S_Foe(string key, string name, string description, S_FoeTypeEnum foeType, int intValue, float floatValue,
        S_TrinketConditionEnum condition, S_TrinketModifyEnum modify, S_TrinketPassiveEnum passive, S_TrinketEffectEnum effect, S_BattleStatEnum stat,
        bool isNeedActivatedCount)
    {
        Key = key;
        Name = name;
        Description = description;
        FoeType = foeType;
        IntValue = intValue;
        FloatValue = floatValue;

        Condition = condition;
        Modify = modify;
        Passive = passive;
        Effect = effect;
        Stat = stat;
        IsNeedActivatedCount = isNeedActivatedCount;
        ActivatedCount = 0;
        ExpectedValue = 0;

        CurrentAccumulateValue = 0;
        TotalTrialAccumulateValue = 0;

        IsMeetCondition = false;
    }
    public virtual S_Foe Clone()
    {
        return new S_Foe(Key, Name, Description, FoeType, IntValue, FloatValue, Condition, Modify, Passive, Effect, Stat, IsNeedActivatedCount);
    }
}