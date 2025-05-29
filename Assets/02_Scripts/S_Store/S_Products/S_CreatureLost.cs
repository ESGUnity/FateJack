using UnityEngine;

public class S_CreatureLost : S_Product
{
    public S_CreatureLost() : base
    (
        "P_CreatureLost",
        "피조물의 유실물",
        "덱에서 카드 1장을 선택하여 소멸시킵니다.\n무작위 카드 1장을 덱에 추가합니다.",
        0,
        S_ProductTypeEnum.Card,
        S_ProductModifyEnum.Remove,
        1
    ) { }

    public override void BuyProduct()
    {
        S_StoreInfoSystem.Instance.StartSelectCardInDeck();
    }

    public override S_Product Clone()
    {
        return new S_CreatureLost();
    }
}