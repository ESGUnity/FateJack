using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

// TODO : ������ Ŀ�� ��� ���� ������
public class S_HoverSkillSystem : MonoBehaviour
{
    // ������Ʈ
    GameObject panel_HoverLootBase;
    TMP_Text text_LootName;
    TMP_Text text_LootCondition;
    TMP_Text text_LootDescription;

    // ī�� ���� �г� ��ġ
    Vector2 basicPanelPos = new Vector3(-80, 50);
    Vector3 productRightPanelPos = new Vector3(1.95f, -1.2f, 0);
    Vector3 productLeftPanelPos = new Vector3(-1.95f, -1.2f, 0);

    // �߰� ���� �г�
    List<GameObject> additivePanel = new();

    // �����÷ο� ���� �˻��
    S_GameFlowStateEnum prevState;

    // ��� 
    HashSet<string> conditionWords = new HashSet<string> { "�ܷ�", "����", "����", "����", "�޾Ƹ�", "������", "��Ż", "���ĵ�" };
    HashSet<string> typeWords = new HashSet<string> { "����", "�߰�", "����", "����", "����", "����", "ȸ��", "�ܷ�", "��Ż", "â��", "����", "�ε�", "ȯ��", "����", "����", "��������", "�켱", "����", "�Ҹ�" };

    // �̱���
    static S_HoverSkillSystem instance;
    public static S_HoverSkillSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // ������Ʈ �Ҵ�
        panel_HoverLootBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_HoverLootBase")).gameObject;
        text_LootName = Array.Find(texts, c => c.gameObject.name.Equals("Text_LootName"));
        text_LootCondition = Array.Find(texts, c => c.gameObject.name.Equals("Text_LootCondition"));
        text_LootDescription = Array.Find(texts, c => c.gameObject.name.Equals("Text_LootDescription"));

        Transform parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_AdditiveDescriptionBase"));
        for (int i = 0; i < parent.childCount; i++)
        {
            additivePanel.Add(parent.GetChild(i).gameObject);
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
                PointerExitForSkillObject();
            }

            prevState = currentState;
        }
    }

    public void PointerEnterForSkillObject(S_Skill loot, Vector3 pos)
    {
        GenerateAdditiveDescription(loot.ConditionText + loot.Description);
        ActivePanel(loot, pos, false);
    }
    public void PointerEnterForLootBoardByProduct(S_Skill loot, Vector3 pos)
    {
        GenerateAdditiveDescription(loot.ConditionText + loot.Description);
        ActivePanel(loot, pos, true);
    }
    public void PointerExitForSkillObject()
    {
        if (panel_HoverLootBase.activeInHierarchy)
        {
            panel_HoverLootBase.SetActive(false);
        }

        foreach (GameObject go in additivePanel)
        {
            go.GetComponent<S_AdditiveDescription>().SetAdditiveDescriptionText("");
            go.SetActive(false);
        }
    }
    void GenerateAdditiveDescription(string text)
    {
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
        foreach (string word in words)
        {
            additivePanel[count].GetComponent<S_AdditiveDescription>().SetAdditiveDescriptionText(word);
            additivePanel[count].SetActive(true);
            count++;
        }
    }

    void ActivePanel(S_Skill loot, Vector3 pos, bool isProduct)
    {
        // ī�� ȿ�� ����
        text_LootName.text = BoldingText(loot.Name);
        text_LootCondition.text = BoldingText(loot.ConditionText);

        StringBuilder sb = new();
        sb.Append(loot.Description);
        if (loot.Condition == S_ActivationConditionEnum.Loot_Growth)
        {
            sb.Append($"\n���� ���� : {loot.ActivatedCount}");
        }
        text_LootDescription.text = BoldingText(sb.ToString());

        foreach (var layout in panel_HoverLootBase.GetComponentsInChildren<LayoutGroup>(true))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
        }

        if (isProduct)
        {
            SetPanelPosByProduct(pos);
        }
        else
        {
            SetPanelPos(pos);
        }

        // �г� Ȱ��ȭ
        panel_HoverLootBase.SetActive(true);

        // ���� ����
        Canvas.ForceUpdateCanvases();
        var rect = panel_HoverLootBase.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent<LayoutGroup>(out var layout))
            {
                var rect1 = t.GetComponent<RectTransform>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect1);
            }
        }
    }
    void SetPanelPos(Vector3 pos)
    {
        RectTransform parentRect = panel_HoverLootBase.transform.parent as RectTransform;

        // ��ũ�� ��ǥ �� �θ� ������ ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            pos,
            Camera.main,
            out Vector2 localPos);

        // ������ ���ؼ� ���� anchoredPosition ����
        panel_HoverLootBase.GetComponent<RectTransform>().anchoredPosition = localPos + basicPanelPos;
    }
    void SetPanelPosByProduct(Vector3 pos)
    {
        Vector3 screenPos = Vector3.zero;
        if (pos.x >= 0)
        {
            screenPos = Camera.main.WorldToScreenPoint(pos + productLeftPanelPos);
        }
        else
        {
            screenPos = Camera.main.WorldToScreenPoint(pos + productRightPanelPos);
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel_HoverLootBase.transform.parent as RectTransform,
            screenPos,
            Camera.main,
            out Vector2 localPos);

        panel_HoverLootBase.GetComponent<RectTransform>().anchoredPosition = localPos;
    }
    string BoldingText(string text)
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
}
