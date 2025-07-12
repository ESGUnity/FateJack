using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_HoverInfoSystem : MonoBehaviour
{
    [Header("컴포넌트")]
    GameObject panel_HoverInfoBase { get; set; }
    GameObject layoutGroup_HoverInfoBase;
    GameObject layoutGroup_AdditiveDescriptionBase;

    [Header("UI")]
    Vector2 CARD_X_OFFSET_POS = new Vector2(60, 0);
    Vector2 CARD_OFFSET_POS = new Vector2(0, 120);
    Vector2 CHARACTER_OFFSET_POS = new Vector2(0, 55);
    Vector2 STACK_TRINKET_OFFSET_POS = new Vector2(0, 100);
    Vector2 STATES_HOVER_POS = new Vector2(0, -390);

    [Header("설명 패널 오브젝트 리스트")]
    List<GameObject> hoverInfoBaseObjects = new();
    List<GameObject> additiveObjects = new();

    [Header("포인터 연출")]
    bool isEnter = false;
    List<S_GameFlowStateEnum> VALID_STATES;

    [HideInInspector] public Dictionary<string, string> AdditiveTargetTermDictionary = new()
    {
        { "완벽", "Perfect" },
        { "버스트", "Burst" },
        { "저주받은 카드", "CursedCard" },
        { "저주", "Curse" },
        { "망상", "Delusion" },
        { "전개", "Expansion" },
        { "우선", "First" },
        { "냉혈", "ColdBlood" },
        { "지속", "Persist" },
        { "발현", "Unleash" },
    };

    // 싱글턴
    static S_HoverInfoSystem instance;
    public static S_HoverInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // 컴포넌트 할당
        panel_HoverInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_HoverInfoBase")).gameObject;
        layoutGroup_HoverInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_HoverInfoBase")).gameObject;
        layoutGroup_AdditiveDescriptionBase = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_AdditiveDescriptionBase")).gameObject;

        // 계산하기 쉽도록 오브젝트를 리스트화 하기
        for (int i = 0; i < layoutGroup_HoverInfoBase.transform.childCount; i++)
        {
            hoverInfoBaseObjects.Add(layoutGroup_HoverInfoBase.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < layoutGroup_AdditiveDescriptionBase.transform.childCount; i++)
        {
            additiveObjects.Add(layoutGroup_AdditiveDescriptionBase.transform.GetChild(i).gameObject);
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
    S_GameFlowStateEnum prevState;
    void Update()
    {
        // 게임 플로우가 바뀔 때 None이라면 자동으로 비활성화되는 기능
        S_GameFlowStateEnum currentState = S_GameFlowManager.Instance.GameFlowState;
        if (prevState != currentState)
        {
            if (currentState == S_GameFlowStateEnum.None)
            {
                DeactiveHoverInfo();
            }

            prevState = currentState;
        }
    }

    #region 카드
    public void ActivateHoverInfoByCard(S_CardBase card, GameObject obj)
    {
        FillHoverInfo(card);
        SetPosByCardObj(obj);

        // 패널 활성화
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel_HoverInfoBase.GetComponent<RectTransform>());
        panel_HoverInfoBase.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel_HoverInfoBase.GetComponent<RectTransform>());
    }
    void FillHoverInfo(S_CardBase card) // 카드 전용
    {
        int hoverInfoCount = 0;
        int additiveDescriptionCount = 0;

        // 이름
        hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
        hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetCardEffectName(card);
        hoverInfoCount++;
        // 효과
        hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
        hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetCardEffectDescription(card);
        hoverInfoCount++;

        // 추가 설명 영역
        List<string> termList = new();
        if (card.IsCursed) // 저주받은 카드
        {
            if (!termList.Contains("CursedCard"))
            {
                termList.Add("CursedCard");
            }
        }
        foreach (S_EngravingEnum engraving in card.OriginEngraving) // 각인류
        {
            if (!termList.Contains(engraving.ToString()))
            {
                termList.Add(engraving.ToString());
            }
        }
        foreach (string keyword in AdditiveTargetTermDictionary.Keys) // 설명에 포함된 단어
        {
            if (S_CardMetadata.CardDatas[card.Key].Description_Korean.Contains(keyword))
            {
                if (!termList.Contains(AdditiveTargetTermDictionary[keyword]))
                {
                    termList.Add(AdditiveTargetTermDictionary[keyword]);
                }
            }
        }
        foreach (S_EngravingEnum engraving in card.Engraving) // 각인류
        {
            if (!termList.Contains(engraving.ToString()))
            {
                termList.Add(engraving.ToString());
            }
        }

        // 추가 설명 달기
        foreach (string s in termList)
        {
            if (!S_CardMetadata.TermDatas.ContainsKey(s)) continue;

            additiveObjects[additiveDescriptionCount].SetActive(true);
            additiveObjects[additiveDescriptionCount].GetComponent<S_AdditiveDescription>().SetAdditiveDescription(s);
            additiveDescriptionCount++;
        }

        layoutGroup_HoverInfoBase.SetActive(true);

        if (additiveDescriptionCount > 0)
        {
            layoutGroup_AdditiveDescriptionBase.SetActive(true);
        }
    }
    void SetPosByCardObj(GameObject obj) // 단순히 3D 오브젝트의 좌표를 UI 좌표로 옮긴 것. 오브젝트의 가장자리 기준으로 x좌표가 0보다 크면 카드 왼쪽에, 작으면 오른쪽의 좌표를 반환
    {
        Vector3 worldPos;
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        RectTransform panelRect = panel_HoverInfoBase.GetComponent<RectTransform>();

        if (obj.transform.position.x >= 0)
        {
            // 카드의 왼쪽에 표시하는 경우
            worldPos = new Vector3(bounds.min.x, bounds.max.y, bounds.center.z);
            panelRect.pivot = new Vector2(1f, 1f);
        }
        else
        {
            // 카드의 오른쪽에 표시하는 경우
            worldPos = new Vector3(bounds.max.x, bounds.max.y, bounds.center.z);
            panelRect.pivot = new Vector2(0f, 1f);
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            Camera.main,
            out Vector2 localPos);

        // 왼쪽과 오른쪽에 따라 오프셋 더하거나 빼기
        localPos = obj.transform.position.x >= 0 ? localPos - CARD_X_OFFSET_POS : localPos + CARD_X_OFFSET_POS;
        localPos += CARD_OFFSET_POS;
        panelRect.anchoredPosition = localPos;

        // 추가 설명 패널 위치도 설정(기본 설명 패널이 세팅이 완료된 후 해야 버그가 없다.)
        RectTransform lrt = layoutGroup_AdditiveDescriptionBase.GetComponent<RectTransform>();
        if (obj.transform.position.x >= 0)
        {
            lrt.anchorMin = new Vector2(0f, 1f);
            lrt.anchorMax = new Vector2(0f, 1f);
            lrt.pivot = new Vector2(1f, 1f);
            layoutGroup_AdditiveDescriptionBase.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperRight;
            lrt.anchoredPosition = new Vector2(-10f, 0f);
        }
        else
        {
            lrt.anchorMin = new Vector2(1f, 1f);
            lrt.anchorMax = new Vector2(1f, 1f);
            lrt.pivot = new Vector2(0f, 1f);
            layoutGroup_AdditiveDescriptionBase.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
            lrt.anchoredPosition = new Vector2(10f, 0f);
        }
    }
    #endregion
    #region 상태
    public void ActivateHoverInfoByStates()
    {
        int hoverInfoCount = 0;

        if (S_PlayerStat.Instance.IsBurst)
        {
            hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
            hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetAdditiveDescription("Burst");
            hoverInfoCount++;
        }
        if (S_PlayerStat.Instance.IsPerfect)
        {
            hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
            hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetAdditiveDescription("Perfect");
            hoverInfoCount++;
        }
        if (S_PlayerStat.Instance.IsDelusion > 0)
        {
            hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
            hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetAdditiveDescription("Delusion");
            hoverInfoCount++;
        }
        if (S_PlayerStat.Instance.IsExpansion > 0)
        {
            hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
            hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetAdditiveDescription("Expansion");
            hoverInfoCount++;
        }
        if (S_PlayerStat.Instance.IsFirst > 0)
        {
            hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
            hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetAdditiveDescription("First");
            hoverInfoCount++;
        }
        if (S_PlayerStat.Instance.IsColdBlood > 0)
        {
            hoverInfoBaseObjects[hoverInfoCount].SetActive(true);
            hoverInfoBaseObjects[hoverInfoCount].GetComponent<S_AdditiveDescription>().SetAdditiveDescription("ColdBlood");
            hoverInfoCount++;
        }
        layoutGroup_HoverInfoBase.SetActive(true);

        RectTransform panelRect = panel_HoverInfoBase.GetComponent<RectTransform>();

        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = STATES_HOVER_POS;

        // 패널 활성화
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel_HoverInfoBase.GetComponent<RectTransform>());
        panel_HoverInfoBase.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel_HoverInfoBase.GetComponent<RectTransform>());
    }
    #endregion
    public void DeactiveHoverInfo()
    {
        if (panel_HoverInfoBase.activeInHierarchy)
        {
            panel_HoverInfoBase.SetActive(false);
        }

        layoutGroup_HoverInfoBase.SetActive(false);
        layoutGroup_AdditiveDescriptionBase.SetActive(false);

        foreach (GameObject go in hoverInfoBaseObjects)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in additiveObjects)
        {
            go.SetActive(false);
        }
    }
    #region 다이얼로그
    public void SetPosByCharacterOrFoe(Renderer renderer, RectTransform targetRect, RectTransform targetCanvasRect)
    {
        Bounds bounds = renderer.bounds;

        Vector3 worldPos = new Vector3(bounds.max.x, bounds.max.y, bounds.center.z);
        targetRect.pivot = new Vector2(0f, 1f);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetCanvasRect,
            screenPos,
            Camera.main,
            out Vector2 localPos);

        // 오프셋 먹이기
        localPos += CHARACTER_OFFSET_POS;
        targetRect.anchoredPosition = localPos;
    }
    #endregion
}

























