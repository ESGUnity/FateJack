using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

//TODO : 호스트 싱글턴이나 서버 싱글턴에 접속한 클라이언트 정보를 가지고 있게 할지 아니면 그냥 상관없이 할지.. 고민 중.. 우선 UserData해서 클라우드 관리는 해보자(나중에~~~~~~~)

public class HostSingleton : MonoBehaviour
{
    const int MAXCONNECTIONS = 4;
    Allocation allocation;
    public string JoinCode;
    string lobbyId;

    // 싱글턴
    static HostSingleton instance;
    public static HostSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singletonObject = new GameObject("HostSingleton");
                instance = singletonObject.AddComponent<HostSingleton>();

                DontDestroyOnLoad(singletonObject);
            }

            return instance;
        }
    }
    public void Init()
    {

    }
    public async Task StartHostAsync(LobbyTypeEnum lobbyType, string lobbyName) // 호스트로서 서버에 접속 시 실행하는 메서드
    {
        // 릴레이 서버에 이 게임이 사용할 세션을 할당하고 유니티 트랜스포트를 통해 연결하기
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MAXCONNECTIONS); // 릴레이 서버에 새로운 세션 생성. Allocation 타입에는 세션의 Ip, Port, JoinCode 등이 포함됨
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); // 방 참가 코드 생성
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Relay 서버 오류 : {ex.Message}");
            return;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Relay 서버 요청 실패: {ex.Message}");
            return;
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"인증 오류: {ex.Message}. Unity 로그인이 필요할 수 있음.");
            return;
        }
        catch (TimeoutException ex)
        {
            Debug.LogError($"네트워크 지연 발생: {ex.Message}");
            return;
        }
        catch (Exception ex)
        {
            Debug.LogError($"알 수 없는 오류 발생: {ex.Message}");
            return;
        }

        RelayServerData relayServerData = allocation.ToRelayServerData("dtls"); // 릴레이 서버에 할당된 세션을 릴레이 서버 데이터로 변환하기
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData); // 트랜스포트가 각 클라이언트를 연결시킬 때 필요한 정보(IP,Port 등)를 설정하기

        // 로비 만들기
        try
        {
            // 로비 옵션 설정하는 부분
            CreateLobbyOptions options = new();
            if (lobbyType == LobbyTypeEnum.Public) // 비방 혹은 공개방 여부
            {
                options.IsPrivate = false; 
            }
            else
            {
                options.IsPrivate = true; 
            }
            options.Data = new Dictionary<string, DataObject> 
            { 
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, JoinCode) } // VisibilityOptions는 데이터가 공개되는 범위이며 Member로 설정하면 로비 참가자만 볼 수 있는 것이다.
            };

            // 로비를 생성하는 부분
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAXCONNECTIONS, options);
            lobbyId = lobby.Id;

            StartCoroutine(HeartbeatLobby()); // 추후 로비가 없어지거나 세션 만료 시 Stop도 꼭 해야함
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            return;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.Log("알 수 없는 오류");
            return;
        }

        // 호스트로 씬에 접속하기
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    IEnumerator HeartbeatLobby()
    {
        var delay = new WaitForSecondsRealtime(15);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
