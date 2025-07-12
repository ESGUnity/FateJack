using UnityEngine;

public class S_Blasphemy : S_CardBase
{
    public S_Blasphemy()
    {
        Key = "Blasphemy";
        Weight = 7;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.ColdBlood, default, 0)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