//public void ActivateHoverInfo(S_Card card, RectTransform rect) // UI 카드 호버링 시 호출(S_StackCard, S_DeckCard, S_StoreCard 등)
//{
//    FillHoverInfo(card);
//    SetPosByRectCard(rect);

//    // 패널 활성화
//    var panelRect = panel_HoverInfoBase.GetComponent<RectTransform>();
//    LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
//    panel_HoverInfoBase.SetActive(true);
//    LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
//}
//void SetPosByRectCard(RectTransform rect)
//{
//    RectTransform panelRect = panel_HoverInfoBase.GetComponent<RectTransform>();
//    RectTransform canvasRect = GetComponent<RectTransform>();

//    // 1. 우측 상단 corner 구하기 (GetWorldCorners 결과: 0=좌하, 1=좌상, 2=우상, 3=우하)
//    Vector3[] worldCorners = new Vector3[4];
//    rect.GetWorldCorners(worldCorners);
//    Vector3 topRightWorldPos;
//    if (rect.transform.position.x >= 0)
//    {
//        // 카드의 왼쪽에 표시하는 경우
//        panelRect.pivot = new Vector2(1f, 1f);
//        topRightWorldPos = worldCorners[1];
//    }
//    else
//    {
//        // 카드의 오른쪽에 표시하는 경우
//        panelRect.pivot = new Vector2(0f, 1f);
//        topRightWorldPos = worldCorners[2];
//    }

