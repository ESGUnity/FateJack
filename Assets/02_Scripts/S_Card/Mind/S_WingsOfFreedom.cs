using UnityEngine;

public class S_WingsOfFreedom : S_CardBase
{
    public S_WingsOfFreedom()
    {
        Key = "WingsOfFreedom";
        Weight = 7;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Weight_MakePerfectAndAddMind, default, 0)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { S_EngravingEnum.Overload };
        Engraving = new();
    }
}
