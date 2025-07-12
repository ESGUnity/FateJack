using UnityEngine;

public class S_Chance : S_CardBase
{
    public S_Chance()
    {
        Key = "Chance";
        Weight = 1;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
