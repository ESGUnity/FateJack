using UnityEngine;

public class S_BindingForce : S_CardBase
{
    public S_BindingForce()
    {
        Key = "BindingForce";
        Weight = 12;

        CardType = S_CardTypeEnum.Str;
        Unleash = new()
        {
            new S_UnleashStruct(S_UnleashEnum.Harm_Per3StrCard, S_StatEnum.Str_Mind, 1)
        };
        Persist = new()
        {

        };
        OriginEngraving = new() { };
        Engraving = new();
    }
}
