using System.Collections.Generic;
using UnityEngine;

public class S_GreatDetermination : S_Product
{
    public S_GreatDetermination() : base
    (
        "P_GreatDetermination",
        "거대한 의지",
        "무작위 카드 6장 중 2장을 덱에 추가합니다.",
        7,
        S_ProductTypeEnum.Card,
        S_ProductModifyEnum.Add,
        2
    ) { }

    public override void BuyProduct()
    {
        List<S_Card> cards = new();

        for (int i = 0; i < 6; i++)
        {
            cards.Add(S_CardManager.Instance.GenerateRandomCard());
        }

        S_StoreInfoSystem.Instance.StartSelectOptionCards(cards);
    }

    public override S_Product Clone()
    {
        return new S_GreatDetermination();
    }
}
