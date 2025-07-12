using UnityEngine;

public class S_DeepInsight_Flip : S_CardBase
{
    public S_DeepInsight_Flip()
    {
        Key = "DeepInsight_Flip";
        Weight = 10;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_CantHarm, default, default, 0),
            new S_PersistStruct(S_PersistEnum.Stat_MultiStat, default, S_StatEnum.Mind, 4)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
