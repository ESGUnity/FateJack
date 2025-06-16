using System;

public class S_Card
{
    public int Id;
    public int Num;

    public S_CardTypeEnum CardType;
    public S_CardEffectEnum CardEffect;
    public S_EngravingEnum Engraving;

    public S_BattleStatEnum Stat; // 무작위 능력치 전용(Berserk와 Balance)
    public int ActivatedCount; // 각인 전용
    public int ExpectedValue; // 효과 발동 시 예상 값(피해량이나 능력치 배율 등). 능력치의 Description용

    public bool IsCurrentTurn;
    public bool IsInDeck;
    public bool IsGenerated;
    public bool IsCursed;
    public bool IsMeetCondition; // 실제 로직 발동과 관련된 중요한 변수
    public bool IsEngravingActiaved; // 각인 빛나는 효과 전용

    public S_Card(int id, int num, S_CardTypeEnum cardType, S_CardEffectEnum cardEffect, S_EngravingEnum engraving)
    {
        Id = id;
        Num = num;

        CardType = cardType;
        CardEffect = cardEffect;
        Engraving = engraving;

        Stat = S_BattleStatEnum.None;
        ActivatedCount = 0;
        ExpectedValue = 0;

        IsCurrentTurn = false;
        IsInDeck = false;
        IsGenerated = false;
        IsCursed = false;
        IsMeetCondition = false;
        IsEngravingActiaved = false;
    }
}