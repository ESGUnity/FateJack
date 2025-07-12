using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_StatInfoSystem : MonoBehaviour
{
    [Header("컴포넌트")]
    TMP_Text text_Health;
    TMP_Text text_CurrentLimit;
    TMP_Text text_CurrentTrial;
    Image image_WeightBar;
    TMP_Text text_WeightValue;

    TMP_Text text_CurrentStrength;
    TMP_Text text_CurrentMind;
    TMP_Text text_CurrentLuck;

    GameObject image_BurstStateBase;
    GameObject image_PerfectStateBase;
    GameObject image_DelusionStateBase;
    TMP_Text text_DelusionState;
    GameObject image_FirstStateBase;
    TMP_Text text_FirstState;
    GameObject image_ExpansionStateBase;
    TMP_Text text_ExpansionState;
    GameObject image_ColdBloodStateBase;
    TMP_Text text_ColdBloodState;

    GameObject image_StateHoverArea;

    [Header("UI")]
    Color weightBarOriginColor = new Color(0.34f, 0.72f, 0.26f, 1f);
    Color weightBarBurstColor = new Color(0.66f, 0.15f, 0.23f, 1f);
    Color weightBarPerfectColor = new Color(0.05f, 0.53f, 0.82f, 1f);

    List<S_GameFlowStateEnum> VALID_STATES;
    bool isEnter = false;

    // 싱글턴
    static S_StatInfoSystem instance;
    public static S_StatInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // 할당
        text_Health = Array.Find(texts, c => c.gameObject.name.Equals("Text_Health"));
        text_CurrentLimit = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentLimit"));
        text_CurrentTrial = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentTrial"));
        image_WeightBar = Array.Find(images, c => c.gameObject.name.Equals("Image_WeightBar"));
        text_WeightValue = Array.Find(texts, c => c.gameObject.name.Equals("Text_WeightValue"));

        text_CurrentStrength = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentStrength"));
        text_CurrentMind = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentMind"));
        text_CurrentLuck = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentLuck"));

        image_BurstStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BurstStateBase")).gameObject;
        image_PerfectStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_PerfectStateBase")).gameObject;

        image_DelusionStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_DelusionStateBase")).gameObject;
        text_DelusionState = Array.Find(texts, c => c.gameObject.name.Equals("Text_DelusionState"));

        image_FirstStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_FirstStateBase")).gameObject;
        text_FirstState = Array.Find(texts, c => c.gameObject.name.Equals("Text_FirstState"));

        image_ExpansionStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ExpansionStateBase")).gameObject;
        text_ExpansionState = Array.Find(texts, c => c.gameObject.name.Equals("Text_ExpansionState"));

        image_ColdBloodStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ColdBloodStateBase")).gameObject;
        text_ColdBloodState = Array.Find(texts, c => c.gameObject.name.Equals("Text_ColdBloodState"));

        image_StateHoverArea = Array.Find(transforms, c => c.gameObject.name.Equals("Image_StateHoverArea")).gameObject;

        // 섹션의 스프라이트에 포인터 엔터 바인딩하기.
        EventTrigger spriteTrigger = image_StateHoverArea.GetComponent<EventTrigger>();
        EventTrigger.Entry spritePointerEnterEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);
        EventTrigger.Entry spritePointerExitEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerExit);
        // 바인딩
        spritePointerEnterEntry.callback.AddListener((eventData) => { PointerEnterStates((PointerEventData)eventData); });
        spritePointerExitEntry.callback.AddListener((eventData) => { PointerExitStates((PointerEventData)eventData); });

        // 할당
        VALID_STATES = new() { S_GameFlowStateEnum.Hit, S_GameFlowStateEnum.Deck };

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
        // Enter 상태인데 상태가 유효하지 않게 바뀌면 강제로 Exit
        if (isEnter && !S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            ForceExit();
        }
    }
    void OnDisable()
    {
        ForceExit();
    }
    void Start()
    {
        InitText();
    }

    public void InitText()
    {
        text_Health.text = $"체력<sprite name=Health> : {S_PlayerStat.Instance.GetCurrentHealth()} / {S_PlayerStat.Instance.MaxHealth}";
        text_CurrentLimit.text = $"한계<sprite name=Limit> : {S_PlayerStat.Instance.CurrentLimit.ToString()}";
        text_CurrentTrial.text = $"시련 : {S_GameFlowManager.Instance.CurrentTrial.ToString()} / 21";
        image_WeightBar.DOFillAmount(0, 0);
        text_WeightValue.text = $"{S_PlayerStat.Instance.CurrentWeight} / {S_PlayerStat.Instance.CurrentLimit}";

        text_CurrentStrength.text = S_PlayerStat.Instance.CurrentStr.ToString();
        text_CurrentMind.text = S_PlayerStat.Instance.CurrentMind.ToString();
        text_CurrentLuck.text = S_PlayerStat.Instance.CurrentLuck.ToString();

        image_BurstStateBase.SetActive(false);
        image_PerfectStateBase.SetActive(false);
        image_DelusionStateBase.SetActive(false);
        image_FirstStateBase.SetActive(false);
    }
    public void ChangeCurrentTrialText()
    {
        text_CurrentTrial.text = $"시련 : {S_GameFlowManager.Instance.CurrentTrial.ToString()} / 21";
    }
    public void ChangeStatVFX() // 체력, 의지, 골드, 한계, 전투 능력치 값 VFX
    {
        // 기본 능력치
        text_Health.text = $"체력<sprite name=Health> : {S_PlayerStat.Instance.GetCurrentHealth()} / {S_PlayerStat.Instance.MaxHealth}";

        // 한계
        int limitValue = int.Parse(text_CurrentLimit.text.Split(':')[1].Trim());
        if (limitValue != S_PlayerStat.Instance.CurrentLimit)
        {
            ChangeStatVFXTween(limitValue, S_PlayerStat.Instance.CurrentLimit, text_CurrentLimit, "한계<sprite name=Limit>");
        }

        // 전투 능력치
        ChangeStatVFXTween(int.Parse(text_CurrentStrength.text), S_PlayerStat.Instance.CurrentStr, text_CurrentStrength);
        ChangeStatVFXTween(int.Parse(text_CurrentMind.text), S_PlayerStat.Instance.CurrentMind, text_CurrentMind);
        ChangeStatVFXTween(int.Parse(text_CurrentLuck.text), S_PlayerStat.Instance.CurrentLuck, text_CurrentLuck);

        // 무게
        ChangeWeightValueVFX();
    }
    public void ChangeWeightValueVFX()
    {
        if (S_PlayerStat.Instance.CurrentWeight != int.Parse(text_WeightValue.text.Split('/')[0].Trim()))
        {
            image_WeightBar.DOFillAmount((float)S_PlayerStat.Instance.CurrentWeight / S_PlayerStat.Instance.CurrentLimit, S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4).SetEase(Ease.OutQuart);
            ChangeWeightValueVFXTween(int.Parse(text_WeightValue.text.Split('/')[0].Trim()), S_PlayerStat.Instance.CurrentWeight, text_WeightValue);
        }
        if (S_PlayerStat.Instance.CurrentLimit != int.Parse(text_WeightValue.text.Split('/')[1].Trim()))
        {
            image_WeightBar.DOFillAmount((float)S_PlayerStat.Instance.CurrentWeight / S_PlayerStat.Instance.CurrentLimit, S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4).SetEase(Ease.OutQuart);
            ChangeLimitValueVFXTween(int.Parse(text_WeightValue.text.Split('/')[1].Trim()), S_PlayerStat.Instance.CurrentLimit, text_WeightValue);
        }

        if (S_PlayerStat.Instance.CurrentWeight == S_PlayerStat.Instance.CurrentLimit)
        {
            image_WeightBar.color = weightBarPerfectColor;
        }
        else if (S_PlayerStat.Instance.CurrentWeight > S_PlayerStat.Instance.CurrentLimit)
        {
            image_WeightBar.color = weightBarBurstColor;
        }
        else
        {
            image_WeightBar.color = weightBarOriginColor;
        }
    }
    void ChangeWeightValueVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = $"{currentNumber} / {S_PlayerStat.Instance.CurrentLimit}"; },
                newValue,
                S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4
            ).SetEase(Ease.OutQuart);
    }
    void ChangeLimitValueVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = $"{S_PlayerStat.Instance.CurrentWeight} / {currentNumber}"; },
                newValue,
                S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4
            ).SetEase(Ease.OutQuart);
    }
    void ChangeStatVFXTween(int oldValue, int newValue, TMP_Text statText, string label = "")
    {
        if (oldValue == newValue) return;

        statText.DOKill();
        int currentNumber = oldValue;
        float effectDuration = S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4;

        if (label == "")
        {
            DOTween.To
                (
                    () => currentNumber,
                    x => { currentNumber = x; statText.text = currentNumber.ToString(); },
                    newValue,
                    effectDuration
                ).SetEase(Ease.OutQuart);
        }
        else
        {
            DOTween.To
                (
                    () => currentNumber,
                    x =>
                    {
                        currentNumber = x;
                        statText.text = $"{label} : {currentNumber.ToString()}";
                    },
                    newValue,
                    effectDuration
                ).SetEase(Ease.OutQuart);
        }
    }
    public void UpdateSpecialAbility()  // 버스트, 클린히트, 망상, 우선, 전개
    {
        image_BurstStateBase.SetActive(S_PlayerStat.Instance.IsBurst);
        image_PerfectStateBase.SetActive(S_PlayerStat.Instance.IsPerfect);

        image_DelusionStateBase.SetActive(S_PlayerStat.Instance.IsDelusion > 0);
        text_DelusionState.text = S_PlayerStat.Instance.IsDelusion.ToString();

        image_ExpansionStateBase.SetActive(S_PlayerStat.Instance.IsExpansion > 0);
        text_ExpansionState.text = S_PlayerStat.Instance.IsExpansion.ToString();

        image_FirstStateBase.SetActive(S_PlayerStat.Instance.IsFirst > 0);
        text_FirstState.text = S_PlayerStat.Instance.IsFirst.ToString();

        image_ColdBloodStateBase.SetActive(S_PlayerStat.Instance.IsColdBlood > 0);
        text_ColdBloodState.text = S_PlayerStat.Instance.IsColdBlood.ToString();
    }

    public void PointerEnterStates(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            if (S_PlayerStat.Instance.IsBurst || S_PlayerStat.Instance.IsPerfect || S_PlayerStat.Instance.IsDelusion > 0 || S_PlayerStat.Instance.IsExpansion > 0 || S_PlayerStat.Instance.IsFirst > 0 || S_PlayerStat.Instance.IsColdBlood > 0)
            {
                S_HoverInfoSystem.Instance.ActivateHoverInfoByStates();
            }

            isEnter = true;
        }
    }
    public void PointerExitStates(PointerEventData eventData)
    {
        ForceExit();
    }
    void ForceExit()
    {
        if (!isEnter) return;

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();

        isEnter = false;
    }
}
