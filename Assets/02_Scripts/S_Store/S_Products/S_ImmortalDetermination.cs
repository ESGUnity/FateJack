using UnityEngine;

public class S_ImmortalDetermination : S_Product
{
    public S_ImmortalDetermination() : base
    (
        "P_ImmortalDetermination",
        "불멸의 의지",
        "덱에서 카드 1장을 선택합니다. 선택한 카드를 1장 복사하여 덱에 추가합니다.",
        9,
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
        return new S_ImmortalDetermination();
    }
}