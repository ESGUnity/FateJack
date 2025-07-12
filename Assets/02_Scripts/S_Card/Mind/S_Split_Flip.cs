using UnityEngine;

public class S_Split_Flip : S_CardBase
{
    public S_Split_Flip()
    {
        Key = "Split_Flip";
        Weight = 6;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbCard_First, S_PersistModifyEnum.Mind, default, 1)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
