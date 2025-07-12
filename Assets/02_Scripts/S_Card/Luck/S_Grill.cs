using UnityEngine;

public class S_Grill : S_CardBase
{
    public S_Grill()
    {
        Key = "Grill";
        Weight = 5;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbState_CantAddState, default, default, 0)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
