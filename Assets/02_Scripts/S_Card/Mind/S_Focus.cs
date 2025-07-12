using UnityEngine;

public class S_Focus : S_CardBase
{
    public S_Focus()
    {
        Key = "Focus";
        Weight = 1;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.Mind, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
