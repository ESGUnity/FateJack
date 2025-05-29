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
    // ������ ������ ������
    [SerializeField] GameObject playerStackPrefab;
    WorldUIPlayerStack worldUIPlayerStack;

    // �� ����
    [HideInInspector] public NetworkList<Card> PlayerDeck = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // ���� ����
    [HideInInspector] public NetworkList<Card> StackCards = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<int> StackSum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // ��Ʈ ����
    [HideInInspector] public NetworkList<Card> DirectHitCards = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkList<Card> HitCards = new NetworkList<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<bool> IsStand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<bool> IsBurst = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<bool> IsCleanHit = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public bool IsHitOrStandCycle = false;
    [HideInInspector] public bool IsDirectHitOrStandCycle = false;
    [HideInInspector] public Card CurrentSelectCard;
    [HideInInspector] public NetworkVariable<Card> hitCard = new NetworkVariable<Card>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // �ð�ȿ�� ����
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
                                InGameUIManager.Instance.CreateLog("��Ʈ�� ī�带 �����ϰ� ��Ʈ���ּ���!");
                                return;
                            }
                            else
                            {
                                hitCard.Value = CurrentSelectCard;
                                GameFlowManager.Instance.ClientSendCallBackServerRpc(); // ��Ʈ �ߴٰ� ������ �ݹ� ������
                                CurrentSelectCard = default(Card); // ���̷�Ʈ ��Ʈ �� ������ ī��� �ʱ�ȭ���ֱ�
                                return;
                            }
                        }
                        else if (IsHitOrStandCycle)
                        {
                            int randomIndex = Random.Range(0, PlayerDeck.Count);
                            hitCard.Value = PlayerDeck[randomIndex];
                            GameFlowManager.Instance.ClientSendCallBackServerRpc(); // ��Ʈ �ߴٰ� ������ �ݹ� ������
                            return;
                        }
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("StandBtn"))
                    {
                        IsStand.Value = true;
                        GameFlowManager.Instance.NotifyClientStandServerRpc(); // ���ĵ��ߴٰ� �˸���
                        GameFlowManager.Instance.ClientSendCallBackServerRpc(); // ���ĵ� �ߴٰ� ������ �ݹ� ������
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
        // ������ ��� ���� ī�带 ��Ʈ�ϰ� �÷��̾� ������ ���������� ���� ó��
        if (IsOwner) 
        {
            if (!hitCard.Value.Equals(default(Card))) // ���̷�Ʈ ��Ʈ�� ���
            {
                DirectHitCards.Add(hitCard.Value);
            }
            else // �׳� ��Ʈ�� ���
            {
                HitCards.Add(hitCard.Value);
            }

            PlayerDeck.Remove(hitCard.Value);
            StackCards.Add(hitCard.Value);
            StackSum.Value += hitCard.Value.Number;
        }

        // ���ʰ� �ƴ� ��� ��� Ŭ���̾�Ʈ�� ȭ�鿡 ���� ī�尡 ���̰� ó��
        AddStackTask(card);
    }
    async void AddStackTask(Card hitCard) // ���ÿ� ī�� �߰��ϰ� ī�� ȿ������ �ߵ��ϴ� �޼���(��Ʈ ��, ����Ʈ �� ��� �ߵ�)
    {
        await worldUIPlayerStack.SetPlayerStack(hitCard);

        if (IsOwner)
        {
            // ī�� ȿ�� �ߵ�
            List<CardEffectTask> tasks = new();
            CardEffect cardEffect = CardEffectList.FindCardEffectToKey(hitCard.CardEffectKey.ToString());

            if (cardEffect.Type == CardEffectTypeEnum.Hit)
            {
                tasks.AddRange(cardEffect.ActiveCardEffect(GetComponent<PlayerController>()));
            }
            // TODO : ���ĵ�� ����Ʈ ȿ���� �߰����� ���߿�

            this.hitCard.Value = default(Card); // ���̷�Ʈ ��Ʈ �� ���� �ʱ�ȭ

            CoreGameManager.Instance.ExecuteCardEffectsServerRpc(tasks.ToArray());
        }
    }
    [ClientRpc]
    public void InitStackClientRpc()
    {
        if (IsOwner)
        {
            foreach (Card card in HitCards) // �׳� ��Ʈ�� ī��� �ٽ� ������
            {
                PlayerDeck.Add(card);
            }
            foreach (Card card in DirectHitCards) // ���̷�Ʈ ��Ʈ ī��� �����Ϸ�
            {
                CardManager.Instance.AddNumberToRootNumberServerRpc(card.Number);
            }

            StackCards.Clear();
            StackSum.Value = 0;
        }

        worldUIPlayerStack.InitStackCard(); // ���� �ʱ�ȭ
    }
    [ClientRpc]
    public void ChangeIsStandClientRpc(bool isStand) // ���ĵ� �� IsStand�� Ȱ��ȭ�ϴ� �޼���
    {
        if (IsOwner)
        {
            IsStand.Value = isStand;
        }
    }
    [ClientRpc]
    public void StackSumByCardEffectClientRpc(int stack) // ī�� ȿ���� ���ÿ� �߰��ϴ� �ð� ȿ�� �� ���� �߰��ϴ� �޼���. CoreGameManager���� ȣ��
    {
        if (IsOwner)
        {
            StackSum.Value += stack;
        }

        // ���ÿ� ���� �߰��Ǵ� ȿ��
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
    void StackPlusVFXLoadComplete(AsyncOperationHandle<GameObject> handle) // ���� �÷��� �ڵ鷯
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = Instantiate(handle.Result, GetComponent<PlayerController>().PlayerCharacterPos.Value, handle.Result.transform.rotation);
            go.GetComponent<GetDamageMPVFX>().TriggerVFX();
        }
    }
    void StackMinusVFXLoadComplete(AsyncOperationHandle<GameObject> handle) // ���� ���̳ʽ� �ڵ鷯
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = Instantiate(handle.Result, GetComponent<PlayerController>().PlayerCharacterPos.Value, handle.Result.transform.rotation);
            go.GetComponent<GetDamageMPVFX>().TriggerVFX();
        }
    }
    void CheckBurst(int prevValue, int newValue) // ����Ʈ���� üũ�ϴ� �޼���
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
    void CheckCleanHit(int prevValue, int newValue) // Ŭ����Ʈ���� üũ�ϴ� �޼���
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