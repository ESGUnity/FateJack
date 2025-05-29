using DG.Tweening;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // 각종 플레이어 관련 스크립트
    [HideInInspector] public PlayerCards playerCards;
    [HideInInspector] public PlayerStats playerStats;

    // 로비 관련
    [HideInInspector] public bool IsThisPlayerReady;
    Vector3 lobbyPos = new Vector3(0, 49.33f, -1.46f);
    Vector3 lobbyRot = new Vector3(90, 0, 0);
    Vector3 mainGamePos = new Vector3(0, 47.5f, -23.29f);
    Vector3 mainGameRot = new Vector3(65, 0, 0);
    const float CAMERA_MOVE_LOBBY_TO_INGAME_TIME = 3f;

    // 카드 및 각종 효과의 시각 연출 관련
    [HideInInspector] public NetworkVariable<FixedString512Bytes> NickName = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // 닉네임
    [HideInInspector] public NetworkVariable<Vector3> PlayerCharacterPos = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // 플레이어 패널에서의 캐릭터 아이콘 위치
    [HideInInspector] public NetworkVariable<Vector3> TossCardPos = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // 카드를 받는 위치

    // 간이 싱글턴
    [HideInInspector] public static PlayerController LocalInstance { get; private set; }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;

            // 플레이어 이름 가져오기
            NickName.Value = AuthenticationService.Instance.PlayerName.IndexOf('#') >= 0 ? AuthenticationService.Instance.PlayerName.Substring(0, AuthenticationService.Instance.PlayerName.IndexOf('#')) : AuthenticationService.Instance.PlayerName ?? "Unkwnown";

            // 시작 전 로비의 카메라 위치 설정
            Camera.main.transform.position = lobbyPos;
            Camera.main.transform.eulerAngles = lobbyRot;

            // 시작 전 준비 상태
            IsThisPlayerReady = false;
        }
    }
    void Start()
    {
        if (IsOwner)
        {
            // 세션 상에서 초기화 하기
            NotifyConnectAndInit();
        }

        // 컴포넌트 할당(NetworkSpawn에 있으면 다른 애들 들어올때마다 실행시키니까 안됨)
        playerCards = GetComponent<PlayerCards>();
        playerStats = GetComponent<PlayerStats>();
    }

    #region 게임 시작 전
    async void NotifyConnectAndInit() // 각 컴포넌트에 클라이언트 정보 보내어 세션 상에서 초기화하기
    {
        if (IsOwner)
        {
            // CoreGameManager가 생길 때까지 대기
            while (CoreGameManager.Instance == null)
            {
                await Task.Delay(500);
            }

            // CoreGameManager에 이 클라이언트의 정보를 초기화
            if (CoreGameManager.Instance != null)
            {
                CoreGameManager.Instance.InitPlayerServerRpc();
            }
        }
    }

    [ClientRpc]
    public void MoveCameraIntroClientRpc() // 게임 시작 시 카메라 위치를 이동시키는 메서드
    {
        if (IsOwner)
        {
            Camera.main.transform.DOMove(mainGamePos, CAMERA_MOVE_LOBBY_TO_INGAME_TIME).SetEase(Ease.OutQuart);
            Camera.main.transform.DORotate(mainGameRot, CAMERA_MOVE_LOBBY_TO_INGAME_TIME).SetEase(Ease.OutQuart);
        }
    }
    #endregion
    #region 시각 연출 관련
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
    #region 히트 및 스탠드 관련

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
