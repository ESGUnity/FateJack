using System.Collections.Generic;
using UnityEngine;

public class S_Stimulus : S_CardBase
{
    public S_Stimulus()
    {
        Key = "Stimulus";
        Weight = 1;

        CardType = S_CardTypeEnum.Str;
        Unleash = new() 
        { 
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.Str, 1) 
        };
        Persist = new() { };
        OriginEngraving = new() { };
        Engraving = new() { };
    }
}
