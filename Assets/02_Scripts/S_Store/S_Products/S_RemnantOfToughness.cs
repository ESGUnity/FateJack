using System.Collections.Generic;
using UnityEngine;

public class S_RemnantOfToughness : S_Product
{
    public S_RemnantOfToughness() : base
    (
        "P_RemnantOfToughness",
        "������ ���� ����",
        "������ ����ǰ 6�� �� 2���� ����ϴ�.",
        9,
        S_ProductTypeEnum.Loot,
        S_ProductModifyEnum.Add,
        2
    ) { }

    public override void BuyProduct()
    {
        List<S_Skill> loots = S_SkillList.Instance.PickRandomSkills(6);

        foreach (S_Skill loot in loots)
        {
            Debug.Log(loot.Name);
        }
        S_StoreInfoSystem.Instance.StartSelectOptionLoots(loots);
    }

    public override S_Product Clone()
    {
        return new S_RemnantOfToughness();
    }
}
