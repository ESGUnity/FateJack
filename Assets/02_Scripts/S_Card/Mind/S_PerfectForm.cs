using UnityEngine;

public class S_PerfectForm : S_CardBase
{
    public S_PerfectForm()
    {
        Key = "PerfectForm";
        Weight = 9;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Weight_AddIfWeightLowerLimit, default, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Fix, S_EngravingEnum.Rebound };
        Engraving = new();
    }
}
