using UnityEngine;

public class S_Resistance : S_CardBase
{
    public S_Resistance()
    {
        Key = "Resistance";
        Weight = 4;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Limit, default, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
