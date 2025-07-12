using UnityEngine;

public class S_Corrupt_Flip : S_CardBase
{
    public S_Corrupt_Flip()
    {
        Key = "Corrupt_Flip";
        Weight = 3;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Cursed_LeftCardsImmunity, default, default, 0)
        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
