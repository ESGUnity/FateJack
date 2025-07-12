using UnityEngine;

public class S_Focus_Flip : S_CardBase
{
    public S_Focus_Flip()
    {
        Key = "Focus_Flip";
        Weight = 1;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbCard_Weight, S_PersistModifyEnum.Mind, default, -1)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
