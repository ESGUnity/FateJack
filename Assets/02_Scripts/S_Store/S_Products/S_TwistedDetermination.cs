using UnityEngine;

public class S_TwistedDetermination : S_Product
{
    public S_TwistedDetermination() : base
    (
        "P_TwistedDetermination",
        "비틀린 의지",
        "덱에서 카드 2장을 선택하며 소멸시킵니다. 먼저 선택한 카드의 숫자와 나중에 선택한 카드의 문양으로 새로운 카드 1장을 덱에 추가합니다.",
        9,
        S_ProductTypeEnum.Card,
        S_ProductModifyEnum.Remove,
        2
    ) { }

    public override void BuyProduct()
    {
        S_StoreInfoSystem.Instance.StartSelectCardInDeck();
    }

    public override S_Product Clone()
    {
        return new S_TwistedDetermination();
    }
}