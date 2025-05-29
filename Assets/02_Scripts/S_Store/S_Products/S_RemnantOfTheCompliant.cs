using System.Collections.Generic;
using UnityEngine;

public class S_RemnantOfTheCompliant : S_Product
{
    public S_RemnantOfTheCompliant() : base
    (
        "P_RemnantOfTheCompliant",
        "������ ���� ����",
        "����ǰ�� 1�� �����Ͽ� �����ϴ�.",
        1,
        S_ProductTypeEnum.Loot,
        S_ProductModifyEnum.Remove,
        1
    ) { }

    public override void BuyProduct()
    {
        S_StoreInfoSystem.Instance.StartRemoveLoot();
    }

    public override S_Product Clone()
    {
        return new S_RemnantOfTheCompliant();
    }
}
