using UnityEngine;

public class S_FlowingSin_Flip : S_CardBase
{
    public S_FlowingSin_Flip()
    {
        Key = "FlowingSin_Flip";
        Weight = 4;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Cursed_Stat, default, S_StatEnum.Str, 2)
        };
        OriginEngraving = new() { S_EngravingEnum.Immunity };
        Engraving = new();
    }
}
