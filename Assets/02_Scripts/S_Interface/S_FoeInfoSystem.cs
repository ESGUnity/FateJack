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
    [Header("적")]
    [HideInInspector] public S_FoeObject CurrentFoe;
    GameObject foeAttackVFX;

    [Header("컴포넌트")]
    GameObject panel_FoeInfoBase;
    Image image_Foe;
    Image image_HealthBar;
    TMP_Text text_HealthValue;
    TMP_Text text_Count;

    GameObject image_NameAndAbilityBase;
    TMP_Text text_Name;
    TMP_Text text_Ability;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject sprite_IsMeetConditionEffect;

    [Header("UI")]
    Vector2 hidePos = new Vector2(0, 300);
    Vector2 originPos = new Vector2(0, 0);

    [Header("연출 관련")]
    Vector3 originScale;
    const float BOUNCING_SCALE_AMOUNT = 1.25f;

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
        image_Foe = Array.Find(images, c => c.gameObject.name.Equals("Image_Foe"));
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

        originScale = image_Foe.transform.localScale;

        InitPos();
    }

    public void SetFoe(S_FoeObject s_Creature) // 피조물 설정 및 텍스트 할당
    {
        // CurrentCreature에 세팅
        CurrentFoe = s_Creature;

        // 각종 텍스트 세팅
        image_HealthBar.fillAmount = 1f;
        text_HealthValue.text = $"{CurrentFoe.OldHealth} / {CurrentFoe.MaxHealth}";
        text_Name.text = CurrentFoe.FoeInfo.Name;
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
        creatureImageOpHandle.Completed += OnCreatureImageLoadComplete;

        // 피조물의 공격 VFX 세팅
        var attackVFXOpHandle = Addressables.LoadAssetAsync<GameObject>($"Prefab_VFX_TempCommon"); // TODO : Prefab_VFX_{CurrentFoe.FoeInfo.FoeType.ToString()}
        attackVFXOpHandle.Completed += OnAttackVFXLoadComplete;
    }
    void OnCreatureImageLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_Foe.sprite = opHandle.Result;
        }
    }
    void OnAttackVFXLoadComplete(AsyncOperationHandle<GameObject> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            foeAttackVFX = opHandle.Result;
        }
    }

    public void InitPos()
    {
        panel_FoeInfoBase.GetComponent<RectTransform>().anchoredPosition = hidePos;
        image_NameAndAbilityBase.SetActive(false);
        panel_FoeInfoBase.SetActive(false);
    }
    public void AppearCreature() // 패널 등장
    {
        // 패널 위치 초기화
        panel_FoeInfoBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_FoeInfoBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_FoeInfoBase.GetComponent<RectTransform>().DOAnchorPos(originPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearCreature() // 패널 퇴장
    {
        panel_FoeInfoBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_FoeInfoBase.GetComponent<RectTransform>().DOAnchorPos(hidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_FoeInfoBase.SetActive(false));
    }

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
    public void FoeImageBouncingVFX()
    {
        image_Foe.transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(image_Foe.transform.DOScale(originScale * BOUNCING_SCALE_AMOUNT, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad))
            .Append(image_Foe.transform.DOScale(originScale, S_EffectActivator.Instance.GetEffectLifeTime() / 4).SetEase(Ease.OutQuad));
    }

    public async Task AttackPlayer()
    {
        // VFX 생성 및 재생
        GameObject go = Instantiate(foeAttackVFX);

        // 공격이 적중할 때까지 대기
        await Task.Delay(go.GetComponent<S_AttackVFX>().GetHitTimeByMs(0.5f)); 

        // 딱 적중하는 시점에 체력 달기
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.DeathAttack && CurrentFoe.FoeInfo.CanActivateEffect)
        {
            await S_PlayerStat.Instance.GetDamagedByStand(9999);
        }
        else
        {
            await S_PlayerStat.Instance.GetDamagedByStand(1);
        }
    }
    public void PointerEnterInCreatureImage()
    {
        image_NameAndAbilityBase.SetActive(true);
    }
    public void PointerExitInCreatureImage()
    {
        image_NameAndAbilityBase.SetActive(false);
    }
    public void DefeatCreatureByEndTrial()
    {
        Destroy(CurrentFoe.gameObject);
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

        if (CurrentFoe.FoeInfo.CanActivateEffect)
        {
            sprite_IsMeetConditionEffect.SetActive(true);
        }
        else
        {
            sprite_IsMeetConditionEffect.SetActive(false);
        }
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
    public void CalcFoeActivatedCountByHit(S_Card card) // ActivatedCount 능력을 위한 메서드
    {
        if (CurrentFoe.FoeInfo.Passive == S_FoePassiveEnum.NeedActivatedCount)
        {
            CurrentFoe.FoeInfo.ActivateCount(card);
        }
    }
    public void SubtractFoeActivatedCountByTwist(List<S_Card> cards)
    {
        foreach (S_Card card in cards)
        {
            if (CurrentFoe.FoeInfo.Passive == S_FoePassiveEnum.NeedActivatedCount)
            {
                CurrentFoe.FoeInfo.ActivateCount(card, true);
            }
        }

        // 만약 스택에 카드가 없다면, 즉 카드를 내서 ActivatedCount가 활성화된게 아니라면 그냥 0. 이거 안하면 ActivatedCount가 최대치로 갈 수 있다.
        if (S_PlayerCard.Instance.GetPreStackCards().Count <= 0)
        {
            CurrentFoe.FoeInfo.ActivatedCount = 0;
        }
    }
    public void ResetFoeActivatedCountByEndTrial()
    {
        if (CurrentFoe.FoeInfo.Passive == S_FoePassiveEnum.NeedActivatedCount)
        {
            CurrentFoe.FoeInfo.ActivatedCount = 0;
        }
    }
    public void CheckFoeMeetCondition(S_Card card = null) // 일부 Reverb류와 대부분의 조건이 충족하는지 확인하는 메서드
    {
        CurrentFoe.FoeInfo.IsMeetCondition(card);
    }
    public async Task ActivateStartTrialFoeByStartTrial()
    {
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.StartTrial)
        {
            await CurrentFoe.FoeInfo.ActiveFoeAbility(S_EffectActivator.Instance, null);
            CurrentFoe.FoeInfo.IsMeetCondition();
            UpdateFoeObject();
        }
    }
    public async Task ActivateReverbFoeByHitCard(S_Card hitCard)
    {
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.StartTrial && CurrentFoe.FoeInfo.CanActivateEffect)
        {
            await CurrentFoe.FoeInfo.ActiveFoeAbility(S_EffectActivator.Instance, hitCard);
            CurrentFoe.FoeInfo.IsMeetCondition();
            UpdateFoeObject();
        }

        S_PlayerStat.Instance.SaveStatHistory(hitCard, S_StatHistoryTriggerEnum.Foe);
    }
    public async Task ActivateStandFoeByStand()
    {
        if (CurrentFoe.FoeInfo.Condition == S_FoeAbilityConditionEnum.Stand && CurrentFoe.FoeInfo.CanActivateEffect)
        {
            await CurrentFoe.FoeInfo.ActiveFoeAbility(S_EffectActivator.Instance, null);
            CurrentFoe.FoeInfo.IsMeetCondition(); // 스탠드인데 또 컨디션 확인하는 이유 : 한 턴에 ~ 하는 overflow류 능력은 능력 발동하자마자 ActivatedCount가 0이 된다. 그래서 또 조건 검사해야 CanActivateEffect가 초기화된다.
            UpdateFoeObject();
        }
    }
}
