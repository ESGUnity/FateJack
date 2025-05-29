using System.Collections.Generic;
using UnityEngine;

public class S_RemnantOfTheBrokenSoul : S_Product
{
    public S_RemnantOfTheBrokenSoul() : base
    (
        "P_RemnantOfTheBrokenSoul",
        "부서진 영혼의 잔해",
        "무작위 전리품 3개 중 1개를 얻습니다.",
        5,
        S_ProductTypeEnum.Loot,
        S_ProductModifyEnum.Add,
        1
    ) { }

    public override void BuyProduct()
    {
        List<S_Skill> loots = S_SkillList.Instance.PickRandomSkills(3);

        foreach (S_Skill loot in loots)
        {
            Debug.Log(loot.Name);
        }
        S_StoreInfoSystem.Instance.StartSelectOptionLoots(loots);
    }

    public override S_Product Clone()
    {
        return new S_RemnantOfTheBrokenSoul();
    }
}
