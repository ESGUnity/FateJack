using System;

public class S_Card : IEquatable<S_Card>
{
    public int Id;
    public int Number;
    public S_CardSuitEnum Suit;
    public S_BattleStatEnum StatValue;
    public S_CardBasicConditionEnum BasicCondition;
    public S_CardAdditiveConditionEnum AdditiveCondition;
    public S_CardDebuffConditionEnum DebuffCondition;
    public S_CardBasicEffectEnum BasicEffect;
    public S_CardAdditiveEffectEnum AdditiveEffect;
    public bool IsCursed;
    public bool IsCurrentTurnHit;
    public bool IsIllusion;
    public bool IsInDeck;
    public bool CanActivateEffect;

    public S_Card(int id, int number, S_CardSuitEnum suit, S_BattleStatEnum statValue,
        S_CardBasicConditionEnum basicCondition, S_CardAdditiveConditionEnum additiveCondition, S_CardDebuffConditionEnum debuffCondition, S_CardBasicEffectEnum basicEffect, S_CardAdditiveEffectEnum additiveEffect,
        bool isCursed = false, bool isCurrentTurnHit = false, bool isIllusion = false, bool isInDeck = false, bool canActivateEffect = false)
    {
        Id = id;
        Number = number;
        Suit = suit;
        StatValue = statValue;
        BasicCondition = basicCondition;
        AdditiveCondition = additiveCondition;
        DebuffCondition = debuffCondition;
        BasicEffect = basicEffect;
        AdditiveEffect = additiveEffect;
        IsCursed = isCursed;
        IsCurrentTurnHit = isCurrentTurnHit;
        IsIllusion = isIllusion;
        IsInDeck = isInDeck;
        CanActivateEffect = canActivateEffect;
    }

    public bool Equals(S_Card other)
    {
        if (other == null) return false;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}