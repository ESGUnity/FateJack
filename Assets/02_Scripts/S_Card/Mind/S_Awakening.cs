using UnityEngine;

public class S_Awakening : S_CardBase
{
    public S_Awakening()
    {
        Key = "Awakening";
        Weight = 12;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per3MindCard, S_StatEnum.Str_Mind, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
