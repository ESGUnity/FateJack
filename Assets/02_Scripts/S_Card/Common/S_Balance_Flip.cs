using UnityEngine;

public class S_Balance_Flip : S_CardBase
{
    public S_Balance_Flip()
    {
        Key = "Balance_Flip";
        Weight = 10;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbCard_Only2WeightWhenHitCurseCard, default, default, 0)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
