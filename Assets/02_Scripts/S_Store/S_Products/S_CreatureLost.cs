using UnityEngine;

public class S_CreatureLost : S_Product
{
    public S_CreatureLost() : base
    (
        "P_CreatureLost",
        "�������� ���ǹ�",
        "������ ī�� 1���� �����Ͽ� �Ҹ��ŵ�ϴ�.\n������ ī�� 1���� ���� �߰��մϴ�.",
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
        return new S_CreatureLost();
    }
}