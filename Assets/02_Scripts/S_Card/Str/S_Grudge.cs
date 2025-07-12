using UnityEngine;

public class S_Grudge : S_CardBase
{
    public S_Grudge()
    {
        Key = "Grudge";
        Weight = 11;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per4CursedCard, S_StatEnum.Str_Mind, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
