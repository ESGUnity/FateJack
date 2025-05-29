using DG.Tweening;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // ���� �÷��̾� ���� ��ũ��Ʈ
    [HideInInspector] public PlayerCards playerCards;
    [HideInInspector] public PlayerStats playerStats;

    // �κ� ����
    [HideInInspector] public bool IsThisPlayerReady;
    Vector3 lobbyPos = new Vector3(0, 49.33f, -1.46f);
    Vector3 lobbyRot = new Vector3(90, 0, 0);
    Vector3 mainGamePos = new Vector3(0, 47.5f, -23.29f);
    Vector3 mainGameRot = new Vector3(65, 0, 0);
    const float CAMERA_MOVE_LOBBY_TO_INGAME_TIME = 3f;

    // ī�� �� ���� ȿ���� �ð� ���� ����
    [HideInInspector] public NetworkVariable<FixedString512Bytes> NickName = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // �г���
    [HideInInspector] public NetworkVariable<Vector3> PlayerCharacterPos = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // �÷��̾� �гο����� ĳ���� ������ ��ġ
    [HideInInspector] public NetworkVariable<Vector3> TossCardPos = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // ī�带 �޴� ��ġ

    // ���� �̱���
    [HideInInspector] public static PlayerController LocalInstance { get; private set; }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;

            // �÷��̾� �̸� ��������
            NickName.Value = AuthenticationService.Instance.PlayerName.IndexOf('#') >= 0 ? AuthenticationService.Instance.PlayerName.Substring(0, AuthenticationService.Instance.PlayerName.IndexOf('#')) : AuthenticationService.Instance.PlayerName ?? "Unkwnown";

            // ���� �� �κ��� ī�޶� ��ġ ����
            Camera.main.transform.position = lobbyPos;
            Camera.main.transform.eulerAngles = lobbyRot;

            // ���� �� �غ� ����
            IsThisPlayerReady = false;
        }
    }
    void Start()
    {
        if (IsOwner)
        {
            // ���� �󿡼� �ʱ�ȭ �ϱ�
            NotifyConnectAndInit();
        }

        // ������Ʈ �Ҵ�(NetworkSpawn�� ������ �ٸ� �ֵ� ���ö����� �����Ű�ϱ� �ȵ�)
        playerCards = GetComponent<PlayerCards>();
        playerStats = GetComponent<PlayerStats>();
    }

    #region ���� ���� ��
    async void NotifyConnectAndInit() // �� ������Ʈ�� Ŭ���̾�Ʈ ���� ������ ���� �󿡼� �ʱ�ȭ�ϱ�
    {
        if (IsOwner)
        {
            // CoreGameManager�� ���� ������ ���
            while (CoreGameManager.Instance == null)
            {
                await Task.Delay(500);
            }

            // CoreGameManager�� �� Ŭ���̾�Ʈ�� ������ �ʱ�ȭ
            if (CoreGameManager.Instance != null)
            {
                CoreGameManager.Instance.InitPlayerServerRpc();
            }
        }
    }

    [ClientRpc]
    public void MoveCameraIntroClientRpc() // ���� ���� �� ī�޶� ��ġ�� �̵���Ű�� �޼���
    {
        if (IsOwner)
        {
            Camera.main.transform.DOMove(mainGamePos, CAMERA_MOVE_LOBBY_TO_INGAME_TIME).SetEase(Ease.OutQuart);
            Camera.main.transform.DORotate(mainGameRot, CAMERA_MOVE_LOBBY_TO_INGAME_TIME).SetEase(Ease.OutQuart);
        }
    }
    #endregion
    #region �ð� ���� ����
    [ClientRpc]
    public void SetPlayerCharacterPosClientRpc(Vector3 pos)
    {
        if (IsOwner)
        {
            PlayerCharacterPos.Value = pos;
        }
    }
    [ClientRpc]
    public void SetTossCardPosClientRpc(Vector3 pos)
    {
        if (IsOwner)
        {
            TossCardPos.Value = pos;
        }
    }
    #endregion
    #region ��Ʈ �� ���ĵ� ����

    #endregion
    private void ChoosePresent(RoundRewardEnum present)
    {
        switch (present)
        {
            case RoundRewardEnum.ClothoPresent:
                break;
            case RoundRewardEnum.LachesisPresent:
                break;
            case RoundRewardEnum.AtroposPresent:
                break;
        }
    }
}
