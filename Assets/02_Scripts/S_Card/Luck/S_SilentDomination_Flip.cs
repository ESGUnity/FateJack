using UnityEngine;

public class S_SilentDomination_Flip : S_CardBase
{
    public S_SilentDomination_Flip()
    {
        Key = "SilentDomination_Flip";
        Weight = 4;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat_Per1LuckCard, S_StatEnum.AllStat, 2)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
