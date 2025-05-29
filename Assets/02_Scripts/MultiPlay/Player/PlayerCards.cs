using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCards : NetworkBehaviour
{
    // 스택이 보여질 프리팹
    [SerializeField] GameObject playerStackPrefab;
    WorldUIPlayerStack worldUIPlayerStack;

    // 덱 관련
    [HideInInspector] public NetworkList<Card> PlayerDeck = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // 스택 관련
    [HideInInspector] public NetworkList<Card> StackCards = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<int> StackSum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // 히트 관련
    [HideInInspector] public NetworkList<Card> DirectHitCards = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkList<Card> HitCards = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<bool> IsStand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<bool> IsBurst = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<bool> IsCleanHit = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public bool IsHitOrStandCycle = false;
    [HideInInspector] public bool IsDirectHitOrStandCycle = false;
    [HideInInspector] public Card CurrentSelectCard;
    [HideInInspector] public NetworkVariable<Card> hitCard = new NetworkVariable<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // 시각효과 관련
    AsyncOperationHandle<GameObject> stackPlusVFXOpHandle;
    AsyncOperationHandle<GameObject> stackMinusVFXOpHandle;

    void Start()
    {
        if (IsOwner)
        {
            StackSum.OnValueChanged += CheckBurst;
            StackSum.OnValueChanged += CheckCleanHit;
            SubscribeGameFlowManager();
        }
    }
    async void SubscribeGameFlowManager()
    {
        while (GameFlowManager.Instance == null)
        {
            await Task.Delay(100);
        }

        GameFlowManager.Instance.InGameStart += SpawnPlayerStackPanelServerRpc;
    }
    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerStackPanelServerRpc()
    {
        SpawnPlayerStackPanelClientRpc();
    }
    [ClientRpc]
    void SpawnPlayerStackPanelClientRpc()
    {
        GameObject go = Instantiate(playerStackPrefab);
        go.transform.position = new Vector3(GetComponent<PlayerController>().PlayerCharacterPos.Value.x, 0.01f, -4.5f);
        worldUIPlayerStack = go.GetComponent<WorldUIPlayerStack>();
    }

    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("HitBtn"))
                    {
                        if (IsDirectHitOrStandCycle)
                        {
                            if (CurrentSelectCard.Equals(default(Card)))
                            {
                                InGameUIManager.Instance.CreateLog("히트할 카드를 선택하고 히트해주세요!");
                                return;
                            }
                            else
                            {
                                hitCard.Value = CurrentSelectCard;
                                GameFlowManager.Instance.ClientSendCallBackServerRpc(); // 히트 했다고 서버에 콜백 날리기
                                CurrentSelectCard = default(Card); // 다이렉트 히트 시 선택한 카드는 초기화해주기
                                return;
                            }
                        }
                        else if (IsHitOrStandCycle)
                        {
                            int randomIndex = Random.Range(0, PlayerDeck.Count);
                            hitCard.Value = PlayerDeck[randomIndex];
                            GameFlowManager.Instance.ClientSendCallBackServerRpc(); // 히트 했다고 서버에 콜백 날리기
                            return;
                        }
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("StandBtn"))
                    {
                        IsStand.Value = true;
                        GameFlowManager.Instance.NotifyClientStandServerRpc(); // 스탠드했다고 알리기
                        GameFlowManager.Instance.ClientSendCallBackServerRpc(); // 스탠드 했다고 서버에 콜백 날리기
                        return;
                    }
                }
            }
        }
    }
    [ClientRpc]
    public void AddPlayerDeckClientRpc(Card card)
    {
        if (IsOwner)
        {
            PlayerDeck.Add(card);
        }
    }
    [ClientRpc]
    public void HitCardClientRpc(Card card)
    {
        // 오너인 경우 실제 카드를 히트하고 플레이어 덱에서 빠져나가는 것을 처리
        if (IsOwner) 
        {
            if (!hitCard.Value.Equals(default(Card))) // 다이렉트 히트의 경우
            {
                DirectHitCards.Add(hitCard.Value);
            }
            else // 그냥 히트인 경우
            {
                HitCards.Add(hitCard.Value);
            }

            PlayerDeck.Remove(hitCard.Value);
            StackCards.Add(hitCard.Value);
            StackSum.Value += hitCard.Value.Number;
        }

        // 오너가 아닌 경우 모든 클라이언트의 화면에 스택 카드가 보이게 처리
        AddStackTask(card);
    }
    async void AddStackTask(Card hitCard) // 스택에 카드 추가하고 카드 효과까지 발동하는 메서드(히트 시, 버스트 시 등등 발동)
    {
        await worldUIPlayerStack.SetPlayerStack(hitCard);

        if (IsOwner)
        {
            // 카드 효과 발동
            List<CardEffectTask> tasks = new();
            CardEffect cardEffect = CardEffectList.FindCardEffectToKey(hitCard.CardEffectKey.ToString());

            if (cardEffect.Type == CardEffectTypeEnum.Hit)
            {
                tasks.AddRange(cardEffect.ActiveCardEffect(GetComponent<PlayerController>()));
            }
            // TODO : 스탠드랑 버스트 효과도 추가하자 나중에

            this.hitCard.Value = default(Card); // 다이렉트 히트 후 변수 초기화

            CoreGameManager.Instance.ExecuteCardEffectsServerRpc(tasks.ToArray());
        }
    }
    [ClientRpc]
    public void InitStackClientRpc()
    {
        if (IsOwner)
        {
            foreach (Card card in HitCards) // 그냥 히트한 카드는 다시 덱으로
            {
                PlayerDeck.Add(card);
            }
            foreach (Card card in DirectHitCards) // 다이렉트 히트 카드는 번파일로
            {
                CardManager.Instance.AddNumberToRootNumberServerRpc(card.Number);
            }

            StackCards.Clear();
            StackSum.Value = 0;
        }

        worldUIPlayerStack.InitStackCard(); // 스택 초기화
    }
    [ClientRpc]
    public void ChangeIsStandClientRpc(bool isStand) // 스탠드 시 IsStand를 활성화하는 메서드
    {
        if (IsOwner)
        {
            IsStand.Value = isStand;
        }
    }
    [ClientRpc]
    public void StackSumByCardEffectClientRpc(int stack) // 카드 효과로 스택에 추가하는 시각 효과 겸 스택 추가하는 메서드. CoreGameManager에서 호출
    {
        if (IsOwner)
        {
            StackSum.Value += stack;
        }

        // 스택에 숫자 추가되는 효과
        if (stack > 0)
        {
            stackPlusVFXOpHandle = Addressables.LoadAssetAsync<GameObject>("StackPlusVFXPrefab");
            stackPlusVFXOpHandle.Completed += StackPlusVFXLoadComplete;
        }
        else if (stack < 0)
        {
            stackMinusVFXOpHandle = Addressables.LoadAssetAsync<GameObject>("StackMinusVFXPrefab");
            stackMinusVFXOpHandle.Completed += StackMinusVFXLoadComplete;
        }
    }
    void StackPlusVFXLoadComplete(AsyncOperationHandle<GameObject> handle) // 스택 플러스 핸들러
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = Instantiate(handle.Result, GetComponent<PlayerController>().PlayerCharacterPos.Value, handle.Result.transform.rotation);
            go.GetComponent<GetDamageMPVFX>().TriggerVFX();
        }
    }
    void StackMinusVFXLoadComplete(AsyncOperationHandle<GameObject> handle) // 스택 마이너스 핸들러
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = Instantiate(handle.Result, GetComponent<PlayerController>().PlayerCharacterPos.Value, handle.Result.transform.rotation);
            go.GetComponent<GetDamageMPVFX>().TriggerVFX();
        }
    }
    void CheckBurst(int prevValue, int newValue) // 버스트인지 체크하는 메서드
    {
        if (IsOwner)
        {
            if (newValue > CoreGameManager.Instance.TargetNumber.Value)
            {
                IsBurst.Value = true;
            }
            else
            {
                IsBurst.Value = false;
            }
        }
    }
    void CheckCleanHit(int prevValue, int newValue) // 클린히트인지 체크하는 메서드
    {
        if (IsOwner)
        {
            if (newValue == CoreGameManager.Instance.TargetNumber.Value)
            {
                IsCleanHit.Value = true;
            }
            else
            {
                IsCleanHit.Value = false;
            }
        }
    }
    void OnDisable()
    {
        StackSum.OnValueChanged -= CheckBurst;
        StackSum.OnValueChanged -= CheckCleanHit;
        GameFlowManager.Instance.InGameStart -= SpawnPlayerStackPanelServerRpc;
        stackPlusVFXOpHandle.Completed -= StackPlusVFXLoadComplete;
        stackMinusVFXOpHandle.Completed -= StackMinusVFXLoadComplete;
    }
}