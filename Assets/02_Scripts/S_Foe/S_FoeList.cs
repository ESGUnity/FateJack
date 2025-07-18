using System.Collections.Generic;

public static class S_FoeList
{
    public static List<S_FoeStruct> FOES = new()
    {
        new S_FoeStruct(0, 200, new(){ new S_Execution() }, new(){ }, 0, new(){ S_ProductEnum.OldMold, S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 3, default),

        new S_FoeStruct(1, 200, new(){ new S_Execution() }, new(){ }, 0, new(){ S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default),
        new S_FoeStruct(2, 350, new(){ new S_Execution() }, new(){ }, 0, new(){ S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default), // 150 증가
        new S_FoeStruct(3, 800, new() { new S_Execution() }, new(){ }, 0, new() { S_ProductEnum.OldMold, S_ProductEnum.Rebound, S_ProductEnum.OldMold }, 3, S_FoeEventEnum.RandomEngraving_1), // 450 증가
        
        new S_FoeStruct(4, 1500, new() { new S_Condemnation(), new S_Retribution() }, new(){ }, 0, new() { S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default), // 700 증가
        new S_FoeStruct(5, 3000, new() { new S_Condemnation(), new S_Retribution() }, new(){ }, 0, new() { S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default), // 1500 증가
        new S_FoeStruct(6, 5000, new() { new S_Execution(), new S_Retribution() }, new(){ }, 0, new() { S_ProductEnum.OldMold, S_ProductEnum.Dismantle, S_ProductEnum.OldMold }, 3, default), // 2000 증가

        new S_FoeStruct(7, 8000, new() { new S_Judgement(), new S_Retribution() }, new(){ }, 0, new() { S_ProductEnum.MeltedMold, S_ProductEnum.SpiritualMold }, 2, default), // 3000 증가
        new S_FoeStruct(8, 12000, new() { new S_Judgement(), new S_Retribution() }, new(){ }, 0, new() { S_ProductEnum.BrightMold, S_ProductEnum.DelicateMold }, 2, default), // 4000 증가
        new S_FoeStruct(9, 17000, new() { new S_Judgement(), new S_Execution() }, new(){ }, 0, new() { S_ProductEnum.Immunity, S_ProductEnum.QuickAction, S_ProductEnum.Rebound, S_ProductEnum.Fix, S_ProductEnum.Flexible, S_ProductEnum.Leap }, 3, default), // 5000 증가

        new S_FoeStruct(10, 32000, new() { new S_Condemnation(), new S_Judgement(), new S_Retribution() }, new(){ }, 0, new() { S_ProductEnum.OldMold, S_ProductEnum.OldMold }, 2, default), // 5000 증가
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

    RandomEngraving_1, RandomEngraving_2, RandomEngraving_3,

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