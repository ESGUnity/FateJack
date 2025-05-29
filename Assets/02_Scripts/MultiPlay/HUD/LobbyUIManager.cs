using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // 컴포넌트
    PlayerController ownerPlayer; // 로컬 플레이어 컨트롤러

    // 준비 관련
    TMP_Text readyBtnText;

    // 게임 시작 관련
    GameObject startGameBtn;

    // 방참가코드 관련
    TMP_Text joinCodeText;
    GameObject copyJoinCodeBtn;

    // 로그 관련
    [SerializeField] GameObject logPrefab;
    Vector2 logPos = new Vector2(0, -233);

    async void Start()
    {
        TMP_Text[] tmps = GetComponentsInChildren<TMP_Text>(true);
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        // 준비 관련
        readyBtnText = Array.Find(tmps, c => c.gameObject.name.Equals("ReadyBtnText"));

        // 게임 시작 관련
        startGameBtn = Array.Find(transforms, c => c.gameObject.name.Equals("StartGameBtn")).gameObject;

        // 방참가코드 관련
        joinCodeText = Array.Find(tmps, c => c.gameObject.name.Equals("JoinCodeText"));
        copyJoinCodeBtn = Array.Find(transforms, c => c.gameObject.name.Equals("CopyJoinCodeBtn")).gameObject;

        // 호스트일 경우(방참가 코드 및 게임시작 버튼 띄우기)
        if (NetworkManager.Singleton.IsServer)
        {
            if (!startGameBtn.activeInHierarchy)
            {
                startGameBtn.SetActive(true);
            }

            joinCodeText.gameObject.SetActive(true);
            joinCodeText.text = $"JoinCode : {HostSingleton.Instance.JoinCode}"; // 방참가 코드 
            copyJoinCodeBtn.SetActive(true);
        }

        await AllocateOwnerPlayerAsync();

        // 준비 버튼 초기화
        if (ownerPlayer.IsThisPlayerReady)
        {
            readyBtnText.text = "준비 취소";
        }
        else
        {
            readyBtnText.text = "준비";
        }

        GameFlowManager.Instance.InGameStart += CloseLobbyUI;
    }

    async Task AllocateOwnerPlayerAsync() // PlayerController의 LocalInstance가 할당될 때까지 기다리기
    {
        while (PlayerController.LocalInstance == null)
        {
            await Task.Delay(500);
        }

        ownerPlayer = PlayerController.LocalInstance;
    }
    public void PressCopyJoinCodeBtn()
    {
        GUIUtility.systemCopyBuffer = HostSingleton.Instance.JoinCode;
        CreateLog("방 참가 코드 복사 완료!");
    }
    public void PressReadyBtn() // 준비 버튼 누를 때 메서드
    {
        if (ownerPlayer.IsThisPlayerReady)
        {
            readyBtnText.text = "준비";
            ownerPlayer.IsThisPlayerReady = false;
            GameFlowManager.Instance.NotifyPlayerReadyServerRpc();
        }
        else
        {
            readyBtnText.text = "준비 취소";
            ownerPlayer.IsThisPlayerReady = true;
            GameFlowManager.Instance.NotifyPlayerReadyServerRpc();
        }
    }
    public void PressStartGameBtn() // 게임 시작 버튼 누를 때 메서드
    {
        GameFlowManager.Instance.RequestStartGameServerRpc();
    }
    void CreateLog(string log) // 게임 정보를 알리는 로그 날리는 메서드
    {
        GameObject go = Instantiate(logPrefab, transform);

        TMP_Text logText = go.GetComponent<TMP_Text>();
        logText.raycastTarget = false;
        logText.text = log;

        logText.GetComponent<RectTransform>().anchoredPosition = logPos;

        Sequence logSequence = DOTween.Sequence();
        logSequence.Append(logText.GetComponent<RectTransform>().DOAnchorPos(logPos + new Vector2(0, 70), 2.5f))
           .Insert(1f, logText.DOFade(0f, 1.5f));
    }
    void CloseLobbyUI()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        GameFlowManager.Instance.InGameStart -= CloseLobbyUI;
    }
}
