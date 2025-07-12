using UnityEngine;

public class S_SilentDomination : S_CardBase
{
    public S_SilentDomination()
    {
        Key = "SilentDomination";
        Weight = 4;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stand_RightCardReboundPer10Luck, default, default, 1)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
