using UnityEngine;

public class S_ImmortalDetermination : S_Product
{
    public S_ImmortalDetermination() : base
    (
        "P_ImmortalDetermination",
        "�Ҹ��� ����",
        "������ ī�� 1���� �����մϴ�. ������ ī�带 1�� �����Ͽ� ���� �߰��մϴ�.",
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