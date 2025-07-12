using UnityEngine;

public class S_Plunder_Flip : S_CardBase
{
    public S_Plunder_Flip()
    {
        Key = "Plunder_Flip";
        Weight = 1;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Delusion, default, 1),
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.AllStat, 2)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
