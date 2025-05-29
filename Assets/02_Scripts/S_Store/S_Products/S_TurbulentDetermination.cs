using System.Collections.Generic;
using UnityEngine;

public class S_TurbulentDetermination : S_Product
{
    public S_TurbulentDetermination() : base
    (
        "P_TurbulentDetermination",
        "�䵿ġ�� ����",
        "������ ī�� 6�� �� 1���� ���� �߰��մϴ�.",
        5,
        S_ProductTypeEnum.Card,
        S_ProductModifyEnum.Add,
        1
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
        return new S_TurbulentDetermination();
    }
}
