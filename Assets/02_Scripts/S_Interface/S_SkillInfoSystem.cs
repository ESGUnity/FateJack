using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_SkillInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_SkillObject;

    [Header("컴포넌트")]
    GameObject panel_SkillInfoBase;
    GameObject layoutGroup_SkillInfoBase;
    TMP_Text text_TotalSkillCount;

    [Header("연출 관련")]
    Vector2 hidePos = new Vector2(-5, -110);
    Vector2 originPos = new Vector2(-5, 55);

    [Header("소유한 능력 리스트")]
    List<GameObject> ownedSkillList = new();

    // 싱글턴
    static S_SkillInfoSystem instance;
    public static S_SkillInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // 컴포넌트 할당
        panel_SkillInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_SkillInfoBase")).gameObject;
        layoutGroup_SkillInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_SkillInfoBase")).gameObject;
        text_TotalSkillCount = Array.Find(texts, c => c.gameObject.name.Equals("Text_TotalSkillCount"));

        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitPos();
    }
    public void InitPos() // 패널 위치 초기화
    {
        panel_SkillInfoBase.GetComponent<RectTransform>().anchoredPosition = hidePos;
        panel_SkillInfoBase.SetActive(false);
    }
    public void AppearSkill() // 패널 등장
    {
        // 패널 위치 초기화
        panel_SkillInfoBase.SetActive(true);

        // 패널 등장 두트윈
        panel_SkillInfoBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_SkillInfoBase.GetComponent<RectTransform>().DOAnchorPos(originPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearSkill() // 패널 퇴장
    {
        // 패널 퇴장 두트윈
        panel_SkillInfoBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_SkillInfoBase.GetComponent<RectTransform>().DOAnchorPos(hidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_SkillInfoBase.SetActive(false));
    }
    public void AddSkillObject(S_Skill loot) // 전리품 추가 시 
    {
        GameObject go = Instantiate(prefab_SkillObject);

        go.transform.SetParent(layoutGroup_SkillInfoBase.transform, false);
        go.GetComponent<S_SkillObject>().SetSkillObjectInfo(loot);
        ownedSkillList.Add(go);

        UpdateTotalSkillCount();
    }
    public void RemoveSkillObject(S_Skill skill) // 전리품 제외 시
    {
        GameObject skillGo = null;
        foreach (GameObject go in ownedSkillList)
        {
            if (go.GetComponent<S_SkillObject>().SkillInfo.Equals(skill))
            {
                skillGo = go;
                break;
            }
        }

        Destroy(skillGo);
        ownedSkillList.Remove(skillGo);

        UpdateTotalSkillCount();
    }
    public void UpdateSkillObject() // 전리품 텍스트 및 ActivatedCount 업데이트
    {
        foreach (GameObject go in ownedSkillList)
        {
            go.GetComponent<S_SkillObject>().UpdateSkillObject();
        }
    }
    void UpdateTotalSkillCount()
    {
        text_TotalSkillCount.text = $"{S_PlayerSkill.Instance.OwnedSkills.Count} / {S_PlayerSkill.MAX_LOOT}";
    }
    public void BouncingSkillObjectVFX(S_Skill skill)
    {
        foreach (GameObject go in ownedSkillList)
        {
            if (go.GetComponent<S_SkillObject>().SkillInfo == skill)
            {
                go.GetComponent<S_SkillObject>().BouncingVFX();
            }
        }
    }
}
