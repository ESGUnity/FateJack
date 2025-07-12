using UnityEngine;

public class S_SharpCut : S_CardBase
{
    public S_SharpCut()
    {
        Key = "SharpCut";
        Weight = 5;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {

        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Fix_FixCursedCard, default, default, 0)
        };
        OriginEngraving = new() { S_EngravingEnum.Fix };
        Engraving = new();
    }
}
