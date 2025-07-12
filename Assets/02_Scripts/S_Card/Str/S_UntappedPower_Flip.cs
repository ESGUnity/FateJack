using UnityEngine;

public class S_UntappedPower_Flip : S_CardBase
{
    public S_UntappedPower_Flip()
    {
        Key = "UntappedPower_Flip";
        Weight = 6;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per1StrCard, S_StatEnum.Str, 10)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
