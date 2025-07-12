using UnityEngine;

public class S_FinishingStrike_Flip : S_CardBase
{
    public S_FinishingStrike_Flip()
    {
        Key = "FinishingStrike_Flip";
        Weight = 9;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per1State, S_StatEnum.Str, 10),
            new S_UnleashStruct(S_UnleashEnum.State_SubAllStates, default, 0)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Immunity };
        Engraving = new();
    }
}
