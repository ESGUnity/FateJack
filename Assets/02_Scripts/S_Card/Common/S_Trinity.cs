using UnityEngine;

public class S_Trinity : S_CardBase
{
    public S_Trinity()
    {
        Key = "Trinity";
        Weight = 3;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.IsSameType_StrCommon, default, default, 0)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
