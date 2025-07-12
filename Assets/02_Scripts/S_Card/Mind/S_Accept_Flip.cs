using UnityEngine;

public class S_Accept_Flip : S_CardBase
{
    public S_Accept_Flip()
    {
        Key = "Accept_Flip";
        Weight = 8;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Weight_PerfectAddStat, default, S_StatEnum.AllStat, 8)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
