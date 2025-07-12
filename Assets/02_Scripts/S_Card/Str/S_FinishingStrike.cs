using UnityEngine;

public class S_FinishingStrike : S_CardBase
{
    public S_FinishingStrike()
    {
        Key = "FinishingStrike";
        Weight = 9;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Curse_AllRightCards, default, 0),
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Str, 50)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
