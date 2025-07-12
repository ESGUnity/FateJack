using UnityEngine;

public class S_Unshackle : S_CardBase
{
    public S_Unshackle()
    {
        Key = "Unshackle";
        Weight = 4;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat_Per1MindCard, S_StatEnum.Mind, 2)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
