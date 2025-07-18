using UnityEngine;

public class S_Resistance_Flip : S_CardBase
{
    public S_Resistance_Flip()
    {
        Key = "Resistance_Flip";
        Weight = 4;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Limit, default, -2)
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_MultiDamage, default, default, 2)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix, S_EngravingEnum.QuickAction };
        Engraving = new();
    }
}
