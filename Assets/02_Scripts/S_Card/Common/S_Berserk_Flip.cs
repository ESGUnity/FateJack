using UnityEngine;

public class S_Berserk_Flip : S_CardBase
{
    public S_Berserk_Flip()
    {
        Key = "Berserk_Flip";
        Weight = 9;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbCard_Stat, S_PersistModifyEnum.Any, S_StatEnum.AllStat, -1),
            new S_PersistStruct(S_PersistEnum.Harm_MultiDamage, default, default, 2)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix, S_EngravingEnum.QuickAction };
        Engraving = new();
    }
}
