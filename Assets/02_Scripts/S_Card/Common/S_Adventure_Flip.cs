using UnityEngine;

public class S_Adventure_Flip : S_CardBase
{
    public S_Adventure_Flip()
    {
        Key = "Adventure_Flip";
        Weight = 4;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.AllStat, 2)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
