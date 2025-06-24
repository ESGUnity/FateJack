using System.Collections.Generic;
using System;
using UnityEngine;

public static class S_ProductMetaData
{
    #region JSON
    public static readonly Dictionary<S_ProductInfoEnum, S_ProductProperty> ProductProperty = new()
    {
        { S_ProductInfoEnum.ThanatosBundle, new S_ProductProperty(7, "타나토스의 보따리", "이것은 타나토스의 보따리입니다. 무작위 쓸만한 물건 3개 중 1개를 얻을 수 있습니다.", "쓸만한 물건 1개를 선택해주세요!") },
        { S_ProductInfoEnum.OldMold, new S_ProductProperty(5, "낡은 거푸집", "그건 낡은 거푸집입니다. 무작위 카드 5장 중 1장을 얻을 수 있습니다.", "덱에 추가할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.MeltedMold, new S_ProductProperty(5, "녹아내린 거푸집", "이것은 녹아내린 거푸집입니다. 힘 카드 3장 중 1장을 얻을 수 있습니다.", "덱에 추가할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.SpiritualMold, new S_ProductProperty(5, "영험한 거푸집", "이것은 영험한 거푸집입니다. 정신력 카드 3장 중 1장을 얻을 수 있습니다.", "덱에 추가할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.BrightMold, new S_ProductProperty(5, "빛나는 거푸집", "이것은 빛나는 거푸집입니다. 행운 카드 3장 중 1장을 얻을 수 있습니다.", "덱에 추가할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.DelicateMold, new S_ProductProperty(5, "정교한 거푸집", "이것은 정교한 거푸집입니다. 공용 카드 3장 중 1장을 얻을 수 있습니다.", "덱에 추가할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.ErisDice, new S_ProductProperty(3, "에리스의 주사위", "이것은 에리스의 주사위입니다. 덱에서 카드 10장을 보고 그 중 1장의 효과를 변경할 수 있습니다.", "효과를 변경할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.GerasBlueprint, new S_ProductProperty(5, "게라스의 설계도", "이것은 게라스의 설계도입니다. 덱에서 카드 10장을 보고 그 중 1장의 효과를 같은 유형의 효과로 변경합니다.", "효과를 변경할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.HypnosBrush, new S_ProductProperty(3, "히프노스의 붓", "이것은 히프노스의 붓입니다. 덱에서 카드 10장을 보고 그 중 1장의 각인을 제거할 수 있습니다.", "각인을 제거할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.OneiroiChisel, new S_ProductProperty(3, "오네이로이의 끌", "이것은 오네이로이의 끌입니다. 덱에서 카드 10장을 보고 그 중 1장에 각인을 부여할 수 있습니다.", "각인을 부여할 카드 1장을 선택해주세요!") },
        { S_ProductInfoEnum.PlutoChisel, new S_ProductProperty(3, "플루토의 끌", "이것은 플루토의 끌입니다. 덱에서 뒤집을 수 있는 각인의 카드 중 1장의 각인을 뒤집을 수 있습니다.", "각인을 뒤집을 카드 1장을 선택해주세요~!") },
        { S_ProductInfoEnum.OracleBall, new S_ProductProperty(0, "예지 구슬", "그건 다음 적의 체력과 능력을 볼 수 있는 예지 구슬입니다.", "예지 구슬은 비매품입니다. 보기보다 굉장히 위험한 물건이거든요.. 적 뿐만 아니라 누구의 결말이든 알 수 있어서요.") },
        { S_ProductInfoEnum.WasteBasket, new S_ProductProperty(0, "휴지통", "그건 휴지통입니다. 당신의 쓸만한 물건 중 1개를 선택해 제거할 수 있습니다. 선택한 물건은 쓸모없는 물건이 되겠네요!", "제거할 쓸만한 물건 1개를 선택해주세요! 되돌릴 수 없으니 신중하게 선택하길 바랍니다.") },
        { S_ProductInfoEnum.ShellGameCup, new S_ProductProperty(0, "야바위 컵", "그건 야바위 컵입니다. 당신의 쓸만한 물건 중 1개를 선택해 왼쪽으로 이동시킬 수 있습니다. 때론 발동 순서가 중요한 법이죠!", "왼쪽으로 이동시킬 쓸만한 물건 1개를 선택해주세요!") },
    };
    #endregion
    #region 주요 함수
    public static int GetPrice(S_ProductInfoEnum product)
    {
        return ProductProperty.TryGetValue(product, out var property) ? property.Price : 0;
    }
    public static string GetName(S_ProductInfoEnum product)
    {
        return ProductProperty.TryGetValue(product, out var property) ? property.Name : string.Empty;
    }
    public static string GetProductDescription(S_ProductInfoEnum product)
    {
        return ProductProperty.TryGetValue(product, out var property) ? property.ProductDescription : string.Empty;
    }
    public static string GetBuiedProductDescription(S_ProductInfoEnum product)
    {
        return ProductProperty.TryGetValue(product, out var property) ? property.BuiedProductDescription : string.Empty;
    }
    #endregion
}

public struct S_ProductProperty
{
    public int Price;
    public string Name;
    public string ProductDescription;
    public string BuiedProductDescription;

    public S_ProductProperty(int price, string name, string productDescription, string buiedProductDescription)
    {
        Price = price;
        Name = name;
        ProductDescription = productDescription;
        BuiedProductDescription = buiedProductDescription;
    }
}