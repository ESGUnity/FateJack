using UnityEngine;

public class S_AllForOne_Flip : S_CardBase
{
    public S_AllForOne_Flip()
    {
        Key = "AllForOne_Flip";
        Weight = 9;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbState_MultiState, default, default, 2)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}