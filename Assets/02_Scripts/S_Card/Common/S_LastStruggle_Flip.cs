using UnityEngine;

public class S_LastStruggle_Flip : S_CardBase
{
    public S_LastStruggle_Flip()
    {
        Key = "LastStruggle_Flip";
        Weight = 11;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_NoBurstPerfect, default, default, 0)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix, S_EngravingEnum.QuickAction };
        Engraving = new();
    }
}
