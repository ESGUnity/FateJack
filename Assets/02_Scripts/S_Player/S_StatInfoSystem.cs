using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_StatInfoSystem : MonoBehaviour
{
    [Header("컴포넌트")]
    TMP_Text text_Health;
    TMP_Text text_Determination;
    TMP_Text text_Gold;
    TMP_Text text_CurrentLimit;
    TMP_Text text_CurrentTrial;
    Image image_StackSumBar;
    TMP_Text text_StackSumValue;

    GameObject panel_BattleStatInfoBase;
    TMP_Text text_CurrentStrength;
    TMP_Text text_CurrentMind;
    TMP_Text text_CurrentLuck;

    GameObject image_BurstStateBase;
    GameObject image_PerfectStateBase;
    GameObject image_DelusionStateBase;
    GameObject image_FirstStateBase;
    TMP_Text text_FirstState;
    GameObject image_ExpansionStateBase;
    TMP_Text text_ExpansionState;
    GameObject image_ColdBloodStateBase;

    [Header("UI")]
    Vector2 basicStatHidePos = new Vector2(0, -100);
    Vector2 basicStatOriginPos = new Vector2(0, 0);
    Vector2 battleStatHidePos = new Vector2(5, -270);
    Vector2 battleStatOriginPos = new Vector2(5, 55);
    Color stackSumOriginColor = new Color(0f, 0.57f, 0.68f, 1f);
    Color stackSumBurstColor = new Color(0.68f, 0.2f, 0f, 1f);
    Color stackSumCleanHitColor = new Color(0f, 0.68f, 0.2f, 1f);

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
        text_Determination = Array.Find(texts, c => c.gameObject.name.Equals("Text_Determination"));
        text_Gold = Array.Find(texts, c => c.gameObject.name.Equals("Text_Gold"));
        text_CurrentLimit = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentLimit"));
        text_CurrentTrial = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentTrial"));
        image_StackSumBar = Array.Find(images, c => c.gameObject.name.Equals("Image_StackSumBar"));
        text_StackSumValue = Array.Find(texts, c => c.gameObject.name.Equals("Text_StackSumValue"));

        panel_BattleStatInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_BattleStatInfoBase")).gameObject;
        text_CurrentStrength = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentStrength"));
        text_CurrentMind = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentMind"));
        text_CurrentLuck = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentLuck"));

        image_BurstStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BurstStateBase")).gameObject;
        image_PerfectStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_PerfectStateBase")).gameObject;
        image_DelusionStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_DelusionStateBase")).gameObject;
        image_FirstStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_FirstStateBase")).gameObject;
        text_FirstState = Array.Find(texts, c => c.gameObject.name.Equals("Text_FirstState"));
        image_ExpansionStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ExpansionStateBase")).gameObject;
        text_ExpansionState = Array.Find(texts, c => c.gameObject.name.Equals("Text_ExpansionState"));
        image_ColdBloodStateBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ColdBloodStateBase")).gameObject;

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
    void Start()
    {
        InitText();
    }

    public void InitText()
    {
        text_Health.text = $"체력 : {S_PlayerStat.Instance.GetCurrentHealth()} / {S_PlayerStat.Instance.MaxHealth}";
        text_Determination.text = $"의지 : {S_PlayerStat.Instance.GetCurrentDetermination()} / {S_PlayerStat.Instance.MaxDetermination}";
        text_Gold.text = $"골드 : {S_PlayerStat.Instance.CurrentGold.ToString()}";
        text_CurrentLimit.text = $"한계 : {S_PlayerStat.Instance.CurrentLimit.ToString()}";
        text_CurrentTrial.text = $"시련 : {S_GameFlowManager.Instance.CurrentTrial.ToString()}";
        image_StackSumBar.DOFillAmount(0, 0);
        text_StackSumValue.text = $"{S_PlayerStat.Instance.CurrentWeight} / {S_PlayerStat.Instance.CurrentLimit}";

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
        text_CurrentTrial.text = $"시련 : {S_GameFlowManager.Instance.CurrentTrial.ToString()}";
    }
    public void ChangeStatVFX() // 체력, 의지, 골드, 한계, 전투 능력치 값 VFX
    {
        // 기본 능력치
        text_Health.text = $"체력 : {S_PlayerStat.Instance.GetCurrentHealth()} / {S_PlayerStat.Instance.MaxHealth}";
        text_Determination.text = $"의지 : {S_PlayerStat.Instance.GetCurrentDetermination()} / {S_PlayerStat.Instance.MaxDetermination}";

        // 골드
        int goldValue = int.Parse(text_Gold.text.Split(':')[1].Trim());
        if (goldValue != S_PlayerStat.Instance.CurrentGold) 
        {
            ChangeStatVFXTween(goldValue, S_PlayerStat.Instance.CurrentGold, text_Gold, "골드");
        }

        // 한계
        int limitValue = int.Parse(text_CurrentLimit.text.Split(':')[1].Trim());
        if (limitValue != S_PlayerStat.Instance.CurrentLimit)
        {
            ChangeStatVFXTween(goldValue, S_PlayerStat.Instance.CurrentLimit, text_CurrentLimit, "한계");
        }

        // 전투 능력치
        ChangeStatVFXTween(int.Parse(text_CurrentStrength.text), S_PlayerStat.Instance.CurrentStr, text_CurrentStrength);
        ChangeStatVFXTween(int.Parse(text_CurrentMind.text), S_PlayerStat.Instance.CurrentMind, text_CurrentMind);
        ChangeStatVFXTween(int.Parse(text_CurrentLuck.text), S_PlayerStat.Instance.CurrentLuck, text_CurrentLuck);

        // 무게 합
        ChangeWeightValueVFX();
    }
    public void ChangeWeightValueVFX()
    {
        if (S_PlayerStat.Instance.CurrentWeight != int.Parse(text_StackSumValue.text.Split('/')[0].Trim()))
        {
            image_StackSumBar.DOFillAmount((float)S_PlayerStat.Instance.CurrentWeight / S_PlayerStat.Instance.CurrentLimit, S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4).SetEase(Ease.OutQuart);
            ChangeWeightValueVFXTween(int.Parse(text_StackSumValue.text.Split('/')[0].Trim()), S_PlayerStat.Instance.CurrentWeight, text_StackSumValue);
        }
        if (S_PlayerStat.Instance.CurrentLimit != int.Parse(text_StackSumValue.text.Split('/')[1].Trim()))
        {
            image_StackSumBar.DOFillAmount((float)S_PlayerStat.Instance.CurrentWeight / S_PlayerStat.Instance.CurrentLimit, S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4).SetEase(Ease.OutQuart);
            ChangeLimitValueVFXTween(int.Parse(text_StackSumValue.text.Split('/')[1].Trim()), S_PlayerStat.Instance.CurrentLimit, text_StackSumValue);
        }

        if (S_PlayerStat.Instance.CurrentWeight == S_PlayerStat.Instance.CurrentLimit)
        {
            image_StackSumBar.color = stackSumCleanHitColor;
        }
        else if (S_PlayerStat.Instance.CurrentWeight > S_PlayerStat.Instance.CurrentLimit)
        {
            image_StackSumBar.color = stackSumBurstColor;
        }
        else
        {
            image_StackSumBar.color = stackSumOriginColor;
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
        image_DelusionStateBase.SetActive(S_PlayerStat.Instance.IsDelusion);
        image_ExpansionStateBase.SetActive(S_PlayerStat.Instance.IsExpansion);
        image_FirstStateBase.SetActive(S_PlayerStat.Instance.IsFirst);
        image_ColdBloodStateBase.SetActive(S_PlayerStat.Instance.IsColdBlood);
    }
}
