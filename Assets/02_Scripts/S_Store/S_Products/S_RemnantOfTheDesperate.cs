using System.Collections.Generic;
using UnityEngine;

public class S_RemnantOfTheDesperate : S_Product
{
    public S_RemnantOfTheDesperate() : base
    (
        "P_RemnantOfTheDesperate",
        "������ ���� ����",
        "������ ����ǰ 6�� �� 1���� ����ϴ�.",
        7,
        S_ProductTypeEnum.Loot,
        S_ProductModifyEnum.Add,
        1
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
        return new S_RemnantOfTheDesperate();
    }
}
