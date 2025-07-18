using UnityEngine;

public class S_Carnage_Flip : S_CardBase
{
    public S_Carnage_Flip()
    {
        Key = "Carnage_Flip";
        Weight = 12;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_CantHarm, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Stat_MultiStat, default, S_StatEnum.AllStat, 3)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
