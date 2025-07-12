using UnityEngine;

public class S_Disorder_Flip : S_CardBase
{
    public S_Disorder_Flip()
    {
        Key = "Disorder_Flip";
        Weight = 10;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_CantHarm, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Stat_MultiStat, default, S_StatEnum.Luck, 4)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
