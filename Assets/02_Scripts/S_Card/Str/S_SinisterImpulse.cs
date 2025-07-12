using UnityEngine;

public class S_SinisterImpulse : S_CardBase
{
    public S_SinisterImpulse()
    {
        Key = "SinisterImpulse";
        Weight = 3;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Health, S_StatEnum.None, -1),
            new S_UnleashStruct(S_UnleashEnum.Stat, S_StatEnum.Str, 8)
        };
        Persist = new();
        OriginEngraving = new() { };
        Engraving = new();
    }
}
