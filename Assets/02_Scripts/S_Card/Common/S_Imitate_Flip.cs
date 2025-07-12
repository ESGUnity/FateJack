using UnityEngine;

public class S_Imitate_Flip : S_CardBase
{
    public S_Imitate_Flip()
    {
        Key = "Imitate_Flip";
        Weight = 5;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat_Per1CommonCard, S_StatEnum.AllStat, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
