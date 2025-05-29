using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    JoinAllocation allocation;

    static ClientSingleton instance;

    public static ClientSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singletonObject = new GameObject("ClientSingleton");
                instance = singletonObject.AddComponent<ClientSingleton>();

                DontDestroyOnLoad(singletonObject);
            }

            return instance;
        }
    }

    public void Init()
    {

    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException ex)
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

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
        transport.SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
    }
}
