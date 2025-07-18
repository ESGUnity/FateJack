using UnityEngine;

public class S_ZenithBreak_Flip : S_CardBase
{
    public S_ZenithBreak_Flip()
    {
        Key = "ZenithBreak_Flip";
        Weight = 10;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Delusion, default, 2)
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_MultiStat, default, S_StatEnum.Str, 4)
        };
        OriginEngraving = new() { };
        Engraving = new() { };
    }
}
