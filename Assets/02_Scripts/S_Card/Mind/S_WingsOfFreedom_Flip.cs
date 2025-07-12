using UnityEngine;

public class S_WingsOfFreedom_Flip : S_CardBase
{
    public S_WingsOfFreedom_Flip()
    {
        Key = "WingsOfFreedom_Flip";
        Weight = 7;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.CheckBurstAndPerfect_CanPerfectDiff1, default, default, 0)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
