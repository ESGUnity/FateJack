using UnityEngine;

public class S_Chance_Flip : S_CardBase
{
    public S_Chance_Flip()
    {
        Key = "Chance_Flip";
        Weight = 1;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.Luck, 3)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
