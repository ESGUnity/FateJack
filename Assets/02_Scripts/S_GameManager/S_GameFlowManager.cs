using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_GameFlowManager : MonoBehaviour
{
    [Header("시련 및 턴 관련")]
    [HideInInspector] public int CurrentTrial;
    [HideInInspector] public int CurrentTurn;
    [HideInInspector] public S_GameFlowStateEnum GameFlowState { get; set; }

    [Header("연출 관련")]
    public const float PANEL_APPEAR_TIME = 0.5f;

    [Header("보상 관련")]
    [HideInInspector] public int SlayFoeGold = 5;
    [HideInInspector] public int SlayEliteFoeGold = 3;
    [HideInInspector] public int SlayBossFoeGold = 10;
    [HideInInspector] public int RemainHealthPerGold = 1;
    [HideInInspector] public int RemainDeterminationPerGold = 1;

    [Header("다이얼로그 관련")]
    [HideInInspector] public bool IsCompleteDialog;

    // 싱글턴
    static S_GameFlowManager instance;
    public static S_GameFlowManager Instance { get { return instance; } }

    void Awake()
    {
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
    async void Start()
    {
        GameFlowState = S_GameFlowStateEnum.None;
        CurrentTrial = 0;

        // 시련 시작 시 설정 TODO : 추후 저장기능 만들면 여기를 대체
        S_PlayerCard.Instance.InitDeckByStartGame(); // 덱 초기화
        S_PlayerStat.Instance.InitStatsByStartGame(); // 스탯 초기화

        if (!PlayerPrefs.HasKey("TutorialCompleted")) // 이 게임이 처음이라면
        {
            PlayerPrefs.SetInt("TutorialCompleted", 0);
            PlayerPrefs.Save();

            S_TutorialManager.Instance.StartTrialByTutorial();
        }
        else if (PlayerPrefs.GetInt("TutorialCompleted") == 0) // 튜토리얼 시작이거나 튜토리얼을 완료하지 못한 경우
        {
            S_TutorialManager.Instance.StartTrialByTutorial();
        }
        else // 그것도 아닌 경우
        {
            await StartRewardAsync();
        }
    }

    #region 시련 단계에 따른 메서드
    // 시련 시작 시
    public async void StartTrial()
    {
        /////////////////////////데모판 코드//////////////////////////
        //if (CurrentTrial == 4)
        //{
        //    S_DemoSystem.Instance.AppearDemoPanel();
        //    return;
        //}

        await StartTrialAsync();
    }
    public async Task StartTrialAsync()
    {
        GameFlowState = S_GameFlowStateEnum.None;

        // 시련 및 턴 설정
        CurrentTrial++;
        S_StatInfoSystem.Instance.ChangeCurrentTrialText();
        CurrentTurn = 0;

        // 시련 시작 시 카드 업데이트
        await S_PlayerCard.Instance.UpdateCardsByStartTrial();
        //await S_FoeInfoSystem.Instance.UpdateFoeByStartTrial();

        // 시련 시작 시로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.StartTrial);

        // 턴 시작
        StartNewTurn();
    }
    public async Task StartTrialByTutorialAsync()
    {
        GameFlowState = S_GameFlowStateEnum.None;

        // 튜토리얼은 시련이 0이다. 그리고 턴 설정
        CurrentTrial = 0;
        S_StatInfoSystem.Instance.ChangeCurrentTrialText();
        CurrentTurn = 0;

        // 시련 시작 시 카쓸적 업데이트
        await S_PlayerCard.Instance.UpdateCardsByStartTrial();
        //await S_FoeInfoSystem.Instance.UpdateFoeByStartTrial();

        // 시련 시작 시로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.StartTrial);

        // 턴 시작
        StartNewTurn();
    }
    // 턴 시작 시
    public void StartNewTurn()
    {
        // 턴 관련
        CurrentTurn++;
        S_InGameUISystem.Instance.AppearCurrentTurnUI();

        // 히트 버튼 등장
        S_HitBtnSystem.Instance.AppearHitBtn();

        // 나중에 여기 적 대사 치는 거 들어올 수는 있을듯

        GameFlowState = S_GameFlowStateEnum.Hit;
    }
    // 히트 시
    public async Task StartHitCardAsync(S_CardBase card) // 카드를 낼 때와 시련 시작 시 속전속결 카드를 낼 때 사용
    {
        GameFlowState = S_GameFlowStateEnum.HittingCard;

        // 게임의 히스토리 업데이트
        S_PlayerStat.Instance.UpdateHistory(card);

        // 카드 내기
        await S_PlayerCard.Instance.HitCard(card);

        // 카드를 냈을 때의 효과 발동
        await S_EffectActivator.Instance.ActivateByHit(card);

        // 카드 오브제 업데이트
        S_PlayerCard.Instance.UpdateCardObjsState();

        // 카드에 의한 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(card, S_StatHistoryTriggerEnum.Card);

        // Hitting -> Hit로 변환
        GameFlowState = S_GameFlowStateEnum.Hit;

        // 적 체력 체크
        if (S_FoeInfoSystem.Instance.CurrentHealth <= 0)
        {
            EndTrial();
        }
    }
    // 스탠드 시
    public async void StartStand()
    {
        await StartStandAsync();
    }
    public async Task StartStandAsync()
    {
        GameFlowState = S_GameFlowStateEnum.Stand;

        // 모든 발현 효과 발동(Overload까지)
        await S_EffectActivator.Instance.ActivateFieldCardsByStand();

        // 스탠드로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.Stand);

        // 적 체력이 0이하라면 시련 종료
        if (S_FoeInfoSystem.Instance.CurrentHealth <= 0) 
        {
            EndTrial();
            return;
        }

        // 시련 종료가 안 되었으면 적 카드 발동
        await S_EffectActivator.Instance.ActivateFoeCardsByStand();

        // 사용한 카드 더미로 카드 보내기(Fix처리와 Fix관련 지속 모두), 무게 초기화, 버스트 및 완벽 체크
        S_PlayerStat.Instance.ResetCurrentWeight();
        S_PlayerStat.Instance.CheckBurstAndPerfect();
        await S_PlayerCard.Instance.UpdateCardsByStand();

        // 히트 다시 시작
        StartNewTurn();
    }
    // 시련 종료 시
    public async void EndTrial() // 카드 사라짐, 내 카드 초기화, 스탯 초기화, 
    {
        // 시련 종료 진행
        GameFlowState = S_GameFlowStateEnum.None;

        // 최대 시련 저장
        if (!PlayerPrefs.HasKey("HighTrial"))
        {
            PlayerPrefs.SetInt("HighTrial", CurrentTrial);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetInt("HighTrial") < Instance.CurrentTrial)
        {
            PlayerPrefs.SetInt("HighTrial", Instance.CurrentTrial);
            PlayerPrefs.Save();
        }

        // 적 카드 제거
        await S_FoeInfoSystem.Instance.ResetFoeCardsByEndTrial(); 

        // 히트 버튼 퇴장
        S_HitBtnSystem.Instance.DisappearHitBtn(); 

        // 내 능력치와 카드 초기화
        S_PlayerStat.Instance.ResetStatsByEndTrial();
        await S_PlayerCard.Instance.ResetCardsByEndTrial();

        // 시련 종료로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.EndTrial);

        // 보상 시작
        await StartRewardAsync();
    }
    // 보상 시작 시
    public async Task StartRewardAsync() // 적 카드 등장, 체력 채우기, 보상 떨구기
    {
        GameFlowState = S_GameFlowStateEnum.None;

        // 히트 버튼 퇴장
        S_HitBtnSystem.Instance.DisappearHitBtn();

        // 적 카드 등장, 체력 채우기
        await S_FoeInfoSystem.Instance.UpdateCardsByRewardTime();

        // 보상 시작
        await S_RewardInfoSystem.Instance.StartReward();

        GameFlowState = S_GameFlowStateEnum.Store;
    }
    public async Task StartRewardByTutorialAsync()
    {
        GameFlowState = S_GameFlowStateEnum.None;

        // 적 카드 제거
        await S_FoeInfoSystem.Instance.ResetFoeCardsByEndTrial();

        // 히트 버튼 퇴장
        S_HitBtnSystem.Instance.DisappearHitBtn();

        // 내 능력치와 카드 초기화
        S_PlayerStat.Instance.ResetStatsByEndTrial();
        await S_PlayerCard.Instance.ResetCardsByEndTrial();

        // 시련 종료로 히스토리 저장
        S_PlayerStat.Instance.SaveStatHistory(null, S_StatHistoryTriggerEnum.EndTrial);

        // 적 카드 등장, 체력 채우기
        await S_FoeInfoSystem.Instance.UpdateCardsByRewardTime();

        // 튜토리얼 보상 시작
        await S_RewardInfoSystem.Instance.StartRewardByTutorial();

        GameFlowState = S_GameFlowStateEnum.Store;
    }
    #endregion
    #region 보조
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
    #endregion
}

public enum S_GameFlowStateEnum
{
    None,
    Dialog,
    Hit,
    HittingCard,
    Deck,
    Used,
    Twist,
    Stand,
    Store,
    StoreBuying,
    GameMenu,
    GameOver
}
