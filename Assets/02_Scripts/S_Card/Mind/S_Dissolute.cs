using UnityEngine;

public class S_Dissolute : S_CardBase
{
    public S_Dissolute()
    {
        Key = "Dissolute";
        Weight = 11;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Str_Mind, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload_NotPerfect };
        Engraving = new();
    }
}
