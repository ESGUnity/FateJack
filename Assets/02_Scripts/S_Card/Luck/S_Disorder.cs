using UnityEngine;

public class S_Disorder : S_CardBase
{
    public S_Disorder()
    {
        Key = "Disorder";
        Weight = 10;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_MultiStat, default, S_StatEnum.Luck, 3)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}