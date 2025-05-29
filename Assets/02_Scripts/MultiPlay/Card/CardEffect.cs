using System.Collections.Generic;

public enum CardEffectRankEnum
{
    Void,
    Normal,
    Rare,
    Epic,
    Mythic
}

public enum CardEffectTypeEnum
{
    RoundEnd,
    Hit,
    DirectHit,
    Burst,
    Stand,
}

public class CardEffect
{
    public string Key;
    public string Name;
    public string Description;
    public CardEffectRankEnum Rank;
    public CardEffectTypeEnum Type;

    public CardEffect(string key, string name, string description, CardEffectRankEnum rank, CardEffectTypeEnum type)
    {
        Key = key;
        Name = name;
        Description = description;
        Rank = rank;
        Type = type;
    }

    public virtual List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        return null;
    }
}