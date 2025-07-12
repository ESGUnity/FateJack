using System.Collections.Generic;

public class S_CardBase
{
    public string Key;
    public int Weight;

    public S_CardTypeEnum CardType;
    public List<S_UnleashStruct> Unleash { get; set; }
    public List<S_PersistStruct> Persist;
    public List<S_EngravingEnum> OriginEngraving;
    public List<S_EngravingEnum> Engraving;

    public int ExpectedValue;
    public int HitCount;
    public int ReboundCount;

    public bool IsInDeck;
    public bool IsInUsed;
    public bool IsCurrentTurn;
    public bool IsCursed;
}

public struct S_UnleashStruct
{
    public S_UnleashEnum Unleash;
    public S_StatEnum Stat;
    public int Value;

    public S_UnleashStruct(S_UnleashEnum unleash, S_StatEnum stat, int value)
    {
        Unleash = unleash;
        Stat = stat;
        Value = value;
    }
}
public struct S_PersistStruct
{
    public S_PersistEnum Persist;
    public S_PersistModifyEnum PersistModify; // Reverb 전용
    public S_StatEnum Stat;
    public int Value;

    public S_PersistStruct(S_PersistEnum persist, S_PersistModifyEnum persistModify, S_StatEnum stat, int value)
    {
        Persist = persist;
        PersistModify = persistModify;
        Stat = stat;
        Value = value;
    }
}