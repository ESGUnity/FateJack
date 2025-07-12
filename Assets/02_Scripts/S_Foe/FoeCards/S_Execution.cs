using UnityEngine;

public class S_Execution : S_CardBase
{
    public S_Execution()
    {
        Key = "Execution";
        Weight = 0;

        CardType = S_CardTypeEnum.Foe;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Damaged, S_StatEnum.None, -2)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
