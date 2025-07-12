using UnityEngine;

public class S_Split : S_CardBase
{
    public S_Split()
    {
        Key = "Split";
        Weight = 6;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per1MindCard, S_StatEnum.Mind, 10)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
