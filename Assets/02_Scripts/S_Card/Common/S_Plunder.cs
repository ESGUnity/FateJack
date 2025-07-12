using UnityEngine;

public class S_Plunder : S_CardBase
{
    public S_Plunder()
    {
        Key = "Plunder";
        Weight = 1;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.AllStat, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