//    // 2. 우상단 월드 좌표를 "스크린 좌표"로 변환
//    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, topRightWorldPos);

//    // 3. HoverInfo Canvas의 local position으로 변환
//    RectTransformUtility.ScreenPointToLocalPointInRectangle(
//        canvasRect,
//        screenPos,
//        Camera.main,
//        out Vector2 localPos
//    );

//    // 4. 해당 위치에 HoverInfo UI 배치
//    localPos = rect.transform.position.x >= 0 ? localPos - offsetXValue : localPos + offsetXValue;
//    panelRect.anchoredPosition = localPos;

//    // 추가 설명 패널 위치도 설정
//    RectTransform lrt = layoutGroup_AdditiveDescriptionBase.GetComponent<RectTransform>();
//    if (rect.transform.position.x >= 0)
//    {
//        lrt.anchorMin = new Vector2(0f, 1f);
//        lrt.anchorMax = new Vector2(0f, 1f);
//        lrt.pivot = new Vector2(1f, 1f);
//        layoutGroup_AdditiveDescriptionBase.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperRight;
//        lrt.anchoredPosition = new Vector2(-10f, 0f);
//    }
//    else
//    {
//        lrt.anchorMin = new Vector2(1f, 1f);
//        lrt.anchorMax = new Vector2(1f, 1f);
//        lrt.pivot = new Vector2(0f, 1f);
//        layoutGroup_AdditiveDescriptionBase.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
//        lrt.anchoredPosition = new Vector2(10f, 0f);
//    }
//}


//public void ActivateHoverInfo(S_Trinket skill, RectTransform rect) // 월드 오브젝트 카드 호버링 시 호출(S_StackCard, S_DeckCard, S_StoreCard 등)
//{
//    // 튜토리얼 때문에
//    if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Dialog)
//    {
//        GetComponent<Canvas>().sortingLayerName = "Dialog";
//        GetComponent<Canvas>().sortingOrder = 2;
//    }
//    else
//    {
//        GetComponent<Canvas>().sortingLayerName = "UI";
//        GetComponent<Canvas>().sortingOrder = 2;
//    }

//    FillHoverInfo(skill);
//    SetPosByRectSkill(rect);

//    // 패널 활성화
//    var panelRect = panel_HoverInfoBase.GetComponent<RectTransform>();
//    LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
//    panel_HoverInfoBase.SetActive(true);
//    LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
//}
//void SetPosByRectSkill(RectTransform rect)
//{
//    RectTransform panelRect = panel_HoverInfoBase.GetComponent<RectTransform>();
//    RectTransform canvasRect = GetComponent<RectTransform>();

//    // 피벗 설정
//    panelRect.pivot = new Vector2(1f, 0f);

//    // 1. 우측 상단 corner 구하기 (GetWorldCorners 결과: 0=좌하, 1=좌상, 2=우상, 3=우하)
//    Vector3[] worldCorners = new Vector3[4];
//    rect.GetWorldCorners(worldCorners);
//    Vector3 topRightWorldPos = worldCorners[2]; // 우상단

//    // 2. 우상단 월드 좌표를 "스크린 좌표"로 변환
//    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, topRightWorldPos);

//    // 3. HoverInfo Canvas의 local position으로 변환
//    RectTransformUtility.ScreenPointToLocalPointInRectangle(
//        canvasRect,
//        screenPos,
//        Camera.main,
//        out Vector2 localPos
//    );

//    // 4. 해당 위치에 HoverInfo UI 배치
//    localPos += offsetYValue;
//    panelRect.anchoredPosition = localPos;
//}


