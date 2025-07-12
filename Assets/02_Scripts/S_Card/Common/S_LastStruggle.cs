using UnityEngine;

public class S_LastStruggle : S_CardBase
{
    public S_LastStruggle()
    {
        Key = "LastStruggle";
        Weight = 11;

        CardType = S_CardTypeEnum.Common;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Expansion, default, 2),
            new S_UnleashStruct(S_UnleashEnum.First, default, 2),
            new S_UnleashStruct(S_UnleashEnum.ColdBlood, default, 2)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
