using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_AdditiveDescription : MonoBehaviour
{
    [SerializeField] Image image_Base;
    [SerializeField] TMP_Text text_Title;
    [SerializeField] TMP_Text text_Description;

    #region 카드 설명
    public void SetCardEffectName(S_CardBase card) // 저주받은, 이름, 무게
    {
        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(false);

        StringBuilder sb = new();
        if (card.IsCursed)
        {
            sb.Append("<Accent_Curse>저주받은 카드!</Accent_Curse>\n");
        }
        sb.Append($"{S_CardMetadata.CardDatas[card.Key].Name_Korean}\n{card.Weight}의 무게");

        string title = sb.ToString();
        title = S_TextHelper.ParseText(title, card);
        title = S_TextHelper.WrapText(title, 20);
        text_Title.text = title;
    }
    public void SetCardEffectDescription(S_CardBase card) // 설명, 각인
    {
        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(false);

        StringBuilder sb = new();
        sb.Append($"{S_CardMetadata.CardDatas[card.Key].Description_Korean}"); // 설명 추가
        if (card.Engraving.Count > 0) // 각인이 있는 경우 설명 붙이기
        {
            sb.Append($"\n<Accent_Engraving>{S_CardMetadata.TermDatas[card.Engraving.First().ToString()].Name_Korean}</Accent_Engraving>");
        }

        string title = sb.ToString();
        title = S_TextHelper.ParseText(title, card);
        title = S_TextHelper.WrapText(title, 20);
        text_Title.text = title;
    }
    #endregion
    #region 사이드에 붙을 추가 설명
    public void SetAdditiveDescription(string word)
    {
        if (!S_CardMetadata.TermDatas.ContainsKey(word)) return;

        text_Title.gameObject.SetActive(true);
        text_Description.gameObject.SetActive(true);

        string title = $"<Accent_Basic>{S_CardMetadata.TermDatas[word].Name_Korean}</Accent_Basic>";
        title = S_TextHelper.ParseText(title);
        text_Title.text = title;

        string description = S_CardMetadata.TermDatas[word].Description_Korean;
        description = S_TextHelper.ParseText(description);
        description = S_TextHelper.WrapText(description, 20);
        text_Description.text = description;
    }
    #endregion
}