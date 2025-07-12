using UnityEngine;

public class S_Blasphemy_Flip : S_CardBase
{
    public S_Blasphemy_Flip()
    {
        Key = "Blasphemy_Flip";
        Weight = 7;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stand_FieldCardLowerThan3Rebound2, default, default, 0)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
