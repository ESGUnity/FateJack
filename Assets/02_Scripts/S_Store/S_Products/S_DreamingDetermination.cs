using System.Collections.Generic;
using UnityEngine;

public class S_DreamingDetermination : S_Product
{
    public S_DreamingDetermination() : base
    (
        "P_DreamingDetermination",
        "꿈꾸는 의지",
        "덱에서 카드 1장을 선택하여 소멸시킵니다. 선택한 카드와 같은 문양의 새로운 카드 1장을 덱에 추가합니다.",
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
        return new S_DreamingDetermination();
    }
}