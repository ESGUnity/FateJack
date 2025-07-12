using UnityEngine;

public class S_Drain_Flip : S_CardBase
{
    public S_Drain_Flip()
    {
        Key = "Drain_Flip";
        Weight = 3;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat_Mind10SubAndStrLuck10Add, default, 10)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
