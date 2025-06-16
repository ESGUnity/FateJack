using System.Threading.Tasks;

public class S_Trinket
{
    public string Key;
    public string Name;
    public string Description;
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

    public bool IsAccumulate;
    public int CurrentAccumulateValue;
    public int TotalTrialAccumulateValue;

    public bool IsMeetCondition;

    public S_Trinket(string key, string name, string description, int intValue, float floatValue,
        S_TrinketConditionEnum condition, S_TrinketModifyEnum modify, S_TrinketPassiveEnum passive, S_TrinketEffectEnum effect, S_BattleStatEnum stat,
        bool isNeedActivatedCount, bool isAccumulate)
    {
        Key = key;
        Name = name;
        Description = description;
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

        IsAccumulate = isAccumulate;
        CurrentAccumulateValue = 0;
        TotalTrialAccumulateValue = 0;

        IsMeetCondition = false;
    }
    public virtual S_Trinket Clone()
    {
        return new S_Trinket(Key, Name, Description, IntValue, FloatValue, Condition, Modify, Passive, Effect, Stat, IsNeedActivatedCount, IsAccumulate);
    }
}