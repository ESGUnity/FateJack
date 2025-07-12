using UnityEngine;

public class S_AllForOne : S_CardBase
{
    public S_AllForOne()
    {
        Key = "AllForOne";
        Weight = 9;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.AllStat, 1)
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_AddStatPer1Rebound, default, default, 1)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
