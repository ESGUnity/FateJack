using System.Collections.Generic;
using UnityEngine;

public static class CardEffectList
{
    // 모든 카드 효과들 리스트
    public static List<CardEffect> CardEffects = new List<CardEffect>()
    {
        new TrinityCE(), new DoppelgangerCE(), new PercyJacksonCE(), new GigaChadCE(), new ManppisppiCE(), new IntenseDistortionCE(), new CaffeineAddictionCE(), new JackBlackCE(), new IdleTransfigurationCE(), new SympathyCE(), // 1
        new MutualAidCE(), new PrimeNumberCE(), new OddOrEvenCE(), new EvenOrOddCE(), new OverthinkingAddictionCE(), new OminousChaosCE(), // 16
        new TrinityCE(), new DoppelgangerCE(), new PercyJacksonCE(), new GigaChadCE(), new ManppisppiCE(), new IntenseDistortionCE(), new CaffeineAddictionCE(), new JackBlackCE(), new IdleTransfigurationCE(), new SympathyCE(), // 1
        new MutualAidCE(), new PrimeNumberCE(), new OddOrEvenCE(), new EvenOrOddCE(), new OverthinkingAddictionCE(), new OminousChaosCE(), // 32
        new TrinityCE(), new DoppelgangerCE(), new PercyJacksonCE(), new GigaChadCE(), new ManppisppiCE(), new IntenseDistortionCE(), new CaffeineAddictionCE(), new JackBlackCE(), new IdleTransfigurationCE(), new SympathyCE(), // 1
        new MutualAidCE(), new PrimeNumberCE(), new OddOrEvenCE(), new EvenOrOddCE(), new OverthinkingAddictionCE(), new OminousChaosCE(), // 48
        new TrinityCE(), new DoppelgangerCE(), new PercyJacksonCE(), new GigaChadCE(), new ManppisppiCE(), new IntenseDistortionCE(), new CaffeineAddictionCE(), new JackBlackCE(), new IdleTransfigurationCE(), new SympathyCE(), // 1
        new MutualAidCE(), new PrimeNumberCE(), new OddOrEvenCE(), new EvenOrOddCE(), new OverthinkingAddictionCE(), new OminousChaosCE(), // 60
        new TrinityCE(), new DoppelgangerCE(), new PercyJacksonCE(), new GigaChadCE(), new ManppisppiCE(), new IntenseDistortionCE(), new CaffeineAddictionCE(), new JackBlackCE(), new IdleTransfigurationCE(), new SympathyCE(), // 1
        new MutualAidCE(), new PrimeNumberCE(), new OddOrEvenCE(), new EvenOrOddCE(), new OverthinkingAddictionCE(), new OminousChaosCE(), // 72
    };

    public static CardEffect FindCardEffectToKey(string cardEffectKey) // Key값으로 카드 효과를 찾아 반환하는 메서드
    {
        foreach (CardEffect cardEffect in CardEffects)
        {
            if (cardEffect.Key == cardEffectKey)
            {
                return cardEffect;
            }
        }

        return null;
    }
    public static CardEffect PickCardEffectByRank(CardEffectRankEnum rank)
    {
        List<CardEffect> cardEffects = new();
        switch (rank)
        {
            case CardEffectRankEnum.Void:
                cardEffects = CardEffects.FindAll(c => c.Rank == CardEffectRankEnum.Void);
                break;
            case CardEffectRankEnum.Normal:
                cardEffects = CardEffects.FindAll(c => c.Rank == CardEffectRankEnum.Normal);
                break;
            case CardEffectRankEnum.Rare:
                cardEffects = CardEffects.FindAll(c => c.Rank == CardEffectRankEnum.Rare);
                break;
            case CardEffectRankEnum.Epic:
                cardEffects = CardEffects.FindAll(c => c.Rank == CardEffectRankEnum.Epic);
                break;
            case CardEffectRankEnum.Mythic:
                cardEffects = CardEffects.FindAll(c => c.Rank == CardEffectRankEnum.Mythic);
                break;
        }

        int randomIndex = Random.Range(0, cardEffects.Count);

        return cardEffects[randomIndex];
    }

}
