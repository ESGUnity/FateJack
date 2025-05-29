using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class S_TutorialManager : MonoBehaviour
{
    // 프리팹
    [SerializeField] List<S_TutorialBase> tutorialList;

    // 다이얼로그 용 변수
    public bool IsCompleteDialog;

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

    public void StartTutorial()
    {
        StartCoroutine(TutorialCoroutine());
    }

    IEnumerator TutorialCoroutine()
    {
        ////////////////////////////////////////////////////////////////////////////////// StartTrial

        // 세계관 설명 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.ExplainUniverse, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 피조물 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Creature, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 스탯 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Stat, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 덱 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Deck, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 덱 넘어갈 시간동안 대기
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 1.5f);

        // 카드 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.CardInfo, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 덱을 다 둘러보고 덱을 끌 때까지 대기
        while (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {
            yield return null;
        }

        // 전리품 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Loot, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 히트 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Hit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 히트 될 때까지 좀만 대기해주기
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 4f);

        // 히트 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(2, S_DialogStateEnum.Hit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 의지 히트 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.DeterminationHit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 덱으로 넘어갈 때까지 좀만 대기해주기
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 1f);

        // 의지 히트 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(2, S_DialogStateEnum.DeterminationHit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 의지 히트를 하거나 취소할 때까지 대기
        while (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {
            yield return null;
        }

        // 비틀기 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Twist, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 비틀기 연출 끝날 때가지 대기
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 1f);

        // 비틀기 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(2, S_DialogStateEnum.Twist, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 스탠드 정보 다이얼로그 시작
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Stand, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // 튜토리얼 완료
        CompletedAllTutorial();
    }

    public void CompletedAllTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1); // 튜토리얼 완수했음을 알리는 로직
        PlayerPrefs.Save();
    }
}
