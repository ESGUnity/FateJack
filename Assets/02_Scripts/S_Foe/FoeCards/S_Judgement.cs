using UnityEngine;

public class S_Judgement : S_CardBase
{
    public S_Judgement()
    {
        Key = "Judgement";
        Weight = 0;

        CardType = S_CardTypeEnum.Foe;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Damaged_DiffLimitWeight, S_StatEnum.None, -1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
