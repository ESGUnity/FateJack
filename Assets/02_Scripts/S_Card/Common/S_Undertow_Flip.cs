using UnityEngine;

public class S_Undertow_Flip : S_CardBase
{
    public S_Undertow_Flip()
    {
        Key = "Undertow_Flip";
        Weight = 6;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_CantHarm, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Stand_AllCardsRebound1, default, default, 1)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
