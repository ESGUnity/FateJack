using System;
using TMPro;
using UnityEngine;

public class WorldUICardInfo : MonoBehaviour
{
    // ÄÄÆ÷³ÍÆ®
    GameObject cardInfoImage;
    TMP_Text cardInfoTitle;
    TMP_Text cardInfoNumber;
    TMP_Text cardInfoText;

    // ½Ì±ÛÅÏ
    static WorldUICardInfo instance;
    public static WorldUICardInfo Instance {  get { return instance; } }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        cardInfoImage = Array.Find(transforms, c => c.gameObject.name.Equals("CardInfoImage")).gameObject;
        cardInfoTitle = Array.Find(texts, c => c.gameObject.name.Equals("CardInfoTitle"));
        cardInfoNumber = Array.Find(texts, c => c.gameObject.name.Equals("CardInfoNumber"));
        cardInfoText = Array.Find(texts, c => c.gameObject.name.Equals("CardInfoText"));
    }

    public void DisplayCardInfo(bool active, Card card = default)
    {
        if (active)
        {
            cardInfoImage.SetActive(true);
            CardEffect cardEffect = CardEffectList.FindCardEffectToKey(card.CardEffectKey.ToString());
            cardInfoTitle.text = cardEffect.Name;
            cardInfoNumber.text = card.Number.ToString();
            cardInfoText.text = cardEffect.Description;
        }
        else
        {
            cardInfoImage.SetActive(false);
            cardInfoTitle.text = "-";
            cardInfoNumber.text = "-";
            cardInfoText.text = "-";
        }
    }
}
