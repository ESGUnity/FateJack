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

// 효과 타입
public enum EffectTypeEnum
{
    LoseDamage, // 패배 시 데미지
    StackSum,
    Damage,
    Heal,
    GetGold,
    GetDamageMP,
    StealDamageMP,
}

// CoreGameManager에 보낼 카드 효과 Task 구조체
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
    // 게임 시작 전
    [HideInInspector] public Dictionary<ulong, PlayerController> AllPlayers = new(); // 모든 플레이어에 대한 딕셔너리
    AsyncOperationHandle<GameObject> playerPanelPrefabOpHandle;
    Dictionary<Vector3, bool> playerPanelPos = new Dictionary<Vector3, bool> { { new Vector3(-13.5f, 0.01f, 4.7f), false }, { new Vector3(-7f, 0.01f, 4.7f), false }, { new Vector3(7f, 0.01f, 4.7f), false }, { new Vector3(13.5f, 0.01f, 4.7f), false }, };
    [HideInInspector] public event Action ConnectedClientCallBack; // 클라이언트는 감지할 수 없기에 별도의 클라이언트 접속 콜백을 생성

    // 목표 점수 관련
    [HideInInspector] public NetworkVariable<int> TargetNumber = new NetworkVariable<int>(33, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 목표 점수는 33점으로 초기화

    // 게임 구조 관련
    [HideInInspector] public NetworkVariable<bool> IsCompleteTask = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 요청한 서버 Rpc가 완료되었는지에 대한 콜백

    // 연출
    [SerializeField] GameObject targetNumberTitle;
    [SerializeField] GameObject targetNumberText;
    AsyncOperationHandle<GameObject> projectileVFXOpHandle;

    // 싱글턴
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

    #region 게임 시작 전
    [ServerRpc(RequireOwnership = false)]
    public void InitPlayerServerRpc(ServerRpcParams rpcParams = default) // 클라이언트 접속 시 AllPlayers에 추가하고 플레이어 패널을 생성하기
    {
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SyncAllPlayersClientRpc(id);
        }

        CreatePlayerPanel(rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void SyncAllPlayersClientRpc(ulong clientId) // 모든 플레이어의 씬에서 AllPlayers를 동기화하는 메서드
    {
        if (!AllPlayers.ContainsKey(clientId)) // 만약 AllPlayers에 clientId가 없다면 추가
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
    async void CreatePlayerPanel(ulong clientId) // 플레이어 패널 생성
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

    public void StartAllPlayers() // 각 플레이어들의 시작 시 메서드를 호출하는 메서드
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
    #region 카드 효과 발동 관련 메서드
    [ServerRpc(RequireOwnership = false)]
    public void ExecuteCardEffectsServerRpc(CardEffectTask[] tasks)
    {
        ExecuteCardEffectsAsync(tasks);
    }
    async void ExecuteCardEffectsAsync(CardEffectTask[] tasks)
    {
        Debug.Log($"CoreGameManagerSend : 비동기 함수 입장");
        IsCompleteTask.Value = false; // 작업 콜백 초기화

        if (tasks.Length == 0) // 태스크가 없다면
        {
            await Task.Delay(2000); // 그냥 2초 대기
        }
        else if (tasks.Length > 0) // 태스크가 있다면 실행
        {
            foreach (var task in tasks)
            {
                List<ulong> targetIdList = task.TargetId.ToList();

                if (targetIdList.Count >= 2 && targetIdList.Contains(task.InvokerId)) // 타겟 리스트가 2개 이상이고 타겟에 인보커도 포함되어있다면 인보커의 인덱스를 0으로 하기
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

                Debug.Log($"CoreGameManagerSend : 카드 효과 발동 태스크 반복문. InvokerId : {task.InvokerId}");
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

            Debug.Log($"CoreGameManagerSend : 카드 효과 발동 완료!. GameFlowManager.Instance.IsClientSendCallBack : {GameFlowManager.Instance.IsClientSendCallBack}");
        }


        IsCompleteTask.Value = true; // 작업 완료 콜백 날리기
        GameFlowManager.Instance.IsClientSendCallBack = true; // 카드 이펙트가 끝났다고 게임 플로우 메니저에게 알려주기
    }
    async Task LoseAndDamageByDealer(ulong invokerId, ulong[] targetIds, int value) // 여긴 다 TEMP다. 나중에 다 수정하자
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

            if (invokerId == id) // 나 스스로를 강화할 때
            {
                AllPlayers[id].playerCards.StackSumByCardEffectClientRpc(value);
            }
            else // 다른 클라이언트의 스택을 변경할 때
            {
                await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id);
                AllPlayers[id].playerCards.StackSumByCardEffectClientRpc(value);
            }

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task DamageByCardEffect(ulong invokerId, ulong[] targetIds, int value) // 실제로 데미지를 주고, 시각효과까지 있는 메서드
    {
        foreach (ulong id in targetIds)
        {
            await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id); // 투사체 날리는 트윈 만들고, 트윈이 끝날 때까지 기다리기

            AllPlayers[id].playerStats.DamagedClientRpc(value); // 트윈이 끝나면 데미지를 주기

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task HealByCardEffect(ulong invokerId, ulong[] targetIds, int value)
    {
        foreach (ulong id in targetIds)
        {
            if (invokerId == id) // 나 스스로를 강화할 때
            {
                AllPlayers[id].playerStats.HealedClientRpc(value);
            }
            else // 다른 클라이언트를 힐해줄때
            {
                await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id); // 투사체 날리는 트윈 만들고, 트윈이 끝날 때까지 기다리기
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
            // 골드 얻는 건 투사체 시각 효과 없음
            AllPlayers[id].playerStats.ChangeGoldClientRpc(value);

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    async Task GetDamageMPByCardEffect(ulong invokerId, ulong[] targetIds, int value)
    {
        foreach (ulong id in targetIds)
        {
            if (invokerId == id) // 나 스스로를 강화할 때
            {
                AllPlayers[id].playerStats.ChangeDamageMPClientRpc(value);
            }
            else // 다른 클라이언트를 강화할 때
            {
                await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id); // 투사체 날리는 트윈 만들고, 트윈이 끝날 때까지 기다리기
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
            await LaunchProjectile("CardEffectDamageProjectilePrefab", invokerId, id, true); // 마지막 매개변수를 true로 줘서 돌아오는 프로젝타일로 만들기

            AllPlayers[invokerId].playerStats.ChangeDamageMPClientRpc(value);
            AllPlayers[id].playerStats.ChangeDamageMPClientRpc(-value);

            await Task.Delay(700);
        }

        await Task.Delay(1000);
    }
    #endregion
    #region 연출 관련
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
        // 포물선을 그리는 프로젝타일 생성을 위한 변수 초기화
        float duration = 1f;
        float height = 2f;

        // 리버스라면 프로젝타일이 돌아오게 하고 아니라면 targetId에 날리게 하기
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

        // 프로젝타일 생성 후 포물선을 그리며 날라가게 하기
        GameObject go = Instantiate(projectileGo, start, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();

        Tween tween = go.transform
            .DOPath(path, duration, PathType.CatmullRom).SetEase(Ease.InOutQuad)
            .OnComplete(() => Destroy(go));
        return tween;
    }
    #endregion
    #region 잡다한 메서드
    [ServerRpc(RequireOwnership = false)]
    public void TurnFalseIsCompleteTaskServerRpc() // 작업 완료 콜백을 다시 false로 돌리는 메서드
    {
        IsCompleteTask.Value = false;
    }

    Task WaitForTween(Tween tween) // 두트윈이 완료될 때까지 기다리는 태스크
    {
        var tcs = new TaskCompletionSource<bool>();

        tween.OnKill(() => tcs.SetResult(true)); // 두트윈이 끝날 때 OnKill 이벤트를 통해 Task 완료

        return tcs.Task;
    }
    #endregion
}
