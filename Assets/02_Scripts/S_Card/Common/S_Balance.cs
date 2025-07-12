using UnityEngine;

public class S_Balance : S_CardBase
{
    public S_Balance()
    {
        Key = "Balance";
        Weight = 10;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.DispelAndCurse_AllCard, default, 0)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
