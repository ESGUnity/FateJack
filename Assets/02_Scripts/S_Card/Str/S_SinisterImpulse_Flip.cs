using UnityEngine;

public class S_SinisterImpulse_Flip : S_CardBase
{
    public S_SinisterImpulse_Flip()
    {
        Key = "SinisterImpulse_Flip";
        Weight = 3;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Delusion, default, 3),
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_MultiDamage, default, default, 2)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix, S_EngravingEnum.QuickAction };
        Engraving = new();
    }
}
