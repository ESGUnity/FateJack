using UnityEngine;

public class S_SharpCut_Flip : S_CardBase
{
    public S_SharpCut_Flip()
    {
        Key = "SharpCut_Flip";
        Weight = 5;

        CardType = S_CardTypeEnum.Mind;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm, S_StatEnum.Mind, 4)
        };
        Persist = new()
        {
            new S_PersistStruct(S_PersistEnum.Harm_MultiDamagePer1HitCount, default, default, 2)
        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
