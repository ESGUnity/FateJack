using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_FoeInfoSystem : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public S_FoeInfo FoeInfo;

    [Header("프리팹")]
    [SerializeField] GameObject prefab_Section;
    [HideInInspector] public GameObject Instance_Section;

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
    Vector3 ATTACK_TARGET_POS = new Vector3(0, -5, -1.15f);
    Vector3 ATTACK_POS_AMOUNT_1 = new Vector3(0, 0, -1);

    [Header("포인터 연출")]
    bool isEnter = false;
    List<S_GameFlowStateEnum> VALID_STATES;

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

        // 유효 상태
        VALID_STATES = new() { S_GameFlowStateEnum.Hit };

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

    #region 초기화
    public void StartFoeTrial()
    {
        // CurrentCreature에 세팅
        FoeInfo = S_FoeManager.Instance.GetFoeInfo();

        // 이름 설정
        text_Name.text = FoeInfo.CurrentFoe.Name;
        if (FoeInfo.CurrentFoe.FoeType == S_FoeTypeEnum.Clotho_Elite || FoeInfo.CurrentFoe.FoeType == S_FoeTypeEnum.Lachesis_Elite || FoeInfo.CurrentFoe.FoeType == S_FoeTypeEnum.Atropos_Elite)
        {
            text_Name.text = $"{text_Name.text}(엘리트)";
        }
        if (FoeInfo.CurrentFoe.FoeType == S_FoeTypeEnum.Clotho_Boss || FoeInfo.CurrentFoe.FoeType == S_FoeTypeEnum.Lachesis_Boss || FoeInfo.CurrentFoe.FoeType == S_FoeTypeEnum.Atropos_Boss)
        {
            text_Name.text = $"{text_Name.text}(보스)";
        }

        // 능력 설정
        text_Ability.text = FoeInfo.CurrentFoe.Description;

        // ActivatedCount
        if (FoeInfo.CurrentFoe.IsNeedActivatedCount)
        {
            text_Count.gameObject.SetActive(true);
            text_Count.text = FoeInfo.CurrentFoe.ActivatedCount.ToString();
        }
        else
        {
            text_Count.gameObject.SetActive(false);
        }

        // 체력바
        image_HealthBar.fillAmount = 1f;
        text_HealthValue.text = $"{FoeInfo.OldHealth} / {FoeInfo.MaxHealth}";

        panel_FoeInfoBase.SetActive(true);
        panel_FoeInfoBase.GetComponent<RectTransform>().DOKill();
        panel_FoeInfoBase.GetComponent<RectTransform>().DOAnchorPos(originPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // 섹션 등장
        Instance_Section = Instantiate(prefab_Section);
        Instance_Section.GetComponent<S_SectionObj>().SpawnSection($"Character_{FoeInfo.CurrentFoe.Key}");
        Instance_Section.GetComponent<S_SectionObj>().sprite_MeetConditionEffect.gameObject.SetActive(false);

        // 섹션의 스프라이트에 포인터 엔터 바인딩하기.
        EventTrigger spriteTrigger = Instance_Section.GetComponent<S_SectionObj>().sprite_Character.GetComponent<EventTrigger>();
        EventTrigger.Entry spritePointerEnterEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);
        EventTrigger.Entry spritePointerExitEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerExit);
        // 바인딩
        spritePointerEnterEntry.callback.AddListener((eventData) => { PointerEnterInFoeSprite((PointerEventData)eventData); });
        spritePointerExitEntry.callback.AddListener((eventData) => { PointerExitInFoeSprite((PointerEventData)eventData); });

    }
    public void StartFoeTrialByTutorial()
    {
        // CurrentCreature에 세팅
        FoeInfo = S_FoeManager.Instance.GetTutorialCharacterInfo();

        // 이름 설정
        text_Name.text = $"{FoeInfo.CurrentFoe.Name}(보스)";

        // 능력 설정
        text_Ability.text = FoeInfo.CurrentFoe.Description;

        // ActivatedCount
        text_Count.gameObject.SetActive(false);

        // 체력바
        image_HealthBar.fillAmount = 1f;
        text_HealthValue.text = $"{FoeInfo.OldHealth} / {FoeInfo.MaxHealth}";

        panel_FoeInfoBase.SetActive(true);
        panel_FoeInfoBase.GetComponent<RectTransform>().DOKill();
        panel_FoeInfoBase.GetComponent<RectTransform>().DOAnchorPos(originPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // 섹션 등장
        Instance_Section = Instantiate(prefab_Section);
        Instance_Section.GetComponent<S_SectionObj>().SpawnSection($"Character_{FoeInfo.CurrentFoe.Key}");
        Instance_Section.GetComponent<S_SectionObj>().sprite_MeetConditionEffect.gameObject.SetActive(false);
    }
    #endregion
    #region 연출
    public void PointerEnterInFoeSprite(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            S_HoverInfoSystem.Instance.ActivateHoverInfo(FoeInfo.CurrentFoe, Instance_Section.GetComponent<S_SectionObj>().sprite_Character);

            isEnter = true;
        }
    }
    public void PointerExitInFoeSprite(PointerEventData eventData)
    {
        ForceExit();
    }
    void ForceExit()
    {
        if (!isEnter) return;

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();
    }
    public void UpdateFoeObject() // 적 오브젝트 업데이트
    {
        if (FoeInfo.CurrentFoe.IsNeedActivatedCount)
        {
            ChangeCountVFXTween(int.Parse(text_Count.text), FoeInfo.CurrentFoe.ActivatedCount, text_Count);
        }
        else
        {
            text_Count.gameObject.SetActive(false);
        }

        if (FoeInfo.CurrentFoe.IsMeetCondition)
        {
            Instance_Section.GetComponent<S_SectionObj>().sprite_MeetConditionEffect.gameObject.SetActive(true);
        }
        else
        {
            Instance_Section.GetComponent<S_SectionObj>().sprite_MeetConditionEffect.gameObject.SetActive(false);
        }
    }
    public void FoeBouncingVFX()
    {
        S_TweenHelper.Instance.BouncingVFX(Instance_Section.GetComponent<S_SectionObj>().sprite_Character.transform, Instance_Section.GetComponent<S_SectionObj>().sprite_Character.transform.localScale);
        S_TweenHelper.Instance.BouncingVFX(Instance_Section.GetComponent<S_SectionObj>().sprite_MeetConditionEffect.transform, Instance_Section.GetComponent<S_SectionObj>().sprite_MeetConditionEffect.transform.localScale);
    }
    #endregion
    #region VFX
    public void ChangeHealthValueVFX()
    {
        if (FoeInfo == null) return;

        image_HealthBar.DOFillAmount((float)FoeInfo.CurrentHealth / FoeInfo.MaxHealth, S_EffectActivator.Instance.GetEffectLifeTime() / 5 * 4).SetEase(Ease.OutQuart);
        ChangeHealthValueVFXTween(int.Parse(text_HealthValue.text.Split('/')[0].Trim()), FoeInfo.CurrentHealth, text_HealthValue);
    }
    void ChangeHealthValueVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = $"{currentNumber} / {FoeInfo.MaxHealth}"; },
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
    #endregion
    #region 시련 진행에 따른 메서드
    public async Task UpdateFoeByStartTrial()
    {
        await S_EffectActivator.Instance.ActivateFoeByStartTrial();
    }
    public void UpdateFoeByStartNewTurn()
    {
        // 매 턴마다 능력치가 변경되는 효과의 능력치를 바꿔주기
        if (FoeInfo.CurrentFoe.Effect == S_TrinketEffectEnum.Harm_TwoStat_Random)
        {
            List<S_BattleStatEnum> stat = new() { S_BattleStatEnum.Str_Mind, S_BattleStatEnum.Str_Luck, S_BattleStatEnum.Mind_Luck };
            FoeInfo.CurrentFoe.Stat = stat[UnityEngine.Random.Range(0, stat.Count)];
        }

        // 한 턴에 ~ 하는 효과의 조건 충족 여부를 무조건 false로 바꾸기
        if (FoeInfo.CurrentFoe.Condition == S_TrinketConditionEnum.Only || FoeInfo.CurrentFoe.Condition == S_TrinketConditionEnum.Resection_Three || FoeInfo.CurrentFoe.Condition == S_TrinketConditionEnum.Overflow_Six ||
            FoeInfo.CurrentFoe.Condition == S_TrinketConditionEnum.GrandChaos_One || FoeInfo.CurrentFoe.Condition == S_TrinketConditionEnum.GrandChaos_Two)
        {
            FoeInfo.CurrentFoe.ActivatedCount = 0;
            FoeInfo.CurrentFoe.IsMeetCondition = false;
        }

        // 조건 검사 한 번 해주기
        S_EffectActivator.Instance.CheckFoeMeetCondition();

        // ActivatedCount와 IsMeetCondition이 변경되었음으로 상태 업데이트
        UpdateFoeObject();
    }
    public void ResetHealthByTwist() // 비틀기 시 체력 되돌리기
    {
        FoeInfo.CurrentHealth = FoeInfo.OldHealth;

        ChangeHealthValueVFX();
    }
    public void FixHealthByStand() // 스탠드 시 체력 고정
    {
        FoeInfo.OldHealth = FoeInfo.CurrentHealth;
    }
    public async Task ExitFoeByEndTrial()
    {
        panel_FoeInfoBase.GetComponent<RectTransform>().DOKill();
        panel_FoeInfoBase.GetComponent<RectTransform>().DOAnchorPos(hidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_FoeInfoBase.SetActive(false));

        await Instance_Section.GetComponent<S_SectionObj>().ExitCharacter();
        FoeInfo = null;
    }
    public void ExitSectionByEndTrial()
    {
        Instance_Section.GetComponent<S_SectionObj>().ExitSection();
    }
    #endregion
    #region 보조
    public bool IsFoeHavePassive(S_TrinketPassiveEnum passive) // passive가 있는지 없는치 확인하는 메서드
    {
        return FoeInfo.CurrentFoe.Passive == passive;
    }
    public string GetFoeDescription(S_Foe foe) // 설명에 추가로 붙은 것들 메서드
    {
        StringBuilder sb = new();

        sb.Append(foe.Description);

        // 무작위 능력치 2개
        if (foe.Effect == S_TrinketEffectEnum.Harm_TwoStat_Random)
        {
            switch (foe.Stat)
            {
                case S_BattleStatEnum.Str_Mind:
                    sb.Replace("능력치 2개를", "힘과 정신력을");
                    break;
                case S_BattleStatEnum.Str_Luck:
                    sb.Replace("능력치 2개를", "힘과 행운을");
                    break;
                case S_BattleStatEnum.Mind_Luck:
                    sb.Replace("능력치 2개를", "정신력과 행운을");
                    break;
            }
        }

        // 조건에 따라 추가 설명
        switch (foe.Condition)
        {
            case S_TrinketConditionEnum.Reverb_Two:
                sb.Append($"\n(낸 카드 : {foe.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.Reverb_Three:
                sb.Append($"\n(낸 카드 : {foe.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.Precision_Six:
                sb.Append($"\n(스택의 카드 : {foe.ActivatedCount}장)");
                break;
            case S_TrinketConditionEnum.Legion_Twenty:
                sb.Append($"\n(스택의 카드 무게 합 : {foe.ActivatedCount})");
                break;
            case S_TrinketConditionEnum.Resection_Three:
                sb.Append($"\n(이번 턴에 낸 카드 : {foe.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.Overflow_Six:
                sb.Append($"\n(이번 턴에 낸 카드 : {foe.ActivatedCount}장 째)");
                break;
            case S_TrinketConditionEnum.GrandChaos_One:
                sb.Append($"\n(만족한 카드 타입 : {foe.ActivatedCount}개)");
                break;
            case S_TrinketConditionEnum.GrandChaos_Two:
                sb.Append($"\n(만족한 카드 타입 : {foe.ActivatedCount}개)");
                break;
        }

        // 예상 값 추가
        switch (foe.Effect)
        {
            case S_TrinketEffectEnum.Harm:
                sb.Append($"\n(예상 피해량 : {foe.ExpectedValue})");
                break;
            case S_TrinketEffectEnum.Harm_TwoStat_Random:
                sb.Append($"\n(예상 피해량 : {foe.ExpectedValue})");
                break;
            case S_TrinketEffectEnum.Harm_AllStat:
                sb.Append($"\n(예상 피해량 : {foe.ExpectedValue})");
                break;
            case S_TrinketEffectEnum.Stat_Multi:
                sb.Append($"\n(예상 증가량 : {foe.ExpectedValue})");
                break;
        }

        return sb.ToString();
    }
    public async Task AttackPlayer()
    {
        GameObject character = Instance_Section.GetComponent<S_SectionObj>().sprite_Character;
        GameObject meetCondition = Instance_Section.GetComponent<S_SectionObj>().sprite_MeetConditionEffect;

        Sequence seq = DOTween.Sequence();

        // 살짝 들리기
        seq.Append(character.transform.DOLocalMove(character.transform.localPosition + ATTACK_POS_AMOUNT_1, 0.3f).SetEase(Ease.OutQuart))
            .Join(meetCondition.transform.DOLocalMove(character.transform.localPosition + ATTACK_POS_AMOUNT_1, 0.3f).SetEase(Ease.OutQuart));

        // 공격
        seq.Append(character.transform.DOLocalMove(ATTACK_TARGET_POS, 0.2f).SetEase(Ease.OutQuart))
            .Join(meetCondition.transform.DOLocalMove(ATTACK_TARGET_POS, 0.2f).SetEase(Ease.OutQuart));

        // 딱 적중하는 시점에 체력 달기
        if (FoeInfo.CurrentFoe.Effect == S_TrinketEffectEnum.DeathAttack && FoeInfo.CurrentFoe.IsMeetCondition) // 즉사 효과에 조건 충족했다면 즉사
        {
            seq.AppendCallback(() => _ = S_PlayerStat.Instance.GetDamagedByStand(9999));
        }
        else if (IsFoeHavePassive(S_TrinketPassiveEnum.NoDeterminationDeathAttack)) // 격동하는 오이지스의 의지 0일 때 즉사
        {
            if (S_PlayerStat.Instance.GetCurrentDetermination() <= 0)
            {
                seq.AppendCallback(() => _ = S_PlayerStat.Instance.GetDamagedByStand(9999));
            }
        }
        else // 아니라면 1의 피해
        {
            seq.AppendCallback(() => _ = S_PlayerStat.Instance.GetDamagedByStand(1));
        }

        // 원위치
        seq.Append(character.transform.DOLocalMove(Instance_Section.GetComponent<S_SectionObj>().CHARACTER_ORIGIN_POS, 0.4f).SetEase(Ease.OutQuart))
           .Join(meetCondition.transform.DOLocalMove(Instance_Section.GetComponent<S_SectionObj>().MEET_CONDITION_EFFECT_ORIGIN_POS, 0.4f).SetEase(Ease.OutQuart));

        await seq.AsyncWaitForCompletion();
    }
    public void DamagedByHarm(int harmValue) // 피해로 인한 데미지
    {
        FoeInfo.CurrentHealth -= harmValue;

        if (FoeInfo.CurrentHealth > FoeInfo.MaxHealth)
        {
            FoeInfo.CurrentHealth = FoeInfo.MaxHealth;
        }

        ChangeHealthValueVFX();
    }
    #endregion
}
