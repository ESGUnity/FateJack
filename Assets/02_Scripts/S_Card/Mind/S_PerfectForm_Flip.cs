using UnityEngine;

public class S_PerfectForm_Flip : S_CardBase
{
    public S_PerfectForm_Flip()
    {
        Key = "PerfectForm_Flip";
        Weight = 9;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Weight_PerfectAddColdBlood, default, default, 1)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
