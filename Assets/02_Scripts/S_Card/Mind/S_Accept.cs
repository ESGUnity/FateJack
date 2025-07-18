using UnityEngine;

public class S_Accept : S_CardBase
{
    public S_Accept()
    {
        Key = "Accept";
        Weight = 8;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Limit_Per10Mind, default, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload_NotPerfect };
        Engraving = new();
    }
}
