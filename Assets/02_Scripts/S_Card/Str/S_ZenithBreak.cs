using UnityEngine;

public class S_ZenithBreak : S_CardBase
{
    public S_ZenithBreak()
    {
        Key = "ZenithBreak";
        Weight = 10;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_MultiStat, default, S_StatEnum.Str, 3)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
