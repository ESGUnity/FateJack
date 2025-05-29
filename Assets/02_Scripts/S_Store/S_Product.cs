using UnityEngine;

public class S_Product
{
    public string Key;
    public string Name;
    public string Description;
    public int Price;
    public S_ProductTypeEnum Type;
    public S_ProductModifyEnum Modify;
    public int SelectCount;

    public S_Product(string key, string name, string description, int price, S_ProductTypeEnum type, S_ProductModifyEnum modify, int selectCount)
    {
        Key = key;
        Name = name;
        Description = description;
        Price = price;
        Type = type;
        Modify = modify;
        SelectCount = selectCount;
    }

    public virtual void BuyProduct()
    {

    }

    public virtual S_Product Clone()
    {
        return new S_Product(Key, Name, Description, Price, Type, Modify, SelectCount);
    }
}

public enum S_ProductTypeEnum
{
    Card,
    Loot,
}
public enum S_ProductModifyEnum
{
    Add,
    Remove
}