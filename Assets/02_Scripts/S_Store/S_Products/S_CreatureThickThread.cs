using UnityEngine;

public class S_CreatureThickThread : S_Product
{
    public S_CreatureThickThread() : base
    (
        "P_CreatureThickThread",
        "�������� ���ǹ�",
        "������ ī�� 1���� �����Ͽ� �Ҹ��ŵ�ϴ�.\n������ ī�庸�� ���� ȿ�� ����� ���ο� ī�� 1���� ���� �߰��մϴ�.\n������ ī�尡 ��ȭ ����� ��� ��ȭ ��� ī�带 �߰��մϴ�.",
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