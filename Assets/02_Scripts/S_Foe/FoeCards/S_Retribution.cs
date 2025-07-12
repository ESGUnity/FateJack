using UnityEngine;

public class S_Retribution : S_CardBase
{
    public S_Retribution()
    {
        Key = "Retribution";
        Weight = 0;

        CardType = S_CardTypeEnum.Foe;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Damaged_CriticalBurst, S_StatEnum.None, -1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
