using UnityEngine;

public class S_Trinity_Flip : S_CardBase
{
    public S_Trinity_Flip()
    {
        Key = "Trinity_Flip";
        Weight = 3;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.IsSameType_MindLuck, default, default, 0)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
