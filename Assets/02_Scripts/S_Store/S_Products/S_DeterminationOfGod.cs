using System.Collections.Generic;
using UnityEngine;

public class S_DeterminationOfGod : S_Product
{
    public S_DeterminationOfGod() : base
    (
        "P_DeterminationOfGod",
        "거대한 의지",
        "신화 등급 카드 3장 중 1장을 선택하여 덱에 추가합니다.",
        9,
        S_ProductTypeEnum.Card,
        S_ProductModifyEnum.Add,
        1
    ) { }

    public override void BuyProduct()
    {
        List<S_Card> cards = new();

        for (int i = 0; i < 3; i++)
        {
            cards.Add(S_CardManager.Instance.GenerateMythicCard());
        }

        S_StoreInfoSystem.Instance.StartSelectOptionCards(cards);
    }

    public override S_Product Clone()
    {
        return new S_DeterminationOfGod();
    }
}
