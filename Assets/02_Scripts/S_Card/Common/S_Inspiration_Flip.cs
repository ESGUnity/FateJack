using UnityEngine;

public class S_Inspiration_Flip : S_CardBase
{
    public S_Inspiration_Flip()
    {
        Key = "Inspiration_Flip";
        Weight = 2;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Weight_1Or11, default, 0),
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
