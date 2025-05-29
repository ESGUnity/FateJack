using System.Collections.Generic;
using UnityEngine;

public static class S_ProductList
{
    public static List<S_Product> CardProducts = new()
    {
        new S_FaintDetermination(), new S_TurbulentDetermination(), new S_GreatDetermination(), new S_DeterminationOfGod(), new S_DreamingDetermination(), 
        new S_RebelliousDetermination(), new S_ImmortalDetermination(), new S_TwistedDetermination(), new S_BurningDetermination()
    };
    public static List<S_Product> LootProducts = new()
    {
        new S_RemnantOfTheBrokenSoul(), new S_RemnantOfTheDesperate(), new S_RemnantOfToughness(), new S_RemnantOfTheCompliant()
    };
    public static List<S_Product> FreeProducts = new()
    {
        new S_CreatureLost(), new S_CreatureThickThread(), 
    };

    public static S_Product PickRandomProductByType(S_ProductTypeEnum type)
    {
        List<S_Product> products = new();

        switch (type)
        {
            case S_ProductTypeEnum.Card:
                products = CardProducts;
                break;
            case S_ProductTypeEnum.Loot:
                products = LootProducts;
                break;
        }

        int randomIndex = Random.Range(0, products.Count);

        return products[randomIndex].Clone();
    }
    public static S_Product PickFreeProduct(int currentTrial)
    {
        if (currentTrial % 3 == 0) // 강력한 피조물을 상대했었다면 좋은 무료 상품
        {
            return new S_CreatureThickThread().Clone();
        }
        else // 아니라면 그냥 무료 상품
        {
            return new S_CreatureLost().Clone();
        }
    }
}
