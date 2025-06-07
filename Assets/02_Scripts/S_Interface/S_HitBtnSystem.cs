using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_HitBtnSystem : MonoBehaviour
{
    [Header("씬 오브젝트")]
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByTwistBtn;

    [Header("컴포넌트")]
    GameObject panel_HitBtnBase;
    GameObject image_BasicHitBtnBase;
    GameObject image_TwistBtnBase;

    GameObject image_HoverHitBtnInfoBase;
    TMP_Text text_CleanHitProb;
    TMP_Text text_BurstProb;

    [Header("UI")]
    Vector2 btnBaseHidePos = new Vector2(0, -140);
    Vector2 btnBaseOriginPos = new Vector2(0, 85);
    int prevDeckCount;
    int prevStackSum;

    // 싱글턴
    static S_HitBtnSystem instance;
    public static S_HitBtnSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // 히트 버튼 관련 컴포넌트 할당
        panel_HitBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_HitBtnBase")).gameObject;
        image_BasicHitBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BasicHitBtnBase")).gameObject;
        image_TwistBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_TwistBtnBase")).gameObject;

        image_HoverHitBtnInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HoverHitBtnInfoBase")).gameObject;
        text_CleanHitProb = Array.Find(texts, c => c.gameObject.name.Equals("Text_CleanHitProb"));
        text_BurstProb = Array.Find(texts, c => c.gameObject.name.Equals("Text_BurstProb"));

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
    S_GameFlowStateEnum prevState;
    void Update()
    {
        // 버스트 및 클린히트를 알려주는 패널. Hit 일 때만 켜지고 아니면 꺼짐. 또한 Hit 켜질 때 마우스 올려져있으면 켜짐
        S_GameFlowStateEnum currentState = S_GameFlowManager.Instance.GameFlowState;
        if (prevState != currentState)
        {
            // 1. 상태가 Hit가 아니면 패널 끔
            if (currentState != S_GameFlowStateEnum.Hit)
            {
                PointerExitOnHitBtn();
                PointerExitOnTwistBtn();
            }
            // 2. 이전 상태는 Hit가 아니었고, 현재 상태가 Hit로 바뀐 경우
            else if (prevState != S_GameFlowStateEnum.Hit && currentState == S_GameFlowStateEnum.Hit)
            {
                if (IsPointerOverUIObject(image_BasicHitBtnBase))
                {
                     PointerEnterOnHitBtn();
                }
                if (IsPointerOverUIObject(image_TwistBtnBase))
                {
                    PointerExitOnTwistBtn();
                }
            }

            prevState = currentState;
        }
    }

    public void InitPos()
    {
        panel_HitBtnBase.GetComponent<RectTransform>().anchoredPosition = btnBaseHidePos;
        panel_HitBtnBase.SetActive(false);
    }
    public void AppearHitBtn() // 패널 등장
    {
        panel_HitBtnBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_HitBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_HitBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnBaseOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearHitBtn() // 패널 퇴장
    {
        panel_HitBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_HitBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnBaseHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_HitBtnBase.SetActive(false));
    }

    #region 버튼 함수
    // 의지 히트 버튼은 S_DeckInfoSystem에 존재
    public async void ClickHitBtnAsync() // 히트, 다이얼로그 시에만 작동
    {
        // Dialog 시스템 전용
        S_DialogInfoSystem.Instance.ClickNextBtn();

        if ((S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit) && !S_PlayerStat.Instance.IsBurst && S_PlayerCard.Instance.GetPreDeckCards().Count > 0)
        {
            if (S_PlayerStat.Instance.IsExpansion)
            {
                S_UICardEffecter.Instance.StartExpansionCards();
            }
            else
            {
                S_Card hitCard = S_PlayerCard.Instance.DrawRandomCard(1)[0];

                // 카드 내기
                await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(hitCard, S_CardOrderTypeEnum.BasicHit);

                // 우선이 있었다면 해제
                if (S_PlayerStat.Instance.IsFirst != S_FirstEffectEnum.None) await S_EffectActivator.Instance.AppliedFirstAsync();

                // 히트 카드 진행
                if (S_GameFlowManager.Instance.GetCardOrderQueueCount() <= 1)
                {
                    await S_GameFlowManager.Instance.StartHittingCard();
                }
            }
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
        else if (S_PlayerStat.Instance.IsBurst)
        {
            S_InGameUISystem.Instance.CreateLog("버스트 시엔 히트할 수 없습니다.");
        }
        else if (S_PlayerCard.Instance.GetPreDeckCards().Count <= 0)
        {
            S_InGameUISystem.Instance.CreateLog("덱에 카드가 없습니다!");
        }
    }
    public void ClickTwistBtn() // 히트, 다이얼로그 시에만 작동
    {
        // Dialog 시스템 전용
        S_DialogInfoSystem.Instance.ClickNextBtn();

        if ((S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit) && !S_PlayerStat.Instance.IsBurst && S_PlayerStat.Instance.CanUseDetermination() && S_GameFlowManager.Instance.IsCurrentTurnHitted())
        {
            S_GameFlowManager.Instance.StartTwist();
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
        else if (S_PlayerStat.Instance.IsBurst)
        {
            S_InGameUISystem.Instance.CreateLog("버스트 시엔 할 수 없어.");
        }
        else if (!S_PlayerStat.Instance.CanUseDetermination())
        {
            S_InGameUISystem.Instance.CreateLog("의지가 부족하다네~");
        }
        else if (!S_GameFlowManager.Instance.IsCurrentTurnHitted())
        {
            S_InGameUISystem.Instance.CreateLog("비틀 운명이 없어.");
        }
    }
    public void ClickStandBtn() // 히트, 다이얼로그 시에만 작동
    {
        // Dialog 시스템 전용
        S_DialogInfoSystem.Instance.ClickNextBtn();

        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit)
        {
            S_GameFlowManager.Instance.StartStand();
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
    }
    public async void SelectHitCardByExpansion(S_Card card) // S_ExpansionCard에서 호출하는 메서드
    {
        // 카드 내기
        await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(card, S_CardOrderTypeEnum.BasicHit);

        // 전개 해제
        await S_EffectActivator.Instance.AppliedExpansionAsync();

        // 우선이 있었다면 해제
        if (S_PlayerStat.Instance.IsFirst != S_FirstEffectEnum.None) await S_EffectActivator.Instance.AppliedFirstAsync();

        // 히트 카드 진행
        if (S_GameFlowManager.Instance.GetCardOrderQueueCount() <= 1)
        {
            await S_GameFlowManager.Instance.StartHittingCard();
        }
    }
    #endregion
    #region 버스트 및 클린히트 알려주는 기능
    public void RenewProbText()
    {
        int limit = S_PlayerStat.Instance.CurrentLimit;
        int stackSum = S_PlayerStat.Instance.StackSum;
        var preDeckCards = S_PlayerCard.Instance.GetPreDeckCards();

        S_FirstEffectEnum firstEffect = S_PlayerStat.Instance.IsFirst;

        if (firstEffect != S_FirstEffectEnum.None)
        {
            var firstCards = S_PlayerCard.Instance.GetValidCardsByFirst();

            if (firstCards.Count > 0)
            {
                CalculateProb(firstCards, stackSum, limit);
                return;
            }
        }

        // None이거나, 우선 카드가 없을 때
        CalculateProb(preDeckCards, stackSum, limit);
    }
    void CalculateProb(List<S_Card> cards, int stackSum, int limit)
    {
        int cleanHitCount = 0;
        int burstCount = 0;
        int totalCount = cards.Count;

        foreach (var c in cards)
        {
            int i = stackSum + c.Number;
            if (i == limit)
            {
                cleanHitCount++;
            }
            else if (i > limit)
            {
                burstCount++;
            }
        }

        float cleanHitProbF = (float)cleanHitCount / totalCount * 100f;
        float burstProbF = (float)burstCount / totalCount * 100f;

        text_CleanHitProb.text = $"{cleanHitProbF.ToString("F1")}%";
        text_BurstProb.text = $"{burstProbF.ToString("F1")}%";
    }
    public void PointerEnterOnHitBtn()
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit)
        {
            RenewProbText();

            if (!image_HoverHitBtnInfoBase.activeInHierarchy)
            {
                image_HoverHitBtnInfoBase.SetActive(true);
            }
        }
    }
    public void PointerExitOnHitBtn()
    {
        image_HoverHitBtnInfoBase.SetActive(false);
    }
    #endregion
    #region 비틀기 시 사라지는 카드 보여주는 기능
    public void PointerEnterOnTwistBtn()
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit && S_StackInfoSystem.Instance.GetCurrentTurnHitCardObjects().Count != 0)
        {
            sprite_BlackBackgroundByTwistBtn.DOKill();
            sprite_BlackBackgroundByTwistBtn.DOFade(0.8f, 0.1f);

            int order = 600;
            foreach (GameObject go in S_StackInfoSystem.Instance.GetCurrentTurnHitCardObjects())
            {
                go.GetComponent<S_StackCard>().SetOrder(order);

                order += 10;
            }
        }
    }
    public void PointerExitOnTwistBtn()
    {
        sprite_BlackBackgroundByTwistBtn.DOKill();
        sprite_BlackBackgroundByTwistBtn.DOFade(0f, 0.1f);

        S_StackInfoSystem.Instance.SortStackVFXOnlyOrder();
    }
    #endregion
    #region 보조 메서드
    bool IsPointerOverUIObject(GameObject target)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject == target || result.gameObject.transform.IsChildOf(target.transform))
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}