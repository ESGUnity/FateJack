using UnityEngine;

public class S_Composure : S_CardBase
{
    public S_Composure()
    {
        Key = "Composure";
        Weight = 3;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.ReverbCard_Expansion, S_PersistModifyEnum.Luck, default, 1)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
