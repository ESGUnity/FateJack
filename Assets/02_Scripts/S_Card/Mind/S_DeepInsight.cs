using UnityEngine;

public class S_DeepInsight : S_CardBase
{
    public S_DeepInsight()
    {
        Key = "DeepInsight";
        Weight = 10;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Stat_MultiStat, default, S_StatEnum.Mind, 3)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
