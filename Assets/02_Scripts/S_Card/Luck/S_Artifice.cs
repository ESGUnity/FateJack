using UnityEngine;

public class S_Artifice : S_CardBase
{
    public S_Artifice()
    {
        Key = "Artifice";
        Weight = 7;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stand_LeftLuckCardsRebound1, default, default, 0)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
