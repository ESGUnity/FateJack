using UnityEngine;

public class S_UnjustSacrifice_Flip : S_CardBase
{
    public S_UnjustSacrifice_Flip()
    {
        Key = "UnjustSacrifice_Flip";
        Weight = 8;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbCard_CurseAndAddStat, S_PersistModifyEnum.Str, S_StatEnum.AllStat, 5)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
