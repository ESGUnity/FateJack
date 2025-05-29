using UnityEngine;

public class S_BurningDetermination : S_Product
{
    public S_BurningDetermination() : base
    (
        "P_BurningDetermination",
        "불타는 의지",
        "덱에서 카드 2장을 선택하며 소멸시킵니다.",
        5,
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
        return new S_BurningDetermination();
    }
}