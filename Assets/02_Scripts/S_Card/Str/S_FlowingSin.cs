using UnityEngine;

public class S_FlowingSin : S_CardBase
{
    public S_FlowingSin()
    {
        Key = "FlowingSin";
        Weight = 4;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per1CursedCard, S_StatEnum.Str, 5),
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
