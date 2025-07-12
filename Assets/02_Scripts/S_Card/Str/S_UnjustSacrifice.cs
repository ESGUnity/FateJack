using UnityEngine;

public class S_UnjustSacrifice : S_CardBase
{
    public S_UnjustSacrifice()
    {
        Key = "UnjustSacrifice";
        Weight = 8;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Cursed_AddRandomState, default, default, 1)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
