using UnityEngine;

public class S_CriticalBlow_Flip : S_CardBase
{
    public S_CriticalBlow_Flip()
    {
        Key = "CriticalBlow_Flip";
        Weight = 6;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Luck, 4)
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_AddDamagePer1Rebound, default, default, 4)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
