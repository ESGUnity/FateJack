using UnityEngine;

public class S_Shake_Flip : S_CardBase
{
    public S_Shake_Flip()
    {
        Key = "Shake_Flip";
        Weight = 11;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per4State, S_StatEnum.Mind_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
