using UnityEngine;

public class S_Corrupt : S_CardBase
{
    public S_Corrupt()
    {
        Key = "Corrupt";
        Weight = 3;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Health, default, -2)
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_MultiDamage, default, default, 2)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix, S_EngravingEnum.QuickAction };
        Engraving = new();
    }
}
