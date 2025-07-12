using UnityEngine;

public class S_Drain : S_CardBase
{
    public S_Drain()
    {
        Key = "Drain";
        Weight = 3;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat_StrLuck5SubAndMind2Multi, default, 5)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
