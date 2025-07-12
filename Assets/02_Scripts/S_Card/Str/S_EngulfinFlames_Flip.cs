using UnityEngine;

public class S_EngulfinFlames_Flip : S_CardBase
{
    public S_EngulfinFlames_Flip()
    {
        Key = "EngulfinFlames_Flip";
        Weight = 5;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Str, 35),
            new S_UnleashStruct(S_UnleashEnum.Curse_AllCardsIfBurst, default, 0)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
