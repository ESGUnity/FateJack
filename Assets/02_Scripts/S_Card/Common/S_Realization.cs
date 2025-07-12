using UnityEngine;

public class S_Realization : S_CardBase
{
    public S_Realization()
    {
        Key = "Realization";
        Weight = 8;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Limit1_Weight2, default, 0)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
