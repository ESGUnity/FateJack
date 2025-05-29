using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

// TODO : 성장형 커서 대면 정보 나오기
public class S_HoverSkillSystem : MonoBehaviour
{
    // 컴포넌트
    GameObject panel_HoverLootBase;
    TMP_Text text_LootName;
    TMP_Text text_LootCondition;
    TMP_Text text_LootDescription;

    // 카드 정보 패널 위치
    Vector2 basicPanelPos = new Vector3(-80, 50);
    Vector3 productRightPanelPos = new Vector3(1.95f, -1.2f, 0);
    Vector3 productLeftPanelPos = new Vector3(-1.95f, -1.2f, 0);

    // 추가 설명 패널
    List<GameObject> additivePanel = new();

    // 게임플로우 상태 검사용
    S_GameFlowStateEnum prevState;

    // 용어 
    HashSet<string> conditionWords = new HashSet<string> { "잔류", "발현", "예견", "범람", "메아리", "대미장식", "이탈", "스탠드" };
    HashSet<string> typeWords = new HashSet<string> { "영원", "추가", "배율", "조작", "저항", "피해", "회복", "단련", "약탈", "창조", "역류", "인도", "환상", "제외", "저주", "저주해제", "우선", "망상", "소멸" };

    // 싱글턴
    static S_HoverSkillSystem instance;
    public static S_HoverSkillSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // 컴포넌트 할당
        panel_HoverLootBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_HoverLootBase")).gameObject;
        text_LootName = Array.Find(texts, c => c.gameObject.name.Equals("Text_LootName"));
        text_LootCondition = Array.Find(texts, c => c.gameObject.name.Equals("Text_LootCondition"));
        text_LootDescription = Array.Find(texts, c => c.gameObject.name.Equals("Text_LootDescription"));

        Transform parent = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_AdditiveDescriptionBase"));
        for (int i = 0; i < parent.childCount; i++)
        {
            additivePanel.Add(parent.GetChild(i).gameObject);
        }

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
    void Update()
    {
        // 게임 플로우가 바뀔 때 None이라면 자동으로 비활성화되는 기능
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
        // 카드 효과 설정
        text_LootName.text = BoldingText(loot.Name);
        text_LootCondition.text = BoldingText(loot.ConditionText);

        StringBuilder sb = new();
        sb.Append(loot.Description);
        if (loot.Condition == S_ActivationConditionEnum.Loot_Growth)
        {
            sb.Append($"\n현재 단위 : {loot.ActivatedCount}");
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

        // 패널 활성화
        panel_HoverLootBase.SetActive(true);

        // 강제 정렬
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

        // 스크린 좌표 → 부모 기준의 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            pos,
            Camera.main,
            out Vector2 localPos);

        // 오프셋 더해서 최종 anchoredPosition 설정
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
