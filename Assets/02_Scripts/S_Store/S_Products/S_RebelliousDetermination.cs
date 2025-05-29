using UnityEngine;

public class S_RebelliousDetermination : S_Product
{
    public S_RebelliousDetermination() : base
    (
        "P_RebelliousDetermination",
        "�ݹ��ϴ� ����",
        "������ ī�� 1���� �����Ͽ� �Ҹ��ŵ�ϴ�. ������ ī��� ���� ������ ���ο� ī�� 1���� ���� �߰��մϴ�.",
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
        return new S_RebelliousDetermination();
    }
}