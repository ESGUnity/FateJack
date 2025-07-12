using UnityEngine;

public class S_SuddenStrike : S_CardBase
{
    public S_SuddenStrike()
    {
        Key = "SuddenStrike";
        Weight = 2;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Luck, 6)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
