using UnityEngine;

public class S_Artifice_Flip : S_CardBase
{
    public S_Artifice_Flip()
    {
        Key = "Artifice_Flip";
        Weight = 7;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Unleash_StatPer1Rebound, default, S_StatEnum.Luck, 2)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
