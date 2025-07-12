using UnityEngine;

public class S_Undertow : S_CardBase
{
    public S_Undertow()
    {
        Key = "Undertow";
        Weight = 6;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_CantStat, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Stand_AllCardsRebound1, default, default, 1)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
