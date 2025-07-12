using UnityEngine;

public class S_EngulfinFlames : S_CardBase
{
    public S_EngulfinFlames()
    {
        Key = "EngulfinFlames";
        Weight = 5;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Dispel_Per10Str, default, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
