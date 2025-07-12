using UnityEngine;

public class S_Realization_Flip : S_CardBase
{
    public S_Realization_Flip()
    {
        Key = "Realization_Flip";
        Weight = 8;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.AllStat, 4)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
