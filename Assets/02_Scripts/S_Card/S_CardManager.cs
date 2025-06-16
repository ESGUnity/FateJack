using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_CardManager : MonoBehaviour
{
    [Header("카드 생성 관련")]
    int idCount = 0;

    // 싱글턴
    static S_CardManager instance;
    public static S_CardManager Instance { get { return instance; } }

    void Awake()
    {
        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<S_Card> GenerateCardByStartGame() // 게임 시작 시 최초 덱 생성
    {
        List<S_Card> cardList = new();

        Dictionary<S_CardTypeEnum, S_CardEffectEnum> cEList = new()
        {
            { S_CardTypeEnum.Str, S_CardEffectEnum.Str_Stimulus }, { S_CardTypeEnum.Str, S_CardEffectEnum.Str_Stimulus }, { S_CardTypeEnum.Str, S_CardEffectEnum.Str_Stimulus },
            { S_CardTypeEnum.Mind, S_CardEffectEnum.Mind_Focus }, { S_CardTypeEnum.Mind, S_CardEffectEnum.Mind_Focus }, { S_CardTypeEnum.Mind, S_CardEffectEnum.Mind_Focus },
            { S_CardTypeEnum.Luck, S_CardEffectEnum.Luck_Chance }, { S_CardTypeEnum.Luck, S_CardEffectEnum.Luck_Chance }, { S_CardTypeEnum.Luck, S_CardEffectEnum.Luck_Chance },
            { S_CardTypeEnum.Str, S_CardEffectEnum.Str_ZenithBreak }, { S_CardTypeEnum.Mind, S_CardEffectEnum.Mind_DeepInsight }, { S_CardTypeEnum.Luck, S_CardEffectEnum.Luck_Disorder },
            { S_CardTypeEnum.Str, S_CardEffectEnum.Str_WrathStrike },{ S_CardTypeEnum.Mind, S_CardEffectEnum.Mind_PreciseStrike }, { S_CardTypeEnum.Luck, S_CardEffectEnum.Luck_SuddenStrike },
        };

        foreach (S_CardTypeEnum cE in cEList.Keys)
        {
            cardList.Add(GenerateCard(cE, cEList[cE]));
        }

        return cardList;
    }
    public S_Card GenerateCard(S_CardTypeEnum cardType, S_CardEffectEnum cardEffect, S_EngravingEnum engraving = S_EngravingEnum.None)
    {
        int num = S_CardEffectMetadata.GetWeights(cardEffect) + S_CardEffectMetadata.GetWeights(engraving);
        num = num < 0 ? 0 : num;    

        S_Card card = new S_Card(idCount, num, cardType, cardEffect, engraving);

        idCount++;

        return card;
    }
    public S_Card CopyCard(S_Card card)
    {
        S_Card copy = new S_Card(idCount, card.Num, card.CardType, card.CardEffect, card.Engraving);

        idCount++;

        return copy;
    }
    public S_Card GenerateRandomCard(S_CardTypeEnum type = default)
    {
        S_CardEffectEnum cardEffect;
        if (type == default)
        {
            cardEffect = GetRandomCardEffect(out type);
        }
        else
        {
            cardEffect = GetCardEffectByCardType(type);
        }

        S_EngravingEnum[] engravingEffectArray = System.Enum.GetValues(typeof(S_EngravingEnum))
            .Cast<S_EngravingEnum>()
            .ToArray();

        S_EngravingEnum engraving = engravingEffectArray[Random.Range(0, engravingEffectArray.Length)];

        return GenerateCard(type, cardEffect, engraving);
    }
    S_CardEffectEnum GetRandomCardEffect(out S_CardTypeEnum cardType) // 랜덤한 카드 효과 설정
    {
        S_CardEffectEnum[] array = System.Enum.GetValues(typeof(S_CardEffectEnum))
            .Cast<S_CardEffectEnum>()
            .Where(x => x != S_CardEffectEnum.None)
            .ToArray();

        int randomIndex = Random.Range(0, array.Length);

        cardType = S_CardEffectMetadata.GetCardTypeFromEffect(array[randomIndex]);
        return array[randomIndex];
    }
    S_CardEffectEnum GetCardEffectByCardType(S_CardTypeEnum cardType) // 특정 타입의 카드 효과 설정
    {
        List<S_CardEffectEnum> list = S_CardEffectMetadata.GetCardEffectsByType(cardType);

        int randomIndex = Random.Range(0, list.Count);

        return list[randomIndex];
    }
    S_EngravingEnum GetRandomEngravingEffect() // 무작위 각인 설정
    {
        // 각인 효과 설정
        S_EngravingEnum[] array = System.Enum.GetValues(typeof(S_EngravingEnum))
            .Cast<S_EngravingEnum>()
            .Where(x => x != S_EngravingEnum.None)
            .ToArray();

        return array[Random.Range(0, array.Length)];
    }

    #region 상품에 의한 카드 효과 변경
    public void ChangeCardEffect(S_Card card) // 카드의 효과 변경
    {
        card.CardEffect = GetRandomCardEffect(out S_CardTypeEnum cardType);
        card.CardType = cardType;
    }
    public void ChangeSameTypeCardEffect(S_Card card) // 같은 유형의 카드로 효과 변경
    {
        S_CardEffectEnum cardEffect = GetCardEffectByCardType(card.CardType);
        card.CardEffect = cardEffect;
    }
    public void DeleteEngraving(S_Card card) // 카드의 각인 제거
    {
        card.Engraving = S_EngravingEnum.None;
    }
    public void GrantEngraving(S_Card card) // 카드에 각인 부여
    {
        S_EngravingEnum engraving = GetRandomEngravingEffect();
        card.Engraving = engraving;
    }
    public void FlipEngraving(S_Card card)
    {
        string name = card.Engraving.ToString();

        if (name.EndsWith("_Flip"))
        {
            string baseName = name.Substring(0, name.Length - "_Flip".Length);
            if (System.Enum.TryParse(baseName, out S_EngravingEnum result))
            {
                card.Engraving = result;
            }
        }
        else
        {
            string baseName = $"{name}_Flip";
            if (System.Enum.TryParse(baseName, out S_EngravingEnum result))
            {
                card.Engraving = result;
            }
        }
    }
    #endregion
}
public enum S_BattleStatEnum
{
    None,
    Str,
    Mind,
    Luck,
    Random,
    Str_Mind,
    Str_Luck,
    Mind_Luck,
    AllStat
}
public enum S_CardTypeEnum
{
    None,
    Str,
    Mind,
    Luck,
    Common
}
public enum S_CardEffectEnum
{
    None,
    Str_Stimulus, Str_ZenithBreak, Str_SinisterImpulse, Str_CalamityApproaches, Str_UntappedPower, Str_UnjustSacrifice, 
    Str_WrathStrike, Str_EngulfInFlames, Str_FinishingStrike, Str_FlowingSin, Str_BindingForce, Str_Grudge,
    Mind_Focus, Mind_DeepInsight, Mind_PerfectForm, Mind_Unshackle, Mind_Drain, Mind_WingsOfFreedom,
    Mind_PreciseStrike, Mind_SharpCut, Mind_Split, Mind_Accept, Mind_Dissolute, Mind_Awakening,
    Luck_Chance, Luck_Disorder, Luck_Composure, Luck_SilentDomination, Luck_Artifice, Luck_AllForOne,
    Luck_SuddenStrike, Luck_CriticalBlow, Luck_ForcedTake, Luck_Grill, Luck_Shake, Luck_FatalBlow,
    Common_Trinity, Common_Balance,
    Common_Berserk, Common_Carnage, Common_LastStruggle,
    Common_Resistance, Common_Realization,
    Common_Corrupt, Common_Imitate,
    Common_Plunder,
    Common_Undertow,
    Common_Adventure, Common_Inspiration, Common_Repose,
}
public enum S_EngravingEnum
{
    None,
    Reverb,
    Resolve,
    Legion, Legion_Flip, AllOut_Flip, AllOut,
    Delicacy, Delicacy_Flip, Precision, Precision_Flip,
    Resection, Resection_Flip, Patience, Patience_Flip,
    Overflow, Overflow_Flip, Fierce, Fierce_Flip,
    GrandChaos, GrandChaos_Flip, Crush, Crush_Flip,
    Overdrive, Immersion,
    Finale, Climax,
    Immunity, 
    Omen,
    Greed,
    Unleash,
    Flexible,
    QuickAction,
    Spell, 
    DeepShadow, 
}
