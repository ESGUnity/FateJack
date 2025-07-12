using UnityEngine;

public class S_Inspiration : S_CardBase
{
    public S_Inspiration()
    {
        Key = "Inspiration";
        Weight = 2;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Expansion, default, 1),
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
