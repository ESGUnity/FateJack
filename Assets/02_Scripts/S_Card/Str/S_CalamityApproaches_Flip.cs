using UnityEngine;

public class S_CalamityApproaches_Flip : S_CardBase
{
    public S_CalamityApproaches_Flip()
    {
        Key = "CalamityApproaches_Flip";
        Weight = 7;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Delusion, default, 2),
            new S_UnleashStruct(S_UnleashEnum.Dispel_AllCard, default, 0)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
