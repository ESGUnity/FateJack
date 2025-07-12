using UnityEngine;

public class S_WrathStrike_Flip : S_CardBase
{
    public S_WrathStrike_Flip()
    {
        Key = "WrathStrike_Flip";
        Weight = 2;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Str, 20)
        };
        Persist = new();
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
