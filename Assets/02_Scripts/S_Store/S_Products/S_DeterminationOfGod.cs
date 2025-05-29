using System.Collections.Generic;
using UnityEngine;

public class S_DeterminationOfGod : S_Product
{
    public S_DeterminationOfGod() : base
    (
        "P_DeterminationOfGod",
        "�Ŵ��� ����",
        "��ȭ ��� ī�� 3�� �� 1���� �����Ͽ� ���� �߰��մϴ�.",
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
