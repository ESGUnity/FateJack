using UnityEngine;

public class S_Dissolute_Flip : S_CardBase
{
    public S_Dissolute_Flip()
    {
        Key = "Dissolute_Flip";
        Weight = 11;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Mind_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload_NotPerfect };
        Engraving = new();
    }
}
