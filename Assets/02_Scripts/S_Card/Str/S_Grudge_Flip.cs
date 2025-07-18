using UnityEngine;

public class S_Grudge_Flip : S_CardBase
{
    public S_Grudge_Flip()
    {
        Key = "Grudge_Flip";
        Weight = 11;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per3CursedCard, S_StatEnum.Str_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
