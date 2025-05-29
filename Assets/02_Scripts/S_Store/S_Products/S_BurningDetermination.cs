using UnityEngine;

public class S_BurningDetermination : S_Product
{
    public S_BurningDetermination() : base
    (
        "P_BurningDetermination",
        "��Ÿ�� ����",
        "������ ī�� 2���� �����ϸ� �Ҹ��ŵ�ϴ�.",
        5,
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
        return new S_BurningDetermination();
    }
}