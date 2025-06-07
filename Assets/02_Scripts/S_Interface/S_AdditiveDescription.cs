using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_AdditiveDescription : MonoBehaviour
{
    [SerializeField] Image image_Base;
    [SerializeField] TMP_Text text_Title;
    [SerializeField] TMP_Text text_Description;

    // 문양과 숫자 관련
    Color suitAndNumberPanelColor = new Color(0f, 0f, 0f, 0f);
    Color suitAndNumberTextRedColor = new Color(0.85f, 0f, 0f, 1f);
    Color suitAndNumberTextBlackColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    Color basicEffectByNumberPanelColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    const float SUIT_AND_NUMBER_FONT_SIZE = 24f;

    Color conditionPanelColor = new Color(0.27f, 0.6f, 0.8f, 1f);
    Color debuffPanelColor = new Color(0f, 0.5f, 0.8f, 1f);
    Color effectPanelColor = new Color(0.26f, 0.6f, 0.46f, 1f);
    Color additiveDescriptionPanelColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    const float ORIGIN_TITLE_FONT_SIZE = 21f;
    const float ORIGIN_DESCRIPTION_FONT_SIZE = 17f;
    const float ADDITIVE_TITLE_FONT_SIZE = 17f;
    const float ADDITIVE_DESCRIPTION_FONT_SIZE = 15f;

    Color origionTextColor = new Color(0.95f, 0.95f, 0.95f, 1f);

    #region 카드 설명
    public void SetDescription(S_CardSuitEnum suit, int number) // 수트와 숫자에 따른 패널 생성
    {
        string suitText = "";
        suitText = suit switch
        {
            S_CardSuitEnum.Spade => "스페이드",
            S_CardSuitEnum.Heart => "하트",
            S_CardSuitEnum.Diamond => "다이아몬드",
            S_CardSuitEnum.Clover => "클로버",
            _ => ""
        };

        text_Title.text = $"{suitText}의 {number}";
        if (suit == S_CardSuitEnum.Heart || suit == S_CardSuitEnum.Diamond)
        {
            text_Title.color = suitAndNumberTextRedColor;
        }
        else
        {
            text_Title.color = suitAndNumberTextBlackColor;
        }
        text_Title.fontSize = SUIT_AND_NUMBER_FONT_SIZE;

        text_Description.text = "";

        image_Base.color = suitAndNumberPanelColor;
    }
    public void SetDescription(S_BattleStatEnum stat, int number, S_Card card) // 스탯과 숫자에 따른 패널 생성
    {
        StringBuilder sb = new();
        sb.Append($"숫자 합 {number} 증가");

        string statText = "";
        statText = stat switch
        {
            S_BattleStatEnum.Strength => "힘",
            S_BattleStatEnum.Mind => "정신력",
            S_BattleStatEnum.Luck => "행운",
            S_BattleStatEnum.Random => "무작위 능력치",
            _ => ""
        };
        sb.Append($"\n{statText} {number} 증가");
        text_Title.text = sb.ToString();
        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        StringBuilder sb2 = new();
        if (card.IsCursed)
        {
            sb2.Append($"저주받음!\n");
        }
        if (card.IsIllusion)
        {
            sb2.Append($"생성됨");
        }
        text_Description.text = sb2.ToString();
        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = basicEffectByNumberPanelColor;
    }
    public void SetDescription(S_CardBasicConditionEnum bc)
    {
        text_Title.text = $"조건({S_CardEffectMetadata.GetWeights(bc)})";
        text_Title.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetName(bc);
        text_Description.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = conditionPanelColor;
    }
    public void SetDescription(S_CardAdditiveConditionEnum ac, S_Card card)
    {
        text_Title.text = $"조건 제약({S_CardEffectMetadata.GetWeights(ac)})";
        text_Title.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetName(ac);
        switch (ac)
        {
            case S_CardAdditiveConditionEnum.Legion_SameSuit:
                text_Description.text = $"{text_Description.text}\n현재 숫자 합 : {S_EffectChecker.Instance.GetSameSuitSumInStack(card.Suit)}";
                break;
            case S_CardAdditiveConditionEnum.GreatLegion_SameSuit:
                text_Description.text = $"{text_Description.text}\n현재 숫자 합 : {S_EffectChecker.Instance.GetSameSuitSumInStack(card.Suit)}";
                break;
            case S_CardAdditiveConditionEnum.Finale:
                text_Description.text = $"{text_Description.text}\n없는 문양 개수 : {4 - S_EffectChecker.Instance.GetDeckSuitCount()}";
                break;
            case S_CardAdditiveConditionEnum.Finale_Climax:
                text_Description.text = $"{text_Description.text}\n없는 문양 개수 : {4 - S_EffectChecker.Instance.GetDeckSuitCount()}";
                break;
            case S_CardAdditiveConditionEnum.Chaos:
                text_Description.text = $"{text_Description.text}\n조건 만족 문양 : {S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1)}";
                break;
            case S_CardAdditiveConditionEnum.Chaos_Anarchy:
                text_Description.text = $"{text_Description.text}\n조건 만족 문양 : {S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(2)}";
                break;
            case S_CardAdditiveConditionEnum.GrandChaos_Anarchy:
                text_Description.text = $"{text_Description.text}\n조건 만족 문양 : {S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(3)}";
                break;
            case S_CardAdditiveConditionEnum.Chaos_Overflow:
                text_Description.text = $"{text_Description.text}\n조건 만족 문양 : {S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStackInCurrentTurn(1)}";
                break;
            case S_CardAdditiveConditionEnum.Offensive:
                text_Description.text = $"{text_Description.text}\n숫자 최대 길이 : {S_EffectChecker.Instance.GetContinueNumMaxLengthInStack()}";
                break;
            case S_CardAdditiveConditionEnum.Offensive_SameSuit:
                text_Description.text = $"{text_Description.text}\n숫자 최대 길이 : {S_EffectChecker.Instance.GetContinueNumSameSuitMaxLengthInStack()}";
                break;
            case S_CardAdditiveConditionEnum.AllOutOffensive:
                text_Description.text = $"{text_Description.text}\n숫자 최대 길이 : {S_EffectChecker.Instance.GetContinueNumMaxLengthInStack()}";
                break;
            case S_CardAdditiveConditionEnum.AllOutOffensive_SameSuit:
                text_Description.text = $"{text_Description.text}\n숫자 최대 길이 : {S_EffectChecker.Instance.GetContinueNumSameSuitMaxLengthInStack()}";
                break;
            case S_CardAdditiveConditionEnum.Offensive_Overflow:
                text_Description.text = $"{text_Description.text}\n숫자 최대 길이 : {S_EffectChecker.Instance.GetContinueNumMaxLengthInStackInCurrentTurn()}";
                break;
            case S_CardAdditiveConditionEnum.Precision_SameSuit:
                text_Description.text = $"{text_Description.text}\n현재 카드 장수 : {S_EffectChecker.Instance.GetSameSuitCardsInStack(card.Suit).Count}";
                break;
            case S_CardAdditiveConditionEnum.HyperPrecision_SameSuit:
                text_Description.text = $"{text_Description.text}\n현재 카드 장수 : {S_EffectChecker.Instance.GetSameSuitCardsInStack(card.Suit).Count}";
                break;
            case S_CardAdditiveConditionEnum.Precision_SameNumber:
                text_Description.text = $"{text_Description.text}\n현재 카드 장수 : {S_EffectChecker.Instance.GetSameNumberCardsInStack(card.Number).Count}";
                break;
            case S_CardAdditiveConditionEnum.HyperPrecision_SameNumber:
                text_Description.text = $"{text_Description.text}\n현재 카드 장수 : {S_EffectChecker.Instance.GetSameNumberCardsInStack(card.Number).Count}";
                break;
            case S_CardAdditiveConditionEnum.Precision_PlethoraNumber:
                text_Description.text = $"{text_Description.text}\n현재 카드 장수 : {S_EffectChecker.Instance.GetPlethoraNumberCardsInStack().Count}";
                break;
            case S_CardAdditiveConditionEnum.HyperPrecision_PlethoraNumber:
                text_Description.text = $"{text_Description.text}\n현재 카드 장수 : {S_EffectChecker.Instance.GetPlethoraNumberCardsInStack().Count}";
                break;
            case S_CardAdditiveConditionEnum.Overflow:
                text_Description.text = $"{text_Description.text}\n현재 카드 장수 : {S_EffectChecker.Instance.GetCardsInStackInCurrentTurn().Count}";
                break;
            case S_CardAdditiveConditionEnum.Unity:
                text_Description.text = $"{text_Description.text}\n현재 문양 개수 : {S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1)}";
                break;
            case S_CardAdditiveConditionEnum.Unity_Drastic:
                text_Description.text = $"{text_Description.text}\n현재 문양 개수 : {S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1)}";
                break;
        }

        text_Description.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = conditionPanelColor;
    }
    public void SetDescription(S_CardDebuffConditionEnum debuff)
    {
        text_Title.text = $"디버프 제약({S_CardEffectMetadata.GetWeights(debuff)})";
        text_Title.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetName(debuff);
        text_Description.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = debuffPanelColor;
    }
    public void SetDescription(S_CardBasicEffectEnum be)
    {
        text_Title.text = $"효과(-{S_CardEffectMetadata.GetWeights(be)})";
        text_Title.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetName(be);
        text_Description.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = effectPanelColor;
    }
    public void SetDescription(S_CardAdditiveEffectEnum ae, S_Card card)
    {
        text_Title.text = $"추가 효과(-{S_CardEffectMetadata.GetWeights(ae)})";
        text_Title.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetName(ae);
        switch (ae)
        {
            case S_CardAdditiveEffectEnum.Reflux_Stack:
                text_Description.text = $"{text_Description.text}\n발동 횟수 : {S_EffectChecker.Instance.GetCardsInStack().Count / 4}";
                break;
            case S_CardAdditiveEffectEnum.Reflux_PlethoraNumber:
                text_Description.text = $"{text_Description.text}\n발동 횟수 : {S_EffectChecker.Instance.GetPlethoraNumberCardsInStack().Count / 3}";
                break;
            case S_CardAdditiveEffectEnum.Reflux_Deck:
                text_Description.text = $"{text_Description.text}\n발동 횟수 : {S_EffectChecker.Instance.GetCardsInDeck().Count / 6}";
                break;
            case S_CardAdditiveEffectEnum.Reflux_Chaos:
                text_Description.text = $"{text_Description.text}\n발동 횟수 : {S_EffectChecker.Instance.GetSuitCountGreaterThanAmountInStack(1) / 6}";
                break;
            case S_CardAdditiveEffectEnum.Reflux_Offensive:
                text_Description.text = $"{text_Description.text}\n발동 횟수 : {S_EffectChecker.Instance.GetContinueNumMaxLengthInStack() / 2}";
                break;
            case S_CardAdditiveEffectEnum.Reflux_Curse:
                text_Description.text = $"{text_Description.text}\n발동 횟수 : {S_EffectChecker.Instance.GetCursedCardsInDeckAndStack().Count / 3}";
                break;
            case S_CardAdditiveEffectEnum.Reflux_Exclusion:
                text_Description.text = $"{text_Description.text}\n발동 횟수 : {S_PlayerCard.Instance.GetPreExclusionTotalCards().Count / 3}";
                break;
        }
        text_Description.fontSize = ORIGIN_TITLE_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = effectPanelColor;
    }
    #endregion
    #region 카드 추가 설명
    public void SetAdditiveDescription(string text)
    {
        switch (text)
        {
            case "저주받음!":
                text_Title.text = text;
                text_Description.text = "카드의 효과가 발동하지 않습니다.";
                break;
            case "생성됨":
                text_Title.text = text;
                text_Description.text = "이 카드는 시련 종료 후 사라집니다.";
                break;
            default:
                text_Title.text = "ERROR! S_ADDITIVEDESCRIPTION SEND";
                text_Description.text = "ERROR! S_ADDITIVEDESCRIPTION SEND";
                break;
        }
        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = additiveDescriptionPanelColor;
    }
    public void SetAdditiveDescription(S_CardBasicConditionEnum bc)
    {
        text_Title.text = S_CardEffectMetadata.GetName(bc);
        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetDescription(bc);
        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = additiveDescriptionPanelColor;
    }
    public void SetAdditiveDescription(S_CardAdditiveConditionEnum ac)
    {
        text_Title.text = S_CardEffectMetadata.GetName(ac);
        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetDescription(ac);
        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = additiveDescriptionPanelColor;
    }
    public void SetAdditiveDescription(S_CardDebuffConditionEnum debuff)
    {
        text_Title.text = S_CardEffectMetadata.GetName(debuff);
        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetDescription(debuff);
        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = additiveDescriptionPanelColor;
    }
    public void SetAdditiveDescription(S_CardBasicEffectEnum be)
    {
        text_Title.text = S_CardEffectMetadata.GetName(be);
        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetDescription(be);
        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = additiveDescriptionPanelColor;
    }
    public void SetAdditiveDescription(S_CardAdditiveEffectEnum ae)
    {
        text_Title.text = S_CardEffectMetadata.GetName(ae);
        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = S_CardEffectMetadata.GetDescription(ae);
        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = additiveDescriptionPanelColor;
    }
    #endregion
    #region 능력 설명
    public void SetDescriptionBySkillName(S_Skill skill)
    {
        text_Title.text = $"{skill.Name}";
        text_Title.fontSize = SUIT_AND_NUMBER_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = "";
        image_Base.color = suitAndNumberPanelColor;
    }
    public void SetDescriptionBySkillDescription(S_Skill skill)
    {
        text_Title.text = "";

        text_Description.text = $"{skill.GetDescription()}";
        text_Description.fontSize = ORIGIN_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = basicEffectByNumberPanelColor;
    }
    #endregion
}