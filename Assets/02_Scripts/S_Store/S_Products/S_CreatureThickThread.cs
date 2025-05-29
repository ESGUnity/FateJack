using UnityEngine;

public class S_CreatureThickThread : S_Product
{
    public S_CreatureThickThread() : base
    (
        "P_CreatureThickThread",
        "피조물의 유실물",
        "덱에서 카드 1장을 선택하여 소멸시킵니다.\n선택한 카드보다 높은 효과 등급의 새로운 카드 1장을 덱에 추가합니다.\n선택한 카드가 신화 등급의 경우 신화 등급 카드를 추가합니다.",
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
        return new S_CreatureThickThread();
    }
}