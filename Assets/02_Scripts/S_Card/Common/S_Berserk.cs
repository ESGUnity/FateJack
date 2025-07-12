using UnityEngine;

public class S_Berserk : S_CardBase
{
    public S_Berserk()
    {
        Key = "Berserk";
        Weight = 9;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_CantStat, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Harm_MultiDamage, default, default, 2)
        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
