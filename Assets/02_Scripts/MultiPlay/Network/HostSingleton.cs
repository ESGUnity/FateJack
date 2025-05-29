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

//TODO : ȣ��Ʈ �̱����̳� ���� �̱��Ͽ� ������ Ŭ���̾�Ʈ ������ ������ �ְ� ���� �ƴϸ� �׳� ������� ����.. ��� ��.. �켱 UserData�ؼ� Ŭ���� ������ �غ���(���߿�~~~~~~~)

public class HostSingleton : MonoBehaviour
{
    const int MAXCONNECTIONS = 4;
    Allocation allocation;
    public string JoinCode;
    string lobbyId;

    // �̱���
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
    public async Task StartHostAsync(LobbyTypeEnum lobbyType, string lobbyName) // ȣ��Ʈ�μ� ������ ���� �� �����ϴ� �޼���
    {
        // ������ ������ �� ������ ����� ������ �Ҵ��ϰ� ����Ƽ Ʈ������Ʈ�� ���� �����ϱ�
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MAXCONNECTIONS); // ������ ������ ���ο� ���� ����. Allocation Ÿ�Կ��� ������ Ip, Port, JoinCode ���� ���Ե�
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); // �� ���� �ڵ� ����
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Relay ���� ���� : {ex.Message}");
            return;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Relay ���� ��û ����: {ex.Message}");
            return;
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"���� ����: {ex.Message}. Unity �α����� �ʿ��� �� ����.");
            return;
        }
        catch (TimeoutException ex)
        {
            Debug.LogError($"��Ʈ��ũ ���� �߻�: {ex.Message}");
            return;
        }
        catch (Exception ex)
        {
            Debug.LogError($"�� �� ���� ���� �߻�: {ex.Message}");
            return;
        }

        RelayServerData relayServerData = allocation.ToRelayServerData("dtls"); // ������ ������ �Ҵ�� ������ ������ ���� �����ͷ� ��ȯ�ϱ�
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData); // Ʈ������Ʈ�� �� Ŭ���̾�Ʈ�� �����ų �� �ʿ��� ����(IP,Port ��)�� �����ϱ�

        // �κ� �����
        try
        {
            // �κ� �ɼ� �����ϴ� �κ�
            CreateLobbyOptions options = new();
            if (lobbyType == LobbyTypeEnum.Public) // ��� Ȥ�� ������ ����
            {
                options.IsPrivate = false; 
            }
            else
            {
                options.IsPrivate = true; 
            }
            options.Data = new Dictionary<string, DataObject> 
            { 
                { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, JoinCode) } // VisibilityOptions�� �����Ͱ� �����Ǵ� �����̸� Member�� �����ϸ� �κ� �����ڸ� �� �� �ִ� ���̴�.
            };

            // �κ� �����ϴ� �κ�
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAXCONNECTIONS, options);
            lobbyId = lobby.Id;

            StartCoroutine(HeartbeatLobby()); // ���� �κ� �������ų� ���� ���� �� Stop�� �� �ؾ���
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            return;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.Log("�� �� ���� ����");
            return;
        }

        // ȣ��Ʈ�� ���� �����ϱ�
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
