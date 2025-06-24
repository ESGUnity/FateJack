using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        // 말하는 캐릭터의 소팅오더를 보이도록 조절
        SpriteRenderer dialogPos = S_FoeInfoSystem.Instance.Instance_Section.GetComponent<S_SectionObj>().sprite_Character.GetComponent<SpriteRenderer>();
        dialogPos.sortingLayerName = "Dialog";
        dialogPos.sortingOrder = 1;

        // 다이얼로그 : 튜토리얼 인트로
        await StartTutorialDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Intro"), dialogPos);

        // 다이얼로그 : 튜토리얼 카드 내기
        await StartTutorialDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Hit"), dialogPos);

        // 카드 내기 시작
        await S_HitBtnSystem.Instance.StartHitCardAsync();
        // 온전히 카드를 낼 때까지 대기
        while (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.HittingCard) || S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.None))
        {
            await Task.Yield();
        }

        // 다이얼로그 : 튜토리얼 카드에 대하여
        await StartTutorialDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Card"), dialogPos);

        // 덱 보기
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;
        S_HitBtnSystem.Instance.DisappearHitBtn();
        await S_DeckInfoSystem.Instance.OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Hit);
        // 덱에서 스택으로 넘어올 때까지 대기
        while (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Deck) || S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.None))
        {
            await Task.Yield();
        }

        // 다이얼로그 : 튜토리얼 비틀기
        await StartTutorialDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Twist"), dialogPos);

        // 되돌리기 시작
        await S_GameFlowManager.Instance.StartTwistAsync();

        // 다이얼로그 : 튜토리얼 스탠드
        await StartTutorialDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Stand"), dialogPos);

        // 다이얼로그 : 튜토리얼 쓸만한 물건
        await StartTutorialDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Trinket"), dialogPos);

        // 다이얼로그 : 튜토리얼 아웃트로
        await StartTutorialDialog(S_DialogMetaData.GetDialogsByPrefix("Tutorial_Outro"), dialogPos);

        // 튜토리얼 완료
        // 캐릭터 소팅 재조절
        dialogPos.sortingLayerName = "WorldObject";
        dialogPos.sortingOrder = 1;

        PlayerPrefs.SetInt("TutorialCompleted", 1); // 튜토리얼 완수했음을 알리는 로직
        PlayerPrefs.Save();

        // 상점으로 넘어가기
        await S_GameFlowManager.Instance.StartStoreByTutorialAsync();
    }

    async Task StartTutorialDialog(List<DialogData> dialogList, SpriteRenderer dialogPos)
    {
        Queue<DialogData> dialogs = new Queue<DialogData>();

        for (int i = 0; i < dialogList.Count; i++)
        {
            dialogs.Enqueue(dialogList[i]);
        }

        await S_DialogInfoSystem.Instance.StartDialog(dialogPos, dialogs);
    }
}
