using UnityEngine;

public class S_UntappedPower : S_CardBase
{
    public S_UntappedPower()
    {
        Key = "UntappedPower";
        Weight = 6;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat_SubAllHighestStatAndStr3Multi, default, 0),
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
