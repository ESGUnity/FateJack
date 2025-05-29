using System.Collections.Generic;
using UnityEngine;

public class S_FaintDetermination : S_Product
{
    public S_FaintDetermination() : base
    (
        "P_FaintDetermination",
        "희미한 의지",
        "무작위 카드 3장 중 1장을 덱에 추가합니다.",
        3,
        S_ProductTypeEnum.Card,
        S_ProductModifyEnum.Add,
        1
    ) { }

    public override void BuyProduct()
    {
        List<S_Card> cards = new();

        for (int i = 0; i < 3; i++)
        {
            cards.Add(S_CardManager.Instance.GenerateRandomCard());
        }

        S_StoreInfoSystem.Instance.StartSelectOptionCards(cards);
    }

    public override S_Product Clone()
    {
        return new S_FaintDetermination();
    }
}
