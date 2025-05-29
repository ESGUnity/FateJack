using UnityEngine;

public class S_TwistedDetermination : S_Product
{
    public S_TwistedDetermination() : base
    (
        "P_TwistedDetermination",
        "��Ʋ�� ����",
        "������ ī�� 2���� �����ϸ� �Ҹ��ŵ�ϴ�. ���� ������ ī���� ���ڿ� ���߿� ������ ī���� �������� ���ο� ī�� 1���� ���� �߰��մϴ�.",
        9,
        S_ProductTypeEnum.Card,
        S_ProductModifyEnum.Remove,
        2
    ) { }

    public override void BuyProduct()
    {
        S_StoreInfoSystem.Instance.StartSelectCardInDeck();
    }

    public override S_Product Clone()
    {
        return new S_TwistedDetermination();
    }
}