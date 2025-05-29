using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// ȿ�� Ÿ��
public enum EffectTypeEnum
{
    LoseDamage, // �й� �� ������
    StackSum,
    Damage,
    Heal,
    GetGold,
    GetDamageMP,
    StealDamageMP,
}

// CoreGameManager�� ���� ī�� ȿ�� Task ����ü
public struct CardEffectTask : INetworkSerializable
{
    public EffectTypeEnum EffectType;
    public ulong InvokerId;
    public ulong[] TargetId;
    public int Value;

    public CardEffectTask(EffectTypeEnum effectType, ulong invokerId, ulong[] targetId, int value)
    {
        EffectType = effectType;
        InvokerId = invokerId;
        TargetId = targetId;
        Value = value;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref EffectType);
        serializer.SerializeValue(ref InvokerId);
        serializer.SerializeValue(ref TargetId);
        serializer.SerializeValue(ref Value);
    }
}

public class CoreGameManager : NetworkBehaviour
{
    // ���� ���� ��
    [HideInInspector] public Dictionary<ulong, PlayerController> AllPlayers = new(); // ��� �÷��̾ ���� ��ųʸ�
    AsyncOperationHandle<GameObject> playerPanelPrefabOpHandle;
    Dictionary<Vector3, bool> playerPanelPos = new Dictionary<Vector3, bool> { { new Vector3(-13.5f, 0.01f, 4.7f), false }, { new Vector3(-7f, 0.01f, 4.7f), false }, { new Vector3(7f, 0.01f, 4.7f), false }, { new Vector3(13.5f, 0.01f, 4.7f), false }, };
    [HideInInspector] public event Action ConnectedClientCallBack; // Ŭ���̾�Ʈ�� ������ �� ���⿡ ������ Ŭ���̾�Ʈ ���� �ݹ��� ����

    // ��ǥ ���� ����
    [HideInInspector] public NetworkVariable<int> TargetNumber = new NetworkVariable<int>(33, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // ��ǥ ������ 33������ �ʱ�ȭ

    // ���� ���� ����
    [HideInInspector] public NetworkVariable<bool> IsCompleteTask = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // ��û�� ���� Rpc�� �Ϸ�Ǿ������� ���� �ݹ�

    // ����
    [SerializeField] GameObject targetNumberTitle;
    [SerializeField] GameObject targetNumberText;
    AsyncOperationHandle<GameObject> projectileVFXOpHandle;

    // �̱���
    static CoreGameManager instance;
    public static CoreGameManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (IsServer)
        {
            TargetNumber.OnValueChanged += SetTargetNumberTextClientRpc;
        }
    }

    #region ���� ���� ��
    [ServerRpc(RequireOwnership = false)]
    public void InitPlayerServerRpc(ServerRpcParams rpcParams = default) // Ŭ���̾�Ʈ ���� �� AllPlayers�� �߰��ϰ� �÷��̾� �г��� �����ϱ�
    {
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SyncAllPlayersClientRpc(id);
        }

