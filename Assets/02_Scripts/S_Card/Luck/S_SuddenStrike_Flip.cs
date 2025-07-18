using UnityEngine;

public class S_SuddenStrike_Flip : S_CardBase
{
    public S_SuddenStrike_Flip()
    {
        Key = "SuddenStrike_Flip";
        Weight = 2;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per1State, S_StatEnum.Luck, 4)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
