using UnityEngine;

public class S_CriticalBlow : S_CardBase
{
    public S_CriticalBlow()
    {
        Key = "CriticalBlow";
        Weight = 6;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per1LuckCard, S_StatEnum.Luck, 10)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
