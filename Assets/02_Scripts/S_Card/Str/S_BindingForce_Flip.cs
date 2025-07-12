using UnityEngine;

public class S_BindingForce_Flip : S_CardBase
{
    public S_BindingForce_Flip()
    {
        Key = "BindingForce_Flip";
        Weight = 12;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per3StrCard, S_StatEnum.Str_Luck, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
