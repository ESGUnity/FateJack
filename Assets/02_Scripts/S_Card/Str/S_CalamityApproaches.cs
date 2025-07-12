using UnityEngine;

public class S_CalamityApproaches : S_CardBase
{
    public S_CalamityApproaches()
    {
        Key = "CalamityApproaches";
        Weight = 7;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbState_Dispel, default, default, 1)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
