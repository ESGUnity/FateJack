using UnityEngine;

public class S_ForcedTake_Flip : S_CardBase
{
    public S_ForcedTake_Flip()
    {
        Key = "ForcedTake_Flip";
        Weight = 8;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.State_AddRandomStatesPer10Luck, default, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
