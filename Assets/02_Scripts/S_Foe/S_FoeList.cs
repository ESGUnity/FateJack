using System.Collections.Generic;

public static class S_FoeList
{
    public static List<S_FoeStruct> FOES = new()
    {
        new S_FoeStruct(0, 150, new(){ new S_Execution() }, new(){ }, 0, new(){ S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default),
        new S_FoeStruct(1, 150, new(){ new S_Execution() }, new(){ }, 0, new(){ S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default),
        new S_FoeStruct(2, 250, new(){ new S_Execution() }, new(){ }, 0, new(){ S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default),
        new S_FoeStruct(3, 800, new() { new S_Execution() }, new(){ }, 0, new() { S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default),
        new S_FoeStruct(4, 2000, new() { new S_Execution(), new S_Retribution() }, new(){ }, 0, new() { S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, S_FoeEventEnum.Engraving_1),

    };
}

public struct S_FoeStruct
{
    public int Trial;
    public int Health;
    public List<S_CardBase> EssentialFoeCards;
    public List<S_CardBase> OptionalFoeCards;
    public int OptionalCount;
    public List<S_ProductEnum> Rewards;
    public int RewardCount;
    public S_FoeEventEnum FoeEvent;

    public S_FoeStruct(int trial, int health, List<S_CardBase> essentialFoeCards, List<S_CardBase> optionalFoeCards, int optionalCount, List<S_ProductEnum> rewards, int rewardCount, S_FoeEventEnum foeEvent = default)
    {
        Trial = trial;
        Health = health;
        EssentialFoeCards = essentialFoeCards;
        OptionalFoeCards = optionalFoeCards;
        OptionalCount = optionalCount;
        Rewards = rewards;
        RewardCount = rewardCount;
        FoeEvent = foeEvent;
    }
}

public enum S_FoeEventEnum
{
    None,

    Engraving_1, Engraving_2, Engraving_3,

    Dismantle_1, Mask_1,
}
public enum S_FoeTypeEnum
{
    None,
    Clotho,
    Lachesis,
    Atropos,
    Clotho_Elite,
    Lachesis_Elite,
    Atropos_Elite,
    Clotho_Boss,
    Lachesis_Boss,
    Atropos_Boss,
}