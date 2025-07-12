using UnityEngine;

public class S_Awakening_Flip : S_CardBase
{
    public S_Awakening_Flip()
    {
        Key = "Awakening_Flip";
        Weight = 12;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per3MindCard, S_StatEnum.Mind_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
