using UnityEngine;

public class S_Unshackle_Flip : S_CardBase
{
    public S_Unshackle_Flip()
    {
        Key = "Unshackle_Flip";
        Weight = 4;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Cursed_Weight, default, default, -3)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
