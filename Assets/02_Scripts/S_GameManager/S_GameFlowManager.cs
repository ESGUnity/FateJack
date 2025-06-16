using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class S_GameFlowManager : MonoBehaviour
{
    [Header("시련 및 턴 관련")]
    [HideInInspector] public int CurrentTrial;
    [HideInInspector] public int CurrentTurn;
    [HideInInspector] public S_GameFlowStateEnum GameFlowState;

    [Header("연출 관련")]
    public const float PANEL_APPEAR_TIME = 0.5f;
    public static Vector3 InGameCameraPos { get; set; }
    public static Vector3 InGameCameraRot { get; set; }
    public static Vector3 DeckCameraPos { get; set; }
    public static Vector3 DeckCameraRot { get; set; }
    public static Vector3 StoreCameraPos { get; set; }
    public static Vector3 StoreCameraRot { get; set; }

    [Header("보상 관련")]
    [HideInInspector] public int SlayFoeGold = 5;
    [HideInInspector] public int SlayEliteFoeGold = 3;
    [HideInInspector] public int SlayBossFoeGold = 10;
    [HideInInspector] public int RemainHealthPerGold = 1;
    [HideInInspector] public int RemainDeterminationPerGold = 1;

    [Header("다이얼로그 관련")]
    [HideInInspector] public bool IsCompleteDialog;

    [Header("히트 관련")]
    Queue<S_CardOrder> cardOrderQueue = new();

    // 싱글턴
    static S_GameFlowManager instance;
    public static S_GameFlowManager Instance { get { return instance; } }

    void Awake()
    {
        InGameCameraPos = new Vector3(0, 22.7f, -13f);
        InGameCameraRot = new Vector3(60f, 0, 0);
        DeckCameraPos = new Vector3(0, 27f, -10.9f);
        DeckCameraRot = new Vector3(85f, 0, 0);
        StoreCameraPos = new Vector3(0f, 22.9f, -3.15f);
        StoreCameraRot = new Vector3(60, 0, 0);

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
        CurrentTrial = 0;
        GameFlowState = S_GameFlowStateEnum.None;

        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            PlayerPrefs.SetInt("TutorialCompleted", 0);
            PlayerPrefs.Save();

            S_TutorialManager.Instance.StartTutorial();
        }
        else if (PlayerPrefs.GetInt("TutorialCompleted") == 0)
        {
            S_TutorialManager.Instance.StartTutorial();
        }
        else
        {
            StartTrial();
        }
    }

    public async void StartTrial()
    {
        /////////////////////////데모판 코드//////////////////////////
        //if (CurrentTrial == 4)
        //{
        //    S_DemoSystem.Instance.AppearDemoPanel();
        //    return;
        //}

        GameFlowState = S_GameFlowStateEnum.None;
 
        // 시련 및 턴 설정
        CurrentTrial++;
        S_StatInfoSystem.Instance.ChangeCurrentTrialText();
        CurrentTurn = 0;

        // TODO : 모이라이의 다이얼로그

        // 첫 시련 시 설정
        if (CurrentTrial == 1)
        {
            S_PlayerCard.Instance.InitDeckByStartGame(); // 덱 초기화
            S_PlayerStat.Instance.InitStatsByStartGame(); // 스탯 초기화
            S_PlayerTrinket.Instance.InitTrinketsByStartGame(); // 초기 전리품 생성
            S_FoeManager.Instance.GenerateFoeByStartGame(); // 모든 적 생성
        }

        // 피조물 생성
        S_FoeManager.Instance.SpawnFoe();
        S_FoeInfoSystem.Instance.AppearUIFoe();
        S_FoeInfoSystem.Instance.AppearFoeSprite();
        // 카메라 이동
        Camera.main.transform.DOMove(InGameCameraPos, PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        Camera.main.transform.DORotate(InGameCameraRot, PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        // UI 등장 시간동안 대기
        await WaitPanelAppearTimeAsync();

        // 적의 조건 체크
        S_FoeInfoSystem.Instance.CheckFoeMeetCondition(); // 없앨예정

        // 시련 시작 시 카쓸적 업데이트
        await S_PlayerCard.Instance.UpdateCardsByStartTrial();
        await S_PlayerTrinket.Instance.UpdateTrinketByStartTrial();
        await S_FoeInfoSystem.Instance.ActivateStartTrialFoeByStartTrial();

        // 시련 시작 시로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.StartTrial);

        // 턴 시작
        StartNewTurn();
    }
    public void StartNewTurn()
    {
        // 턴 늘리고 UI 등장
        CurrentTurn++;
        S_InGameUISystem.Instance.AppearCurrentTurnUI();

        // 턴 시작 시 카쓸적 업데이트
        S_PlayerCard.Instance.UpdateCardByStartNewTurn();
        S_PlayerTrinket.Instance.UpdateTrinketByStartNewTurn();
        S_FoeInfoSystem.Instance.UpdateFoeObject(); // TODO 

        // 히트 버튼 등장
        S_HitBtnSystem.Instance.AppearHitBtn();

        GameFlowState = S_GameFlowStateEnum.Hit;
    }
    // 효과 발동 -> 카드 업데이트 -> 조건 업데이트
    public async Task EnqueueCardOrderAndUpdateCardsState(S_Card card, S_CardOrderTypeEnum type) // 히트, 제외 시 호출. 여긴 내는 시늉만 하는 곳. 창조와 인도 때문에...
    {
        GameFlowState = S_GameFlowStateEnum.HittingCard;

        // 오더큐에 추가
        S_CardOrder order = new S_CardOrder(card, type);
        cardOrderQueue.Enqueue(order);

        // 카드에 의한 히스토리 처리(중요한 거 아니라 오더큐에 들어가면 그냥 계산해버린다.)
        S_PlayerStat.Instance.CalcHistory(type, card);

        // 기본 히트, 의지 히트, 제외에 따라서 다르게 처리
        switch (type)
        {
            case S_CardOrderTypeEnum.Hit:
                S_PlayerCard.Instance.HitCard(card);
                S_DeckInfoSystem.Instance.UpdateDeckCardsState();
                await S_StackInfoSystem.Instance.HitToStackAsync(card);
                break;
            case S_CardOrderTypeEnum.Gen:
                S_PlayerCard.Instance.GenCard(card);
                S_DeckInfoSystem.Instance.UpdateDeckCardsState();
                await S_StackInfoSystem.Instance.HitToStackAsync(card);
                break;
            case S_CardOrderTypeEnum.Exclusion:
                S_PlayerCard.Instance.ExclusionCard(card);
                S_DeckInfoSystem.Instance.UpdateDeckCardsState();
                await S_StackInfoSystem.Instance.ExclusionCardAsync(card);
                break;
        }

        // 메아리와 발현 각인만 신경쓰면 되기에 바로 실행
        if (GetCardOrderQueueCount() > 0)
        {
            await StartHittingCard();
        }
    }
    public async Task StartHittingCard() // 히트, 덱 제외 시 호출. 대부분 쓸만한 물건이 히트할 때마다 그거에서 씀. 카드는 각인 중에 발현 그거만.
    {
        GameFlowState = S_GameFlowStateEnum.HittingCard;

        // 큐에서 꺼내어 히트 계산하기
        while (cardOrderQueue.Count > 0)
        {
            S_CardOrder cardOrder = cardOrderQueue.Peek();
            S_Card executedCard = cardOrder.Card;

            // 카쓸적 조건 확인
            S_EffectActivator.Instance.CheckCardMeetCondition();
            S_EffectActivator.Instance.CheckTrinketMeetCondition(executedCard); // 쓸만한 물건엔 ~ 낼 때마다가 있기 때문에 히트한 카드를 인수로 넣어줘야한다.
            S_FoeInfoSystem.Instance.CheckFoeMeetCondition(executedCard); // TODO

            // 카드를 냈다면 효과 발동
            if (cardOrder.Type == S_CardOrderTypeEnum.Hit || cardOrder.Type == S_CardOrderTypeEnum.Gen)
            {
                // 카쓸적 효과 발동
                await S_EffectActivator.Instance.ActivateCardByHit(executedCard, cardOrder.Type); // 기본적인 무게 계산을 위해 발현이 없더라도 꼭 카드를 낼 때마다 해야한다.
                await S_EffectActivator.Instance.ActivateTrinketByHit(executedCard);
                await S_FoeInfoSystem.Instance.ActivateReverbFoeByHitCard(executedCard);
            }

            // 카드에 의한 히스토리 저장
            S_PlayerStat.Instance.SaveStatHistory(executedCard, S_StatHistoryTriggerEnum.Card);

            // 오더 큐에서 디큐하기
            S_DeckInfoSystem.Instance.UpdateDeckCardsState();
            cardOrderQueue.Dequeue();
        }

        // Hitting -> Hit로 변환
        GameFlowState = S_GameFlowStateEnum.Hit;

        // 히트 도중 피조물이 죽었다면 전투 종료
        if (S_FoeInfoSystem.Instance.CurrentFoe.CurrentHealth <= 0)
        {
            EndTrial();
        }
    }
    public async void StartTwist()
    {
        GameFlowState = S_GameFlowStateEnum.Twist;

        // 의지 사용
        S_PlayerStat.Instance.UseDetermination();

        // 카드 돌아오기 및 생성된 카드는 사라지기.
        S_PlayerCard.Instance.ResetCardsByTwist(out List<S_Card> stacks, out List<S_Card> exclusions);
        await S_StackInfoSystem.Instance.ReturnCardsByTwistAsync(stacks);

        // 비틀기 시 카쓸적 조건 체크
        S_EffectActivator.Instance.CheckCardMeetCondition();
        S_EffectActivator.Instance.CheckTrinketMeetCondition();
        S_FoeInfoSystem.Instance.CheckFoeMeetCondition();

        // 스탯, 히스토리를 스택의 카드를 내기 전으로 되돌리기.
        S_PlayerStat.Instance.ResetStatsByTwist();

        // 비틀기로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.Twist);

        // 피조물이 죽었다면 전투 종료
        if (S_FoeInfoSystem.Instance.CurrentFoe.CurrentHealth <= 0)
        {
            EndTrial();
        }
        else // 아니라면 히트 다시 시작
        {
            StartNewTurn();
        }
    }
    public async void StartStand()
    {
        GameFlowState = S_GameFlowStateEnum.Stand;

        // 스탠드 시 카쓸적 효과 발동
        await S_EffectActivator.Instance.ActivatedCardByStand();
        await S_EffectActivator.Instance.ActivateTrinketByStand();
        await S_FoeInfoSystem.Instance.ActivateStandFoeByStand();

        // 카드 및 적 체력 고정
        S_PlayerCard.Instance.FixCardsByStand();
        S_FoeInfoSystem.Instance.FixHealthByStand();

        // 스탠드로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.Stand);

        // 피조물 처치 여부에 따른 처리
        if (S_FoeInfoSystem.Instance.CurrentFoe.OldHealth <= 0) // 피조물이 죽었다면 전투 종료
        {
            EndTrial();
        }
        else // 살았다면 피조물이 플레이어를 공격
        {
            // 플레이어 공격
            await S_FoeInfoSystem.Instance.AttackPlayer();

            // 공격받고 나서 스택 합을 0으로 만들고 클린히트, 버스트 초기화하자.
            S_PlayerStat.Instance.ResetStackSum();
            S_PlayerStat.Instance.CheckBurstAndPerfect();
            S_StatInfoSystem.Instance.UpdateSpecialAbility();

            // 히트 다시 시작
            StartNewTurn();
        }
    }
    public async void EndTrial()
    {
        // 시련 종료 진행(아무것도 할 수 없음)
        GameFlowState = S_GameFlowStateEnum.None;

        // 최대 시련 저장
        if (!PlayerPrefs.HasKey("HighTrial"))
        {
            PlayerPrefs.SetInt("HighTrial", CurrentTrial);
            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.GetInt("HighTrial") < Instance.CurrentTrial)
            {
                PlayerPrefs.SetInt("HighTrial", Instance.CurrentTrial);
                PlayerPrefs.Save();
            }
        }

        // 스탯 초기화 전 골드 계산
        int gold = CalcResultGold();
        int health = S_PlayerStat.Instance.GetCurrentHealth();
        int determination = S_PlayerStat.Instance.GetCurrentDetermination();
        int omenCount = S_PlayerCard.Instance.GetDeckCards().Where(x => x.Engraving == S_EngravingEnum.Omen).Count();
        int greedCount = S_PlayerCard.Instance.GetDeckCards().Where(x => x.Engraving == S_EngravingEnum.Greed).Where(x => x.IsCursed).Count();
        greedCount += S_PlayerCard.Instance.GetStackCards().Where(x => x.Engraving == S_EngravingEnum.Greed).Where(x => x.IsCursed).Count();

        // UI 퇴장(적과 히트 버튼)
        S_FoeInfoSystem.Instance.DisappearUIFoe();
        S_FoeInfoSystem.Instance.DisappearFoeSprite();
        S_HitBtnSystem.Instance.DisappearHitBtn();

        // 스탯 초기화
        S_PlayerStat.Instance.ResetStatsByEndTrial();
        // 덱으로 모든 카드를 돌려보내기
        S_PlayerCard.Instance.ResetCardsByEndTrial();
        await S_StackInfoSystem.Instance.ResetCardsByEndTrialAsync();
        // 시련 종료 시 쓸만한 물건 업데이트
        S_PlayerTrinket.Instance.UpdateTrinketByEndTrial();

        // 보상 패널 등장
        S_ResultInfoSystem.Instance.AppearResult();
        S_ResultInfoSystem.Instance.AppearResultOKBtn();
        await Task.Delay(Mathf.RoundToInt(PANEL_APPEAR_TIME * 1500)); // 캔버스 등장 동안 대기

        // 골드 계산 VFX
        await S_ResultInfoSystem.Instance.CalcResultGoldAsync(health, determination, omenCount, greedCount);

        // 골드 획득
        S_PlayerStat.Instance.AddOrSubtractGold(gold);

        // 피조물 제거
        S_FoeInfoSystem.Instance.DestroyFoeByEndTrial();

        // 시련 종료로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.EndTrial);

        GameFlowState = S_GameFlowStateEnum.Store;
    }
    public async void StartStore()
    {
        GameFlowState = S_GameFlowStateEnum.None;

        S_ResultInfoSystem.Instance.DisappearResult();
        S_ResultInfoSystem.Instance.DisappearResultOKBtn();
        await Task.Delay(Mathf.RoundToInt(PANEL_APPEAR_TIME * 1000)); // 보상 캔버스 퇴장 대기

        Camera.main.transform.DOMove(StoreCameraPos, PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        Camera.main.transform.DORotate(S_GameFlowManager.StoreCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        await S_StoreInfoSystem.Instance.StartStore();

        GameFlowState = S_GameFlowStateEnum.Store;
    }



    public int GetCardOrderQueueCount()
    {
        return cardOrderQueue.Count;
    }
    public bool IsCurrentTurnHitted()
    {
        int count = S_PlayerCard.Instance.GetStackCards().Where(x => x.IsCurrentTurn).Count();
        return count > 0;
    }
    public int CalcResultGold()
    {
        // 처치 시 기본 골드
        int gold = SlayFoeGold;

        // 엘리트 적이라면 보너스 골드
        if (S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Clotho_Elite || 
            S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Lachesis_Elite || 
            S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Atropos_Elite)
        {
            gold += SlayEliteFoeGold; 
        }

        // 보스 적이라면 보너스 골드
        if (S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Clotho_Boss ||
            S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Lachesis_Boss ||
            S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.FoeType == S_FoeTypeEnum.Atropos_Boss)
        {
            gold += SlayBossFoeGold;
        }

        // 남은 체력과 의지 1당 골드
        gold += S_PlayerStat.Instance.GetCurrentHealth();
        gold += S_PlayerStat.Instance.GetCurrentDetermination();

        // 흉조, 탐욕 카드
        int omenCount = S_PlayerCard.Instance.GetDeckCards().Where(x => x.Engraving == S_EngravingEnum.Omen).Count();
        int greedCount = S_PlayerCard.Instance.GetDeckCards().Where(x => x.Engraving == S_EngravingEnum.Greed).Where(x => x.IsCursed).Count();
        greedCount += S_PlayerCard.Instance.GetStackCards().Where(x => x.Engraving == S_EngravingEnum.Greed).Where(x => x.IsCursed).Count();
        gold += (omenCount * 3) + (greedCount * 3);

        return gold;
    }
    public static async Task WaitPanelAppearTimeAsync()
    {
        await Task.Delay(Mathf.RoundToInt(PANEL_APPEAR_TIME * 1000));
    }
    public bool IsGameFlowState(S_GameFlowStateEnum state)
    {
        return GameFlowState == state;
    }
    public bool IsInState(List<S_GameFlowStateEnum> states)
    {
        return states.Contains(GameFlowState);
    }
}

public struct S_CardOrder // 카드오더큐에 사용되는 구조체
{
    public S_Card Card;
    public S_CardOrderTypeEnum Type;

    public S_CardOrder(S_Card card, S_CardOrderTypeEnum type)
    {
        Card = card;
        Type = type;
    }
}

public enum S_CardOrderTypeEnum // 카드 오더 큐에 사용되는 열거형
{
    Hit, 
    Gen, 
    Exclusion
}

public enum S_GameFlowStateEnum
{
    None,
    Dialog,
    Hit,
    HittingCard,
    Deck,
    Twist,
    Stand,
    Store,
    StoreBuying,
    GameMenu,
    GameOver
}
