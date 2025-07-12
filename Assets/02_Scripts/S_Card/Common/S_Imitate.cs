using UnityEngine;

public class S_Imitate : S_CardBase
{
    public S_Imitate()
    {
        Key = "Imitate";
        Weight = 5;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stand_FirstCardRebound2, default, default, 2)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
