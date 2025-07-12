using UnityEngine;

public class S_Condemnation : S_CardBase
{
    public S_Condemnation()
    {
        Key = "Condemnation";
        Weight = 0;

        CardType = S_CardTypeEnum.Foe;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Damaged, S_StatEnum.None, -1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
