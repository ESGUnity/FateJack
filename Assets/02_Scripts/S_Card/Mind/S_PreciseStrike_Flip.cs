using UnityEngine;

public class S_PreciseStrike_Flip : S_CardBase
{
    public S_PreciseStrike_Flip()
    {
        Key = "PreciseStrike_Flip";
        Weight = 2;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Mind, 12)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload_NotPerfect };
        Engraving = new();
    }
}
