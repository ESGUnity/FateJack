using UnityEngine;

public class S_FatalBlow_Flip : S_CardBase
{
    public S_FatalBlow_Flip()
    {
        Key = "FatalBlow_Flip";
        Weight = 12;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per3LuckCard, S_StatEnum.Mind_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
