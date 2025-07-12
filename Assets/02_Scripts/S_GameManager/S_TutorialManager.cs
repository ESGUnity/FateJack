using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class S_TutorialManager : MonoBehaviour
{
    // 싱글턴
    static S_TutorialManager instance;
    public static S_TutorialManager Instance { get { return instance; } }

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

    public async void StartTrialByTutorial()
    {
        Queue<DialogData> dialogs = new Queue<DialogData>();
        List<DialogData> dialogList = new();

        // 시련 시작
        await S_GameFlowManager.Instance.StartTrialByTutorialAsync();

        // 다이얼로그 : 튜토리얼 인트로
        await S_DialogInfoSystem.Instance.StartQueueDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Intro"));

        // 다이얼로그 : 튜토리얼 카드 내기
        await S_DialogInfoSystem.Instance.StartQueueDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Hit"));

        // 카드 내기 시작
        S_HitBtnSystem.Instance.HitCard();
        // 온전히 카드를 낼 때까지 대기
        while (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.HittingCard) || S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.None))
        {
            await Task.Yield();
        }

        // 다이얼로그 : 튜토리얼 카드에 대하여
        await S_DialogInfoSystem.Instance.StartQueueDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Card"));

        // 덱 보기
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;
        S_HitBtnSystem.Instance.DisappearHitBtn();
        await S_DeckInfoSystem.Instance.OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Hit);
        // 덱에서 스택으로 넘어올 때까지 대기
        while (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Deck) || S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.None))
        {
            await Task.Yield();
        }

        // 다이얼로그 : 튜토리얼 스탠드
        await S_DialogInfoSystem.Instance.StartQueueDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Stand"));

        // 다이얼로그 : 튜토리얼 아웃트로
        await S_DialogInfoSystem.Instance.StartQueueDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Outro"));

        // 튜토리얼 완료
        PlayerPrefs.SetInt("TutorialCompleted", 1); // 튜토리얼 완수했음을 알리는 로직
        PlayerPrefs.Save();

        // 상점으로 넘어가기
        await S_GameFlowManager.Instance.StartRewardByTutorialAsync();

        // 다이얼로그 : 튜토리얼 상점
        await S_DialogInfoSystem.Instance.StartQueueDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Reward"));
    }
}
