using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class S_HoverCardSystem : MonoBehaviour
{
    // ������Ʈ
    GameObject panel_HoverCardBase;
    Image image_HoverCardBase;
    TMP_Text text_SuitAndNumber;
    TMP_Text text_CardEffectName;
    TMP_Text text_CardEffectCondition;
    TMP_Text text_CardEffectDescription;

    // ȿ�� ����
    Color blackSuitColor = Color.black;
    Color redSuitColor = new Color(0.9f, 0, 0, 1);

    // ī�� ���� �г� ��ġ
    Vector3 rightPanelPos = new Vector3(1.9f, 1.18f, 0);
    Vector3 leftPanelPos = new Vector3(-1.9f, 1.18f, 0);

    // �߰� ���� �г�
    List<GameObject> rightAdditivePanel = new();
    List<GameObject> leftAdditivePanel = new();

    // �����÷ο� ���� �˻��
    S_GameFlowStateEnum prevState;

    // ��� 
    HashSet<string> conditionWords = new HashSet<string> { "�ܷ�", "����", "����", "����", "�޾Ƹ�", "������", "��Ż", "���ĵ�" };
    HashSet<string> typeWords = new HashSet<string> { "����", "�߰�", "����", "����", "����", "����", "ȸ��", "�ܷ�", "��Ż", "â��", "����", "�ε�", "ȯ��", "����", "����", "��������", "�켱", "����", "�Ҹ�" };

    // �̱���
    static S_HoverCardSystem instance;
    public static S_HoverCardSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // ������Ʈ �Ҵ�
        panel_HoverCardBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_HoverCardBase")).gameObject;
        image_HoverCardBase = Array.Find(images, c => c.gameObject.name.Equals("Image_HoverCardBase"));
        text_SuitAndNumber = Array.Find(texts, c => c.gameObject.name.Equals("Text_SuitAndNumber"));
        text_CardEffectName = Array.Find(texts, c => c.gameObject.name.Equals("Text_CardEffectName"));
        text_CardEffectCondition = Array.Find(texts, c => c.gameObject.name.Equals("Text_CardEffectCondition"));
        text_CardEffectDescription = Array.Find(texts, c => c.gameObject.name.Equals("Text_CardEffectDescription"));

        Transform parent;
        parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_RightAdditiveDescriptionBase1"));
        for (int i = 0; i < parent.childCount; i++)
        {
            rightAdditivePanel.Add(parent.GetChild(i).gameObject);
        }
        parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_RightAdditiveDescriptionBase2"));
        for (int i = 0; i < parent.childCount; i++)
        {
            rightAdditivePanel.Add(parent.GetChild(i).gameObject);
        }
        parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_RightAdditiveDescriptionBase3"));
        for (int i = 0; i < parent.childCount; i++)
        {
            rightAdditivePanel.Add(parent.GetChild(i).gameObject);
        }
        parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_LeftAdditiveDescriptionBase1"));
        for (int i = 0; i < parent.childCount; i++)
        {
            leftAdditivePanel.Add(parent.GetChild(i).gameObject);
        }
        parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_LeftAdditiveDescriptionBase2"));
        for (int i = 0; i < parent.childCount; i++)
        {
            leftAdditivePanel.Add(parent.GetChild(i).gameObject);
        }
        parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_LeftAdditiveDescriptionBase3"));
        for (int i = 0; i < parent.childCount; i++)
        {
            leftAdditivePanel.Add(parent.GetChild(i).gameObject);
        }

        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        // ���� �÷ο찡 �ٲ� �� None�̶�� �ڵ����� ��Ȱ��ȭ�Ǵ� ���
        S_GameFlowStateEnum currentState = S_GameFlowManager.Instance.GameFlowState;
        if (prevState != currentState)
        {
            if (currentState == S_GameFlowStateEnum.None)
            {
                DisablePanelOnCard();
            }

            prevState = currentState;
        }
    }
    void ActivePanelOnCard(S_Card card, Vector3 pos)
    {
        // ����� ���� ����
        StringBuilder sb = new();
        string suit = "";
        suit = card.Suit switch
        {
            S_CardSuitEnum.Spade => "�����̵�",
            S_CardSuitEnum.Heart => "��Ʈ",
            S_CardSuitEnum.Diamond => "���̾Ƹ��",
            S_CardSuitEnum.Clover => "Ŭ�ι�",
            _ => ""
        };
        sb.Append(suit);

        string statValue = "";
        statValue = card.StatValue switch
        {
            S_BattleStatEnum.Strength => "(��)",
            S_BattleStatEnum.Mind => "(���ŷ�)",
            S_BattleStatEnum.Luck => "(���)",
            _ => ""
        };
        sb.Append(statValue);

        sb.Append($" {card.Number}");

        if (card.IsIllusion)
        {
            sb.Append("\nȯ��");
        }

        if (card.IsCursed)
        {
            sb.Append("\n���ֹ���!");
        }

        text_SuitAndNumber.text = sb.ToString();

        text_SuitAndNumber.color = card.Suit switch
        {
            S_CardSuitEnum.Spade => blackSuitColor,
            S_CardSuitEnum.Heart => redSuitColor,
            S_CardSuitEnum.Diamond => redSuitColor,
            S_CardSuitEnum.Clover => blackSuitColor,
            _ => blackSuitColor
        };

        // ī�� ȿ�� ����
        text_CardEffectName.text = BoldText(card.CardEffect.Name);

        int index = card.CardEffect.Description.IndexOf('\n');
        string part1 = index >= 0 ? card.CardEffect.Description.Substring(0, index) : card.CardEffect.Description;
        string part2 = index >= 0 ? card.CardEffect.Description.Substring(index + 1) : string.Empty;
        text_CardEffectCondition.text = BoldText(part1);
        text_CardEffectDescription.text = BoldText(part2);

        // �г� Ȱ��ȭ
        panel_HoverCardBase.SetActive(true);

        var rect = panel_HoverCardBase.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
    Vector2 SetPanelPosOnCard(Vector3 pos)
    {
        Vector3 screenPos = Vector3.zero;
        if (pos.x >= 0)
        {
            screenPos = Camera.main.WorldToScreenPoint(pos + leftPanelPos);
        }
        else
        {
            screenPos = Camera.main.WorldToScreenPoint(pos + rightPanelPos);
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel_HoverCardBase.transform.parent as RectTransform,
            screenPos,
            Camera.main,
            out Vector2 localPos);

        panel_HoverCardBase.GetComponent<RectTransform>().anchoredPosition = localPos;

        return localPos;
    }
    string BoldText(string text)
    {
        text = Regex.Replace(text, @"\((.*?)\)", match =>
        {
            string inner = match.Groups[1].Value;
            return $"<b>({inner})</b>";
        });

        foreach (string word in conditionWords)
        {
            text = Regex.Replace(text, word, $"<b>{word}</b>");
        }

        foreach (string word in typeWords)
        {
            text = Regex.Replace(text, word, $"<b>{word}</b>");
        }

        return text;
    }
    void GenerateAdditiveDescription(S_Card card, Vector3 pos, Vector2 panelPos)
    {
        string text = card.CardEffect.Description;
        List<string> words = new();
        foreach (string word in conditionWords)
        {
            if (text.Contains(word) && !words.Contains(word))
            {
                words.Add(word);
            }
        }
        foreach (string word in typeWords)
        {
            if (text.Contains(word) && !words.Contains(word))
            {
                words.Add(word);
            }
        }

        int count = 0;

        // ȯ�� üũ
        if (card.IsIllusion)
        {
            if (!words.Contains("ȯ��"))
            {
                words.Add("ȯ��");
            }
        }

        // ���� üũ
        if (card.IsCursed) 
        {
            if (!words.Contains("����"))
            {
                words.Add("����");
            }
        }

        foreach (string word in words)
        {
            if (pos.x >= 0)
            {
                leftAdditivePanel[count].GetComponent<S_AdditiveDescription>().SetAdditiveDescriptionText(word);
                leftAdditivePanel[count].SetActive(true);
            }
            else
            {
                rightAdditivePanel[count].GetComponent<S_AdditiveDescription>().SetAdditiveDescriptionText(word);
                rightAdditivePanel[count].SetActive(true);
            }
            count++;
        }
    }

    public void ActivePanelByStackCard(S_Card card, Vector3 pos)
    {
        Vector2 panelPos = SetPanelPosOnCard(pos);
        GenerateAdditiveDescription(card, pos, panelPos);
        ActivePanelOnCard(card, pos);
    }
    public void ActivePanelByDeckCard(S_Card card, Vector3 pos)
    {
        Vector2 panelPos = SetPanelPosOnCard(pos);
        GenerateAdditiveDescription(card, pos, panelPos);
        ActivePanelOnCard(card, pos);
    }


    public void DisablePanelOnCard()
    {
        if (panel_HoverCardBase.activeInHierarchy)
        {
            panel_HoverCardBase.SetActive(false);
        }

        foreach (GameObject go in rightAdditivePanel)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in leftAdditivePanel)
        {
            go.SetActive(false);
        }
    }
}