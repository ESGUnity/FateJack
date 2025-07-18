using UnityEngine;

public class S_Shake : S_CardBase
{
    public S_Shake()
    {
        Key = "Shake";
        Weight = 11;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per2State, S_StatEnum.Str_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
