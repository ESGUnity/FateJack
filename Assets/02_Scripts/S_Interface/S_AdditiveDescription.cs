using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_AdditiveDescription : MonoBehaviour
{
    [SerializeField] Image image_Base;
    [SerializeField] TMP_Text text_Title;
    [SerializeField] TMP_Text text_Description;

    // 폰트 크기
    const float TITLE_FONT_SIZE = 24f;
    const float DESCRIPTION_FONT_SIZE = 21f;
    const float ADDITIVE_TITLE_FONT_SIZE = 18f;
    const float ADDITIVE_DESCRIPTION_FONT_SIZE = 15f;

    // 폰트 색깔
    Color origionTextColor = new Color(0.95f, 0.95f, 0.95f, 1f);

    // 패널 색깔
    Color originPanelColor = new Color(0f, 0f, 0f, 1f);
    Color transparentPanelColor = new Color(0f, 0f, 0f, 0f);
    Color additiveDescriptionPanelColor = new Color(0.3f, 0.3f, 0.3f, 1f);


    #region 카드 설명
    public void SetCursedAndGenText(S_Card card)
    {
        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(false);

        StringBuilder sb = new();
        if (card.IsGenerated)
        {
            sb.Append("생성된 카드");
        }
        if (card.IsCursed)
        {
            if (sb.Length > 0)
            {
                sb.Append($"\n저주받은 카드!");
            }
            else
            {
                sb.Append("저주받은 카드!");
            }
        }
        text_Title.text = sb.ToString();
        text_Title.fontSize = TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        image_Base.color = transparentPanelColor;
    }
    public void SetCardEffectDescription(S_Card card)
    {
        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(true);

        text_Title.text = $"{S_CardEffectMetadata.GetName(card.CardEffect)}\n{S_CardEffectMetadata.GetWeights(card.CardEffect)}의 무게";
        text_Title.fontSize = TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = $"{S_PlayerCard.Instance.GetCardEffectDescription(card)}";
        text_Description.fontSize = DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = originPanelColor;
    }
    public void SetEngravingDescription(S_Card card)
    {
        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(true);

        text_Title.text = $"{S_CardEffectMetadata.GetName(card.Engraving)}\n{S_CardEffectMetadata.GetWeights(card.Engraving)}의 무게";
        text_Title.fontSize = TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.text = $"{S_PlayerCard.Instance.GetEngravingDescription(card)}";
        text_Description.fontSize = DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = originPanelColor;
    }
    #endregion
    #region 쓸만한 물건 설명
    public void SetTrinketName(S_Trinket tri)
    {
        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(false);

        text_Title.text = $"{tri.Name}";
        text_Title.fontSize = TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        image_Base.color = originPanelColor;
    }
    public void SetTrinektDescription(S_Trinket tri)
    {
        text_Title.gameObject.SetActive(false);
        text_Description.gameObject.SetActive(true);

        text_Description.text = $"{S_PlayerTrinket.Instance.GetTrinketDescription(tri)}";
        text_Description.fontSize = DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = originPanelColor;
    }
    #endregion
    #region 사이드에 붙을 추가 설명
    public void SetAdditiveDescription(S_AdditiveDescriptionTargetWordEnum word)
    {
        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(true);

        // 단어 세팅
        switch (word)
        {
            case S_AdditiveDescriptionTargetWordEnum.Gen:
                text_Title.text = $"생성된 카드";
                text_Description.text = $"이 카드는 시련 종료 후 사라집니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.Cursed:
                text_Title.text = $"저주받은 카드";
                text_Description.text = $"이 카드는 효과가 발동하지 않습니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.Curse:
                text_Title.text = $"저주";
                text_Description.text = $"저주받은 카드는 효과가 발동하지 않습니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.Burst:
                text_Title.text = $"버스트";
                text_Description.text = $"무게가 한계를 초과하여 모든 피해량이 0.25배가 되는 디버프 상태입니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.Perfect:
                text_Title.text = $"완벽";
                text_Description.text = $"무게와 한계가 같아 모든 피해량이 2배가 되는 버프 상태입니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.Delusion:
                text_Title.text = $"망상";
                text_Description.text = $"다음에 내는 카드를 저주하는 디버프 상태입니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.Expansion:
                text_Title.text = $"전개";
                text_Description.text = $"카드 보기가 2장 증가하는 버프 상태입니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.First:
                text_Title.text = $"우선";
                text_Description.text = $"카드를 낼 때 최대한 완벽이 되는 무게의 카드를 낼 수 있는 버프 상태입니다."; break;
            case S_AdditiveDescriptionTargetWordEnum.ColdBlood:
                text_Title.text = $"냉혈";
                text_Description.text = $"다음에 내는 카드는 무게를 얻지 않는 버프 상태입니다."; break;
        }

        text_Title.fontSize = ADDITIVE_TITLE_FONT_SIZE;
        text_Title.color = origionTextColor;

        text_Description.fontSize = ADDITIVE_DESCRIPTION_FONT_SIZE;
        text_Description.color = origionTextColor;

        image_Base.color = additiveDescriptionPanelColor;
    }
    #endregion
}

public enum S_AdditiveDescriptionTargetWordEnum
{
    None, Gen, Cursed, Curse, Burst, Perfect, Delusion, Expansion, First, ColdBlood, 
}