using UnityEngine;

public class S_Composure_Flip : S_CardBase
{
    public S_Composure_Flip()
    {
        Key = "Composure_Flip";
        Weight = 3;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Stat_Per1State, S_StatEnum.Luck, 3)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
