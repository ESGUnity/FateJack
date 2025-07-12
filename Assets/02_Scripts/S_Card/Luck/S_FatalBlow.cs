using UnityEngine;

public class S_FatalBlow : S_CardBase
{
    public S_FatalBlow()
    {
        Key = "FatalBlow";
        Weight = 12;

        CardType = S_CardTypeEnum.Luck;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per3LuckCard, S_StatEnum.Str_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
