using System.Collections.Generic;
using UnityEngine;

public class S_DreamingDetermination : S_Product
{
    public S_DreamingDetermination() : base
    (
        "P_DreamingDetermination",
        "�޲ٴ� ����",
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
        return new S_DreamingDetermination();
    }
}