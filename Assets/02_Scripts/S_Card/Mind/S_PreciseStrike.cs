using UnityEngine;

public class S_PreciseStrike : S_CardBase
{
    public S_PreciseStrike()
    {
        Key = "PreciseStrike";
        Weight = 2;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Mind, 6)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
