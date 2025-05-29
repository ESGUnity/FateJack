using UnityEngine;

public class S_RebelliousDetermination : S_Product
{
    public S_RebelliousDetermination() : base
    (
        "P_RebelliousDetermination",
        "반발하는 의지",
        "덱에서 카드 1장을 선택하여 소멸시킵니다. 선택한 카드와 같은 숫자의 새로운 카드 1장을 덱에 추가합니다.",
        5,
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
        return new S_RebelliousDetermination();
    }
}