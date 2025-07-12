using UnityEngine;

public class S_Adventure : S_CardBase
{
    public S_Adventure()
    {
        Key = "Adventure";
        Weight = 4;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.First, default, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
