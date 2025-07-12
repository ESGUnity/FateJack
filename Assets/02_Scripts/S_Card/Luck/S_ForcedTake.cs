using UnityEngine;

public class S_ForcedTake : S_CardBase
{
    public S_ForcedTake()
    {
        Key = "ForcedTake";
        Weight = 8;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Unleash_HarmPer1Rebound, default, S_StatEnum.Luck, 5)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
