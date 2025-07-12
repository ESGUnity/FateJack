using UnityEngine;

public class S_Carnage : S_CardBase
{
    public S_Carnage()
    {
        Key = "Carnage";
        Weight = 12;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_CantStat, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Harm_MultiDamage, default, default, 3)
        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
