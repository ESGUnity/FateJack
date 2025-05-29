using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // ������Ʈ
    PlayerController ownerPlayer; // ���� �÷��̾� ��Ʈ�ѷ�

    // �غ� ����
    TMP_Text readyBtnText;

    // ���� ���� ����
    GameObject startGameBtn;

    // �������ڵ� ����
    TMP_Text joinCodeText;
    GameObject copyJoinCodeBtn;

    // �α� ����
    [SerializeField] GameObject logPrefab;
    Vector2 logPos = new Vector2(0, -233);

    async void Start()
    {
        TMP_Text[] tmps = GetComponentsInChildren<TMP_Text>(true);
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        // �غ� ����
        readyBtnText = Array.Find(tmps, c => c.gameObject.name.Equals("ReadyBtnText"));

        // ���� ���� ����
        startGameBtn = Array.Find(transforms, c => c.gameObject.name.Equals("StartGameBtn")).gameObject;

        // �������ڵ� ����
        joinCodeText = Array.Find(tmps, c => c.gameObject.name.Equals("JoinCodeText"));
        copyJoinCodeBtn = Array.Find(transforms, c => c.gameObject.name.Equals("CopyJoinCodeBtn")).gameObject;

        // ȣ��Ʈ�� ���(������ �ڵ� �� ���ӽ��� ��ư ����)
        if (NetworkManager.Singleton.IsServer)
        {
            if (!startGameBtn.activeInHierarchy)
            {
                startGameBtn.SetActive(true);
            }

            joinCodeText.gameObject.SetActive(true);
            joinCodeText.text = $"JoinCode : {HostSingleton.Instance.JoinCode}"; // ������ �ڵ� 
            copyJoinCodeBtn.SetActive(true);
        }

        await AllocateOwnerPlayerAsync();

        // �غ� ��ư �ʱ�ȭ
        if (ownerPlayer.IsThisPlayerReady)
        {
            readyBtnText.text = "�غ� ���";
        }
        else
        {
            readyBtnText.text = "�غ�";
        }

        GameFlowManager.Instance.InGameStart += CloseLobbyUI;
    }

    async Task AllocateOwnerPlayerAsync() // PlayerController�� LocalInstance�� �Ҵ�� ������ ��ٸ���
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
        CreateLog("�� ���� �ڵ� ���� �Ϸ�!");
    }
    public void PressReadyBtn() // �غ� ��ư ���� �� �޼���
    {
        if (ownerPlayer.IsThisPlayerReady)
        {
            readyBtnText.text = "�غ�";
            ownerPlayer.IsThisPlayerReady = false;
            GameFlowManager.Instance.NotifyPlayerReadyServerRpc();
        }
        else
        {
            readyBtnText.text = "�غ� ���";
            ownerPlayer.IsThisPlayerReady = true;
            GameFlowManager.Instance.NotifyPlayerReadyServerRpc();
        }
    }
    public void PressStartGameBtn() // ���� ���� ��ư ���� �� �޼���
    {
        GameFlowManager.Instance.RequestStartGameServerRpc();
    }
    void CreateLog(string log) // ���� ������ �˸��� �α� ������ �޼���
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