        CreatePlayerPanel(rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void SyncAllPlayersClientRpc(ulong clientId) // ��� �÷��̾��� ������ AllPlayers�� ����ȭ�ϴ� �޼���
    {
        if (!AllPlayers.ContainsKey(clientId)) // ���� AllPlayers�� clientId�� ���ٸ� �߰�
        {
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

            foreach (PlayerController player in players)
            {
                if (player.GetComponent<NetworkObject>().OwnerClientId == clientId)
                {
                    AllPlayers[clientId] = player;
                }
            }
        }
    }
    async void CreatePlayerPanel(ulong clientId) // �÷��̾� �г� ����
    {
        foreach (var pos in playerPanelPos.Keys)
        {
            if (!playerPanelPos[pos])
            {
                playerPanelPos[pos] = true;
                AllPlayers[clientId].SetPlayerCharacterPosClientRpc(pos - new Vector3(0, 0, 4.3f));
                AllPlayers[clientId].SetTossCardPosClientRpc(pos - new Vector3(0, 0, 18.7f));

                playerPanelPrefabOpHandle = Addressables.InstantiateAsync("PlayerPanelPrefab");
                GameObject go = await playerPanelPrefabOpHandle.Task;
                go.transform.position = pos;
                go.transform.eulerAngles = new Vector3(90, 0, 0);
                go.GetComponent<NetworkObject>().Spawn();
                go.GetComponent<NetworkObject>().ChangeOwnership(clientId);
                break;
            }
        }

        ConnectedClientCallBackClientRpc();
    }
    [ClientRpc]
    void ConnectedClientCallBackClientRpc()
    {
        ConnectedClientCallBack?.Invoke();
    }

    public void StartAllPlayers() // �� �÷��̾���� ���� �� �޼��带 ȣ���ϴ� �޼���
    {
        WorldUIPlayerPanel[] playerPanels = FindObjectsByType<WorldUIPlayerPanel>(FindObjectsSortMode.None);
        foreach (WorldUIPlayerPanel playerPanel in playerPanels)
        {
            playerPanel.InitValueClientRpc();
        }

        foreach (ulong client in AllPlayers.Keys)
        {
            AllPlayers[client].MoveCameraIntroClientRpc();
        }

    }
    #endregion
    #region ī�� ȿ�� �ߵ� ���� �޼���
    [ServerRpc(RequireOwnership = false)]
    public void ExecuteCardEffectsServerRpc(CardEffectTask[] tasks)
    {
        ExecuteCardEffectsAsync(tasks);
    }
    async void ExecuteCardEffectsAsync(CardEffectTask[] tasks)
    {
        Debug.Log($"CoreGameManagerSend : �񵿱� �Լ� ����");
        IsCompleteTask.Value = false; // �۾� �ݹ� �ʱ�ȭ

        if (tasks.Length == 0) // �½�ũ�� ���ٸ�
        {
            await Task.Delay(2000); // �׳� 2�� ���
        }
        else if (tasks.Length > 0) // �½�ũ�� �ִٸ� ����
        {
            foreach (var task in tasks)
            {
                List<ulong> targetIdList = task.TargetId.ToList();

                if (targetIdList.Count >= 2 && targetIdList.Contains(task.InvokerId)) // Ÿ�� ����Ʈ�� 2�� �̻��̰� Ÿ�ٿ� �κ�Ŀ�� ���ԵǾ��ִٸ� �κ�Ŀ�� �ε����� 0���� �ϱ�
                {
                    foreach (ulong id in targetIdList)
                    {
                        if (id == task.InvokerId)
                        {
                            int invokerIdIndex = targetIdList.IndexOf(id);
                            (targetIdList[0], targetIdList[invokerIdIndex]) = (targetIdList[invokerIdIndex], targetIdList[0]);
                            break;
                        }
                    }
                }

                Debug.Log($"CoreGameManagerSend : ī�� ȿ�� �ߵ� �½�ũ �ݺ���. InvokerId : {task.InvokerId}");
                switch (task.EffectType)
                {
                    case EffectTypeEnum.LoseDamage:
                        await LoseAndDamageByDealer(task.InvokerId, targetIdList.ToArray(), task.Value);
                        break;
                    case EffectTypeEnum.StackSum:
                        await StackSumByCardEffect(task.InvokerId, targetIdList.ToArray(), task.Value);
                        break;
                    case EffectTypeEnum.Damage:
                        await DamageByCardEffect(task.InvokerId, targetIdList.ToArray(), task.Value);
                        break;
                    case EffectTypeEnum.Heal:
                        await HealByCardEffect(task.InvokerId, targetIdList.ToArray(), task.Value);
                        break;
                    case EffectTypeEnum.GetGold:
                        await GetGoldByCardEffect(task.InvokerId, targetIdList.ToArray(), task.Value);
                        break;
                    case EffectTypeEnum.GetDamageMP:
                        await GetDamageMPByCardEffect(task.InvokerId, targetIdList.ToArray(), task.Value);
                        break;
                    case EffectTypeEnum.StealDamageMP:
                        await StealDamageMPByCardEffect(task.InvokerId, targetIdList.ToArray(), task.Value);
                        break;
                }
            }

            Debug.Log($"CoreGameManagerSend : ī�� ȿ�� �ߵ� �Ϸ�!. GameFlowManager.Instance.IsClientSendCallBack : {GameFlowManager.Instance.IsClientSendCallBack}");
        }


        IsCompleteTask.Value = true; // �۾� �Ϸ� �ݹ� ������
        GameFlowManager.Instance.IsClientSendCallBack = true; // ī�� ����Ʈ�� �����ٰ� ���� �÷ο� �޴������� �˷��ֱ�
    }
    async Task LoseAndDamageByDealer(ulong invokerId, ulong[] targetIds, int value) // ���� �� TEMP��. ���߿� �� ��������
    {
        foreach (ulong id in targetIds)
        {
            Debug.Log($"invokerId : {invokerId}");
            Debug.Log($"targetId : {id}");

            Dealer.LocalInstance.DoAttackClientRpc(id); 

            await Task.Delay(1000);

            AllPlayers[id].playerStats.DamagedClientRpc(value);
        }

        await Task.Delay(1000);
    }
    async Task StackSumByCardEffect(ulong invokerId, ulong[] targetIds, int value)
    {
        foreach (ulong id in targetIds)
        {
            Debug.Log($"invokerId : {invokerId}");
            Debug.Log($"targetId : {id}");

            if (invokerId == id) // �� �����θ� ��ȭ�� ��
            {
                AllPlayers[id].playerCards.StackSumByCardEffectClientRpc(value);
            }
            else // �ٸ� Ŭ���̾�Ʈ�� ������ ������ ��
            {
                await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id);
                AllPlayers[id].playerCards.StackSumByCardEffectClientRpc(value);
            }

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task DamageByCardEffect(ulong invokerId, ulong[] targetIds, int value) // ������ �������� �ְ�, �ð�ȿ������ �ִ� �޼���
    {
        foreach (ulong id in targetIds)
        {
            await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id); // ����ü ������ Ʈ�� �����, Ʈ���� ���� ������ ��ٸ���

            AllPlayers[id].playerStats.DamagedClientRpc(value); // Ʈ���� ������ �������� �ֱ�

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task HealByCardEffect(ulong invokerId, ulong[] targetIds, int value)
    {
        foreach (ulong id in targetIds)
        {
            if (invokerId == id) // �� �����θ� ��ȭ�� ��
            {
                AllPlayers[id].playerStats.HealedClientRpc(value);
            }
            else // �ٸ� Ŭ���̾�Ʈ�� �����ٶ�
            {
                await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id); // ����ü ������ Ʈ�� �����, Ʈ���� ���� ������ ��ٸ���
                AllPlayers[id].playerStats.HealedClientRpc(value);
            }

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task GetGoldByCardEffect(ulong invokerId, ulong[] targetIds, int value)
    {
        foreach (ulong id in targetIds)
        {
            // ��� ��� �� ����ü �ð� ȿ�� ����
            AllPlayers[id].playerStats.ChangeGoldClientRpc(value);

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task GetDamageMPByCardEffect(ulong invokerId, ulong[] targetIds, int value)
    {
        foreach (ulong id in targetIds)
        {
            if (invokerId == id) // �� �����θ� ��ȭ�� ��
            {
                AllPlayers[id].playerStats.ChangeDamageMPClientRpc(value);
            }
            else // �ٸ� Ŭ���̾�Ʈ�� ��ȭ�� ��
            {
                await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id); // ����ü ������ Ʈ�� �����, Ʈ���� ���� ������ ��ٸ���
                AllPlayers[id].playerStats.ChangeDamageMPClientRpc(value);
            }

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task StealDamageMPByCardEffect(ulong invokerId, ulong[] targetIds, int value)
    {
        foreach (ulong id in targetIds)
        {
            await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id, true); // ������ �Ű������� true�� �༭ ���ƿ��� ������Ÿ�Ϸ� �����

            AllPlayers[invokerId].playerStats.ChangeDamageMPClientRpc(value);
            AllPlayers[id].playerStats.ChangeDamageMPClientRpc(-value);

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    #endregion
    #region ���� ����
    [ClientRpc]
    public void ActiveTargetNumberTextClientRpc()
    {
        targetNumberTitle.SetActive(true);
        targetNumberText.SetActive(true);
    }
    [ClientRpc]
    public void SetTargetNumberTextClientRpc(int oldValue, int newValue)
    {
        targetNumberText.GetComponent<TMP_Text>().text = newValue.ToString();
    }

    async Task LaunchProjectile(string projectileName, ulong invokerId, ulong targetId, bool reverse = false)
    {
        projectileVFXOpHandle = Addressables.LoadAssetAsync<GameObject>(projectileName);
        await projectileVFXOpHandle.Task;

        if (projectileVFXOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            await WaitForTween(LaunchProjectileTween(projectileVFXOpHandle.Result, invokerId, targetId, reverse));
        }
        else
        {
            Debug.LogError("Failed to load GetDamageMPVFXPrefab!");
        }
    }
    Tween LaunchProjectileTween(GameObject projectileGo, ulong invokerId, ulong targetId, bool reverse)
    {
        // �������� �׸��� ������Ÿ�� ������ ���� ���� �ʱ�ȭ
        float duration = 1f;
        float height = 2f;

        // ��������� ������Ÿ���� ���ƿ��� �ϰ� �ƴ϶�� targetId�� ������ �ϱ�
        Vector3 start;
        Vector3 end;
        if (reverse)
        {
            start = AllPlayers[targetId].PlayerCharacterPos.Value;
            end = AllPlayers[invokerId].PlayerCharacterPos.Value;
        }
        else
        {
            start = AllPlayers[invokerId].PlayerCharacterPos.Value;
            end = AllPlayers[targetId].PlayerCharacterPos.Value;
        }

        Vector3 middle = new Vector3((start.x + end.x) / 2, Mathf.Max(start.y, end.y) + height, (start.z + end.z) / 2);
        Vector3[] path = { start, middle, end };

        // ������Ÿ�� ���� �� �������� �׸��� ���󰡰� �ϱ�
        GameObject go = Instantiate(projectileGo, start, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();

        Tween tween = go.transform
            .DOPath(path, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad)
            .OnComplete(() => Destroy(go));
        return tween;
    }
    #endregion
    #region ����� �޼���
    [ServerRpc(RequireOwnership = false)]
    public void TurnFalseIsCompleteTaskServerRpc() // �۾� �Ϸ� �ݹ��� �ٽ� false�� ������ �޼���
    {
        IsCompleteTask.Value = false;
    }

    Task WaitForTween(Tween tween) // ��Ʈ���� �Ϸ�� ������ ��ٸ��� �½�ũ
    {
        var tcs = new TaskCompletionSource<bool>();

        tween.OnKill(() => tcs.SetResult(true)); // ��Ʈ���� ���� �� OnKill �̺�Ʈ�� ���� Task �Ϸ�

        return tcs.Task;
    }
    #endregion
}
