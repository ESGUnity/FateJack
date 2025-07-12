using System.Collections.Generic;
using UnityEngine;

public class S_WrathStrike : S_CardBase
{
    public S_WrathStrike()
    {
        Key = "WrathStrike";
        Weight = 2;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Str, 6)
        };
        Persist = new();
        OriginEngraving = new();
        Engraving = new();
    }
}
