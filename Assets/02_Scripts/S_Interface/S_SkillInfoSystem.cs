using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_SkillInfoSystem : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject prefab_SkillObject;

    [Header("������Ʈ")]
    GameObject panel_SkillInfoBase;
    GameObject layoutGroup_SkillInfoBase;
    TMP_Text text_TotalSkillCount;

    [Header("���� ����")]
    Vector2 hidePos = new Vector2(-5, -110);
    Vector2 originPos = new Vector2(-5, 55);

    [Header("������ �ɷ� ����Ʈ")]
    List<GameObject> ownedSkillList = new();

    // �̱���
    static S_SkillInfoSystem instance;
    public static S_SkillInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // ������Ʈ �Ҵ�
        panel_SkillInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_SkillInfoBase")).gameObject;
        layoutGroup_SkillInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_SkillInfoBase")).gameObject;
        text_TotalSkillCount = Array.Find(texts, c => c.gameObject.name.Equals("Text_TotalSkillCount"));

        // �̱���
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
    public void InitPos() // �г� ��ġ �ʱ�ȭ
    {
        panel_SkillInfoBase.GetComponent<RectTransform>().anchoredPosition = hidePos;
        panel_SkillInfoBase.SetActive(false);
    }
    public void AppearSkill() // �г� ����
    {
        // �г� ��ġ �ʱ�ȭ
        panel_SkillInfoBase.SetActive(true);

        // �г� ���� ��Ʈ��
        panel_SkillInfoBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_SkillInfoBase.GetComponent<RectTransform>().DOAnchorPos(originPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearSkill() // �г� ����
    {
        // �г� ���� ��Ʈ��
        panel_SkillInfoBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_SkillInfoBase.GetComponent<RectTransform>().DOAnchorPos(hidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_SkillInfoBase.SetActive(false));
    }
    public void AddSkillObject(S_Skill loot) // ����ǰ �߰� �� 
    {
        GameObject go = Instantiate(prefab_SkillObject);

        go.transform.SetParent(layoutGroup_SkillInfoBase.transform, false);
        go.GetComponent<S_SkillObject>().SetSkillObjectInfo(loot);
        ownedSkillList.Add(go);

        UpdateTotalSkillCount();
    }
    public void RemoveSkillObject(S_Skill skill) // ����ǰ ���� ��
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
    public void UpdateSkillObject() // ����ǰ �ؽ�Ʈ �� ActivatedCount ������Ʈ
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
