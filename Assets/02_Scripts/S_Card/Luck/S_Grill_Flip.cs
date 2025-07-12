using UnityEngine;

public class S_Grill_Flip : S_CardBase
{
    public S_Grill_Flip()
    {
        Key = "Grill_Flip";
        Weight = 5;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stand_AllRightCardsRebound2, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Stand_Overload_AllRightCards, default, default, 0),
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
