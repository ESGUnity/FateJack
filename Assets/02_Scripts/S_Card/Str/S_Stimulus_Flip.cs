using System.Collections.Generic;
using UnityEngine;

public class S_Stimulus_Flip : S_CardBase
{
    public S_Stimulus_Flip()
    {
        Key = "Stimulus_Flip";
        Weight = 1;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Delusion, default, 1),
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.Str, 4)
        };
        Persist = new();
        OriginEngraving = new();
        Engraving = new();
    }
}
