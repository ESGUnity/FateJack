using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_FoeInfoSystem : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public S_FoeInfo CurrentFoe;
    GameObject foeAttackVFX;

    [Header("씬 오브젝트")]
    [SerializeField] SpriteRenderer sprite_Foe;
    [SerializeField] SpriteRenderer sprite_FoeMeetConditionEffect;

    [Header("컴포넌트")]
    GameObject panel_FoeInfoBase;
    Image image_HealthBar;
    TMP_Text text_HealthValue;
    TMP_Text text_Count;

    GameObject image_NameAndAbilityBase;
    TMP_Text text_Name;
    TMP_Text text_Ability;

    [Header("UI")]
    Vector2 hidePos = new Vector2(0, 300);
    Vector2 originPos = new Vector2(0, 0);

    // 싱글턴
    static S_FoeInfoSystem instance;
    public static S_FoeInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        // 컴포넌트 할당
        panel_FoeInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_FoeInfoBase")).gameObject;
        image_HealthBar = Array.Find(images, c => c.gameObject.name.Equals("Image_HealthBar"));
        text_HealthValue = Array.Find(texts, c => c.gameObject.name.Equals("Text_HealthValue"));
        text_Count = Array.Find(texts, c => c.gameObject.name.Equals("Text_Count"));

        image_NameAndAbilityBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_NameAndAbilityBase")).gameObject;
        text_Name = Array.Find(texts, c => c.gameObject.name.Equals("Text_Name"));
        text_Ability = Array.Find(texts, c => c.gameObject.name.Equals("Text_Ability"));

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
    #region 초기화
    public void SetFoe(S_FoeInfo foeInfo)
    {
        // CurrentCreature에 세팅
        CurrentFoe = foeInfo;

        // 각종 텍스트 세팅
        image_HealthBar.fillAmount = 1f;
        text_HealthValue.text = $"{CurrentFoe.OldHealth} / {CurrentFoe.MaxHealth}";

        // 이름 설정
        text_Name.text = CurrentFoe.FoeInfo.Name;
        if (CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Clotho_Elite ||
            CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Lachesis_Elite ||
            CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Atropos_Elite)
        {
            text_Name.text = $"{text_Name.text}(엘리트)";
        }
        if (CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Clotho_Boss || CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Lachesis_Boss || CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Atropos_Boss)
        {
            text_Name.text = $"{text_Name.text}(보스)";
        }

        // 능력 설정
        text_Ability.text = CurrentFoe.FoeInfo.AbilityDescription;

        // ActivatedCount
        if (CurrentFoe.FoeInfo.Passive == S_FoePassiveEnum.NeedActivatedCount)
        {
            text_Count.gameObject.SetActive(true);
            text_Count.text = CurrentFoe.FoeInfo.ActivatedCount.ToString();
        }
        else
        {
            text_Count.gameObject.SetActive(false);
        }

        // 피조물 이미지 세팅
        var creatureImageOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{CurrentFoe.FoeInfo.Key}");
        creatureImageOpHandle.Completed += OnFoeSpriteLoadComplete;

        // 피조물의 공격 VFX 세팅
        var attackVFXOpHandle = Addressables.LoadAssetAsync<GameObject>($"Prefab_VFX_TempCommon"); // TODO : Prefab_VFX_{CurrentFoe.FoeInfo.FoeType.ToString()}
        attackVFXOpHandle.Completed += OnAttackVFXLoadComplete;
    }
    void OnFoeSpriteLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_Foe.sprite = opHandle.Result;
        }
    }
    void OnAttackVFXLoadComplete(AsyncOperationHandle<GameObject> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            foeAttackVFX = opHandle.Result;
        }
    }
    #endregion
    #region 연출
    public void InitPos()
    {
        panel_FoeInfoBase.GetComponent<RectTransform>().anchoredPosition = hidePos;
        image_NameAndAbilityBase.SetActive(false);
        panel_FoeInfoBase.SetActive(false);
    }
    public void AppearUIFoe() // 패널 등장
    {
        panel_FoeInfoBase.SetActive(true);
        panel_FoeInfoBase.GetComponent<RectTransform>().DOKill(); 
        panel_FoeInfoBase.GetComponent<RectTransform>().DOAnchorPos(originPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearUIFoe() // 패널 퇴장
    {
        panel_FoeInfoBase.GetComponent<RectTransform>().DOKill(); 
        panel_FoeInfoBase.GetComponent<RectTransform>().DOAnchorPos(hidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_FoeInfoBase.SetActive(false));
    }
    public void AppearFoeSprite()
    {
        sprite_Foe.gameObject.SetActive(true);
        sprite_Foe.GetComponent<SpriteRenderer>().DOKill(); // 두트윈 전 트윈 초기화
        sprite_Foe.DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearFoeSprite()
    {
        sprite_Foe.DOKill(); // 두트윈 전 트윈 초기화
        sprite_Foe.DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => 
            {
                sprite_Foe.gameObject.SetActive(false);
            });
    }
    public void PointerEnterInFoeSprite()
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit)
        {
            text_Ability.text = CurrentFoe.FoeInfo.GetDescription();
            image_NameAndAbilityBase.SetActive(true);
        }
    }
    public void PointerExitInFoeSprite()
    {
        image_NameAndAbilityBase.SetActive(false);
    }
    public void UpdateFoeObject() // 적 오브젝트 업데이트
    {
        if (CurrentFoe.FoeInfo.Passive == S_FoePassiveEnum.NeedActivatedCount)
        {
            ChangeCountVFXTween(int.Parse(text_Count.text), CurrentFoe.FoeInfo.ActivatedCount, text_Count);
        }
        else
        {
            text_Count.gameObject.SetActive(false);
        }

        if (CurrentFoe.FoeInfo.IsMeetCondition)
        {
            sprite_FoeMeetConditionEffect.gameObject.SetActive(true);
        }
        else
        {
            sprite_FoeMeetConditionEffect.gameObject.SetActive(false);
        }
    }
    #endregion
    #region VFX
    public void ChangeHealthValueVFX()
    {
        if (CurrentFoe == null) return;

        image_HealthBar.DOFillAmount((float)CurrentFoe.CurrentHealth / CurrentFoe.MaxHealth, S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4).SetEase(Ease.OutQuart);
        ChangeHealthValueVFXTween(int.Parse(text_HealthValue.text.Split('/')[0].Trim()), CurrentFoe.CurrentHealth, text_HealthValue);
    }
    void ChangeHealthValueVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = $"{currentNumber} / {CurrentFoe.MaxHealth}"; },
                newValue,
                S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4
            ).SetEase(Ease.OutQuart);
    }
    void ChangeCountVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = currentNumber.ToString(); },
                newValue,
                S_EffectActivator.Instance.GetEffectLifeTime() * 0.8f
            ).SetEase(Ease.OutQuart);
    }
    public void FoeSpriteBouncingVFX()
    {
        S_TweenHelper.Instance.BouncingVFX(sprite_Foe.transform, sprite_Foe.transform.localScale);
    }
    #endregion
    #region 보조
    public async Task AttackPlayer()
    {
        // VFX 생성 및 재생
        GameObject go = Instantiate(foeAttackVFX);

        // 공격이 적중할 때까지 대기
        await Task.Delay(go.GetComponent<S_AttackVFX>().GetHitTimeByMs(0.5f)); 

        // 딱 적중하는 시점에 체력 달기
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.DeathAttack && CurrentFoe.FoeInfo.IsMeetCondition)
        {
            await S_PlayerStat.Instance.GetDamagedByStand(9999);
        }
        else
        {
            await S_PlayerStat.Instance.GetDamagedByStand(1);
        }
    }
    public void DestroyFoeByEndTrial()
    {
        CurrentFoe = null;
        foeAttackVFX = null;
    }
    public void DamagedByHarm(int harmValue) // 피해로 인한 데미지
    {
        CurrentFoe.CurrentHealth -= harmValue;

        if (CurrentFoe.CurrentHealth > CurrentFoe.MaxHealth)
        {
            CurrentFoe.CurrentHealth = CurrentFoe.MaxHealth;
        }

        ChangeHealthValueVFX();
    }
    public void FixHealthByStand() // 스탠드 시 체력 고정
    {
        CurrentFoe.OldHealth = CurrentFoe.CurrentHealth;

        ChangeHealthValueVFX();
    }
    public void ResetHealthByTwist() // 비틀기 시 체력 되돌리기
    {
        CurrentFoe.CurrentHealth = CurrentFoe.OldHealth;

        ChangeHealthValueVFX();
    }
    public void ResetFoeActivatedCountByEndTrial() // 시련 종료 시 ActivatedCount 초기화
    {
        if (CurrentFoe.FoeInfo.Passive == S_FoePassiveEnum.NeedActivatedCount)
        {
            CurrentFoe.FoeInfo.ActivatedCount = 0;
        }
    }
    public void CheckFoeMeetCondition(S_Card card = null) // 조건 검사(ActivatedCount도 같이)
    {
        if (CurrentFoe.FoeInfo.Passive == S_FoePassiveEnum.NeedActivatedCount)
        {
            CurrentFoe.FoeInfo.CheckMeetConditionByActivatedCount(card);
        }
        else
        {
            CurrentFoe.FoeInfo.CheckMeetCondition(card);
        }

        UpdateFoeObject();
    }
    public void CheckFoeMeetConditionAfterEffect() // 이게 필요한 건 바로 발동하는 찐 메아리 효과만
    {
        CurrentFoe.FoeInfo.CheckMeetCondition();

        UpdateFoeObject();
    }
    public async Task ActivateStartTrialFoeByStartTrial()
    {
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.StartTrial)
        {
            await CurrentFoe.FoeInfo.ActiveFoeAbility(S_EffectActivator.Instance, null);
            CheckFoeMeetConditionAfterEffect();
        }
    }
    public async Task ActivateReverbFoeByHitCard(S_Card hitCard)
    {
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.StartTrial && CurrentFoe.FoeInfo.IsMeetCondition)
        {
            await CurrentFoe.FoeInfo.ActiveFoeAbility(S_EffectActivator.Instance, hitCard);
            CheckFoeMeetConditionAfterEffect();
        }

        S_PlayerStat.Instance.SaveStatHistory(hitCard, S_StatHistoryTriggerEnum.Foe);
    }
    public async Task ActivateStandFoeByStand()
    {
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.Stand && CurrentFoe.FoeInfo.IsMeetCondition)
        {
            await CurrentFoe.FoeInfo.ActiveFoeAbility(S_EffectActivator.Instance, null);
            CheckFoeMeetConditionAfterEffect();
        }
    }
    #endregion
}
