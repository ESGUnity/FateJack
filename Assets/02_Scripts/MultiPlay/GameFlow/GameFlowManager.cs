using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum RoundRewardEnum
{
    None,
    ClothoPresent,
    LachesisPresent,
    AtroposPresent,
    Loot
}
public enum GameFlowStateEnum
{
    UnableDoAnything,
    DirectHitOrStand,
    HitOrStand,
    ChoiceLootOrPresent,
}

public class GameFlowManager : NetworkBehaviour
{
    // ���� ���� �� �κ�
    List<ulong> readyPlayers = new(); // �غ� �Ϸ��� Ŭ���̾�Ʈ ���̵�
    [HideInInspector] public event Action InGameStart; // ���� ���� �� Invoke�ϴ� ��������Ʈ

    // ���� �ð�
    const float CURRENT_ROUND_UI_LIFE_TIME = 3f; // ���� UI�� ���� �ð�
    const float CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME = 0.3f; // UI�� ���� �� ���忡 �ɸ��� �ð�
    const float TERM_TIME = 1f; // �ణ�� ������ �ð�(ī�带 �ް��� ��Ʈ�� �Ѿ�ų� ��ΰ� ���ĵ��ϰ� ���� ���� �ܰ迡 �Ѿ�� ��)
    const float TOSS_CARD_TIME = 0.5f; // ī�� �� ���� �佺���ִ� �ð�
    const float HIT_TIME = 20f; // ��Ʈ�ϴµ� �ɸ��� �ð�
    const float ChoicePresentTime = 40f; // ���� �� �ð�
    const float PurchaseLootTime = 60f; // ����ǰ ���� �ð�

    // ���� ���� �� ���� ������
    const int DETERMINATION_AMOUNT_ON_ROUND_START = 2; // ���� ������
    const int GOLD_AMOUNT_ON_ROUND_START = 4; // ��� ������

    // ���� ���� ����
    [HideInInspector] public NetworkVariable<int> CurrentRound = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 1�� �ʱ�ȭ
    [HideInInspector] public bool IsClientSendCallBack = false;
    [HideInInspector] public bool IsStand = false;
    //[HideInInspector] public NetworkVariable<int> GameFlowState = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 0���� �ʱ�ȭ

    // ���� �� ���� ����
    RoundRewardEnum[] roundRewards = { RoundRewardEnum.ClothoPresent, RoundRewardEnum.LachesisPresent, RoundRewardEnum.AtroposPresent };

    // ���� ����
    bool isCardTossed = false; // ���� ���� �� ī�带 �����ִ� �ð� ȿ���� ���Ǵ� ����
    [SerializeField] GameObject remainTimeTitle;
    [SerializeField] GameObject remainTimeText;
    [SerializeField] GameObject currentHitPlayerTitle;
    [SerializeField] GameObject currentHitPlayerText;
    [SerializeField] GameObject hitBtn;
    [SerializeField] GameObject standBtn;

    // �̱���
    static GameFlowManager instance;
    public static GameFlowManager Instance { get { return instance; } }
    void Awake()
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

    #region ���� ���� ��
    [ServerRpc(RequireOwnership = false)]
    public void NotifyPlayerReadyServerRpc(ServerRpcParams rpcParams = default) // Ŭ���̾�Ʈ�� ������ �غ� �Ϸ��ߴٰ� �˸��� Rpc
    {
        if (!readyPlayers.Contains(rpcParams.Receive.SenderClientId))
        {
            readyPlayers.Add(rpcParams.Receive.SenderClientId);
        }
        else if (readyPlayers.Contains(rpcParams.Receive.SenderClientId))
        {
            readyPlayers.Remove(rpcParams.Receive.SenderClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestStartGameServerRpc() // ȣ��Ʈ�� ���� ���� ��ư�� ȣ���ϴ� �ΰ��� ���� �޼���
    {
        if (readyPlayers.Count == CoreGameManager.Instance.AllPlayers.Keys.Count) // �غ��� Ŭ��� ����� Ŭ�� ���� ���
        {
            CoreGameManager.Instance.StartAllPlayers(); // �� Ŭ���̾�Ʈ�� �ΰ��� ���� �� ������ �����ϱ�
            InvokeInGameStartClientRpc(); // InGameStart ��������Ʈ ����
            ActiveHitPlayerInfoTitleClientRpc(true); // ���� ��Ʈ ���� �÷��̾� Ÿ��Ʋ ǥ��
            CoreGameManager.Instance.ActiveTargetNumberTextClientRpc(); // ��ǥ ���� ǥ��
            CoreGameManager.Instance.SetTargetNumberTextClientRpc(0, CoreGameManager.Instance.TargetNumber.Value); // ��ǥ ���� ǥ��
            GetComponent<CardManager>().GenerateRootNumber(); // ��Ʈ ���� ����
            GetComponent<CardManager>().ActiveProbabilityTableClientRpc(); // ī�� ȿ�� Ȯ��ǥ ǥ��

            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys) // �������
            {
                CoreGameManager.Instance.AllPlayers[clientId].playerCards.PlayerDeck.OnListChanged += PlayerDeckChangedCallBack;
            }

            StartRound(); // �������� �ΰ��� �÷ο� �����ϱ�
        }
    }
    [ClientRpc]
    void InvokeInGameStartClientRpc()
    {
        InGameStart?.Invoke();
    }
    void PlayerDeckChangedCallBack(NetworkListEvent<Card> changeEvent)
    {
        isCardTossed = true;
    }
    #endregion
    #region ���� ���� ��
    void StartRound()
    {
        if (IsServer)
        {
            if (CurrentRound.Value == 1) // 1 ���� �� �⺻ ��� �ʱ�ȭ
            {
                StartCoroutine(StartPhaseCoroutine(RoundRewardEnum.None));
            }
            else if (CurrentRound.Value % 2 == 0) // ¦�� ���� ��
            {
                //int randomIndex = UnityEngine.Random.Range(0, roundRewards.Length);
                //RoundRewardEnum reward = roundRewards[randomIndex];

                StartCoroutine(StartPhaseCoroutine(RoundRewardEnum.None));
            }
            else // 1 ���带 ������ Ȧ�� ���� ��
            {
                StartCoroutine(StartPhaseCoroutine(RoundRewardEnum.None));
            }

            GetComponent<CardManager>().SetProbability(CurrentRound.Value);
        }
    }
    IEnumerator StartPhaseCoroutine(RoundRewardEnum roundReward)
    {
        if (IsServer)
        {
            // ���� ���带 ǥ���ϰ� ��û�ϱ�
            RequestDisplayPhraseClientRpc($"{CurrentRound.Value} ����");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // ���� ���� ǥ�� UI�� ������ ��û�ϱ�
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // ī�� �����ֱ�
            int receiveCompleteCount = 0;
            while (receiveCompleteCount < CoreGameManager.Instance.AllPlayers.Keys.Count) // ī�带 �� ���� �÷��̾� ���ڿ� ��� �÷��̾� ���ڰ� ������ �ݺ��� ����
            {
                foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
                {
                    if (CoreGameManager.Instance.AllPlayers[clientId].playerCards.PlayerDeck.Count < 12)
                    {
                        GetComponent<CardManager>().TossCardToPlayerClientRpc(clientId);

                        // ������ ī�带 ���� ������ ���
                        while (!isCardTossed)
                        {
                            yield return null;
                        }

                        isCardTossed = false;
                    }

                    if (CoreGameManager.Instance.AllPlayers[clientId].playerCards.PlayerDeck.Count == 12)
                    {
                        receiveCompleteCount++;
                    }
                }
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);


            // ���� ����
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                CoreGameManager.Instance.AllPlayers[clientId].playerStats.ChangeDamageMPClientRpc(DETERMINATION_AMOUNT_ON_ROUND_START);
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // ��� ����
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                CoreGameManager.Instance.AllPlayers[clientId].playerStats.ChangeGoldClientRpc(GOLD_AMOUNT_ON_ROUND_START);
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);




            // ��Ʈ ������ ����

            // ��Ʈ ����� ���۵��� ǥ���ϰ� ��û�ϱ�
            RequestDisplayPhraseClientRpc($"��Ʈ �Ǵ� ���ĵ�!");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI�� ������ ��û�ϱ�
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // ü���� ���� ���� �÷��̾���� ����. 1 - 1 ������ ��� ������ �÷��̾� ����
            int highestHealth = 0;
            ulong highestHealthClient = 0;
            if (CurrentRound.Value == 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, CoreGameManager.Instance.AllPlayers.Keys.Count);
                highestHealthClient = CoreGameManager.Instance.AllPlayers.Keys.ToList()[randomIndex];
            }
            else
            {
                foreach (var clientId in CoreGameManager.Instance.AllPlayers.Keys)
                {
                    if (CoreGameManager.Instance.AllPlayers[clientId].playerStats.Health.Value > highestHealth)
                    {
                        highestHealth = CoreGameManager.Instance.AllPlayers[clientId].playerStats.Health.Value;
                        highestHealthClient = clientId;
                    }
                }
            }

            // ü���� ���� ���� �÷��̾���� ���� ������� ���ư���
            int cycleCount = 1;
            Queue<ulong> cycleQueue = new();
            Queue<ulong> standQueue = new();

            // ���� �����ϴ� �÷��̾���� ������� ���ư��� ť �����
            cycleQueue.Enqueue(highestHealthClient);
            for (int i = 0; i < CoreGameManager.Instance.AllPlayers.Keys.Count - 1; i++)
            {
                highestHealthClient = (highestHealthClient + 1) % (ulong)CoreGameManager.Instance.AllPlayers.Keys.Count;
                cycleQueue.Enqueue(highestHealthClient);
            }

            while (standQueue.Count != CoreGameManager.Instance.AllPlayers.Keys.Count)
            {
                for (int i = 0; i < cycleQueue.Count; i++)
                {
                    ulong pop = cycleQueue.Dequeue();
                    ClientRpcParams rpcParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { pop } } };

                    // ���� ��Ʈ ���� �÷��̾� UI ǥ��
                    RequestDisplayPhraseClientRpc($"���� ����", rpcParams); // ���� ���� ��� UI
                    SetCurrentHitPlayerInfoClientRpc(true, CoreGameManager.Instance.AllPlayers[pop].NickName.Value.ToString()); // ���� ��Ʈ ���� �÷��̾� ǥ��

                    // cycleCount�� ���� ��Ʈ �Ǵ� ���̷�Ʈ ��Ʈ�� Ŭ���̾�Ʈ�� ��û
                    float startTime = Time.time;
                    if (cycleCount < 3)
                    {
                        RequestHitOrStandClientRpc(true, true, true, rpcParams); // ���̷�Ʈ ��Ʈ ��û

                        RequestCountRemainTimeClientRpc(true, (int)HIT_TIME); // Ÿ�̸�
                        yield return new WaitUntil(() => IsClientSendCallBack || Time.time >= startTime + HIT_TIME);
                    }
                    else
                    {
                        RequestHitOrStandClientRpc(false, true, true, rpcParams); // �׳� ��Ʈ ��û

                        RequestCountRemainTimeClientRpc(true, (int)HIT_TIME); // Ÿ�̸�
                        yield return new WaitUntil(() => IsClientSendCallBack || Time.time >= startTime + HIT_TIME);
                    }

                    // UI �����
                    RequestHidePhraseClientRpc(rpcParams); // ���� ���� UI �����
                    RequestCountRemainTimeClientRpc(true); // ���� �ð��� "-"�� ǥ���ϱ�
                    RequestHitOrStandClientRpc(true, true, false, rpcParams); // ��ư �����
                    yield return new WaitForSecondsRealtime(0.5f); // TODO : ��Ʈ��ũ �踮��� ����ȭ ���� ������ �Ƿ� �ؼ� �Ϻη� 0.5�� ��ٸ�. �׷����� �ϵ� �ڵ��� �ȵȴ�.. ������ ������!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                    // �÷��̾ ��ư�� �������� �� ���������� ����
                    if (IsClientSendCallBack) // �ݹ��� ���� ��
                    {
                        if (IsStand) // ���ĵ� ��ư�� �����ٸ�
                        {
                            CoreGameManager.Instance.AllPlayers[pop].playerCards.ChangeIsStandClientRpc(true);
                            IsClientSendCallBack = true; // Ŭ���̾�Ʈ�� �ݹ��� �� �������� true
                        }
                        else // ��Ʈ�� �����ٸ� ��Ʈ ī�带 ��Ű�� �ݹ��� ��ٸ���
                        {
                            IsClientSendCallBack = false;
                            CoreGameManager.Instance.AllPlayers[pop].playerCards.HitCardClientRpc(CoreGameManager.Instance.AllPlayers[pop].playerCards.hitCard.Value);
                        }
                    }
                    else // Ÿ�Ӿƿ� ��
                    {
                        Debug.Log("Ÿ�Ӿƿ���");
                        IsStand = true; // ���ĵ� ó��
                        CoreGameManager.Instance.AllPlayers[pop].playerCards.ChangeIsStandClientRpc(true);

                        IsClientSendCallBack = true; // Ŭ���̾�Ʈ�� �ݹ��� �� �������� true
                    }
                    yield return new WaitUntil(() => IsClientSendCallBack); // �ݹ��� �� ������ ��ٸ���. ī�� ����Ʈ ��� �� �ʿ�
                    RequestHitOrStandClientRpc(false, false, false, rpcParams); // ��Ʈ�ߴ� �÷��̾��� ��Ʈ ���� ����

                    // ���ĵ��� Ŭ���̾�Ʈ�� ���� ť�� �����Ͽ� �����ϱ�
                    if (IsStand)
                    {
                        standQueue.Enqueue(pop);
                    }
                    else
                    {
                        cycleQueue.Enqueue(pop);
                    }

                    // �ݹ� ������ �ʱ�ȭ
                    IsClientSendCallBack = false;
                    IsStand = false;
                }
                cycleCount++;
            }

            // ��Ʈ ������ ����
            SetCurrentHitPlayerInfoClientRpc(false); // ���� ��Ʈ ���� �÷��̾� �����
            RequestCountRemainTimeClientRpc(false); // ���� �ð� UI �����
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // ���� ��� ������ ����

            // ���� ��� ����� ���۵��� ǥ���ϰ� ��û�ϱ�
            RequestDisplayPhraseClientRpc($"�̹� ���� ���ڴ�...");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI�� ������ ��û�ϱ�
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // ���� ����
            List<ulong> winners = new();
            int winnerScore = int.MaxValue;
            int winnerDamageMP = 0;
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                if (Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value) < winnerScore)
                {
                    winnerScore = Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value);
                    winners.Clear();
                    winners.Add(clientId);
                }
                else if (Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value) == winnerScore)
                {
                    winners.Add(clientId);
                }
            }
            foreach (ulong winnerId in winners) // ���� �� ���� ���� �ɷ� ������ ���� ����
            {
                if (CoreGameManager.Instance.AllPlayers[winnerId].playerStats.Determination.Value > winnerDamageMP)
                {
                    winnerDamageMP = CoreGameManager.Instance.AllPlayers[winnerId].playerStats.Determination.Value;
                }
            }
            if (winnerScore == 0) // ������ ���ھ 0�̶��, �� Ŭ�� ��Ʈ��� ������ ���� 2��
            {
                winnerDamageMP *= 2;
            }

            // �й��� ����Ʈ �����
            List<ulong> loser = new();
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                if (!winners.Contains(clientId))
                {
                    loser.Add(clientId);
                }
            }

            // ���� ��ǥ
            string winnerText = "";
            foreach (ulong winnerId in winners)
            {
                winnerText += $"{CoreGameManager.Instance.AllPlayers[winnerId].NickName.Value}, ";
            }
            winnerText = winnerText.TrimEnd(',', ' ');
            RequestDisplayPhraseClientRpc($"{winnerText}��(��) �¸��߽��ϴ�!");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI�� ������ ��û�ϱ�
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // ������ �¸� �� ī��ȿ�� �ߵ�
            foreach (ulong winnerId in winners)
            {
                List<CardEffectTask> winnerCardEffectTasks = new();
                foreach (Card card in CoreGameManager.Instance.AllPlayers[winnerId].playerCards.StackCards)
                {
                    CardEffect cardEffect = CardEffectList.FindCardEffectToKey(card.CardEffectKey.ToString());
                    if (cardEffect.Type == CardEffectTypeEnum.RoundEnd)
                    {
                        winnerCardEffectTasks.AddRange(cardEffect.ActiveCardEffect(CoreGameManager.Instance.AllPlayers[winnerId]));
                    }
                }
                CoreGameManager.Instance.ExecuteCardEffectsServerRpc(winnerCardEffectTasks.ToArray()); // �۾� ����Ʈ�� ������ �������� ����ǰ� �����
                yield return new WaitUntil(() => IsClientSendCallBack); // �ݹ��� �� ������ ��ٸ���. ī�� ����Ʈ ���� ��.
                IsClientSendCallBack = false; // �ݹ� ���� �ʱ�ȭ
            }

            // �й��� û�� �ð�
            RequestDisplayPhraseClientRpc($"�й��� û�� �ð�");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI�� ������ ��û�ϱ�
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // �й��ڸ� �����ϰ� �й����� ī�� ȿ�� �ߵ���Ű��
            foreach (ulong clientId in loser)
            {
                List<CardEffectTask> loserCardEffectTasks = new(); // �й����� �½�ũ

                // �й��� ���� �ִ��� �½�ũ�� �߰�
                int damage = Mathf.Abs(Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value) - winnerScore) * winnerDamageMP; // ������ ���� ���� ������ ���� ���ϱ�!
                if (CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value > CoreGameManager.Instance.TargetNumber.Value) { damage *= 2; } // ����Ʈ��� �޴� �������� 2���
                loserCardEffectTasks.Add(new CardEffectTask(EffectTypeEnum.LoseDamage, NetworkManager.Singleton.LocalClientId, new ulong[] {clientId}, damage));

                // �й� ī�� ȿ���� �½�ũ�� �߰�
                foreach (Card card in CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackCards)
                {
                    CardEffect cardEffect = CardEffectList.FindCardEffectToKey(card.CardEffectKey.ToString());
                    if (cardEffect.Type == CardEffectTypeEnum.RoundEnd)
                    {
                        loserCardEffectTasks.AddRange(cardEffect.ActiveCardEffect(CoreGameManager.Instance.AllPlayers[clientId]));
                    }
                }
                CoreGameManager.Instance.ExecuteCardEffectsServerRpc(loserCardEffectTasks.ToArray()); // �۾� ����Ʈ�� ������ �������� ����ǰ� �����
                yield return new WaitUntil(() => IsClientSendCallBack); // �ݹ��� �� ������ ��ٸ���. ī�� ����Ʈ ���� ��.
                IsClientSendCallBack = false; // �ݹ� ���� �ʱ�ȭ
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // ���� �ʱ�ȭ
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                CoreGameManager.Instance.AllPlayers[clientId].playerCards.InitStackClientRpc();
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);

            CurrentRound.Value++;
            StartRound();
        }
    }
    #endregion
    #region UI ǥ�� ����
    [ClientRpc]
    public void RequestDisplayPhraseClientRpc(string phrase, ClientRpcParams rpcParams = default)
    {
        InGameUIManager.Instance.DisplayPhrase(phrase);
    }
    [ClientRpc]
    public void RequestHidePhraseClientRpc(ClientRpcParams rpcParams = default)
    {
        InGameUIManager.Instance.EndDisplayPhrase();
    }
    #endregion
    #region ��Ʈ �� ���ĵ� ����
    [ClientRpc]
    void ActiveHitPlayerInfoTitleClientRpc(bool active) // ��Ʈ ������ �� ���� ��Ʈ ���� �÷��̾� Ÿ��Ʋ�� ���� �ð� Ÿ��Ʋ�� �̸� Ȱ��ȭ�ϱ�
    {
        currentHitPlayerTitle.SetActive(active);
        remainTimeTitle.SetActive(active);
    }
    [ClientRpc]
    void SetCurrentHitPlayerInfoClientRpc(bool active, string nickName = "") // ���� ��Ʈ ���� �÷��̾ ǥ��
    {
        currentHitPlayerText.SetActive(active);
        currentHitPlayerText.GetComponent<TMP_Text>().text = nickName;
    }
    Tween remainTimeTween;
    [ClientRpc]
    void RequestCountRemainTimeClientRpc(bool active, int time = 0) // ���� �ð� ǥ��
    {
        remainTimeText.SetActive(active);

        if (remainTimeTween != null)
        {
            remainTimeTween.Kill(); // ���� ��Ʈ�� �ߴ�
        }

        if (time == 0)
        {
            remainTimeText.GetComponent<TMP_Text>().text = "-"; 
        }
        else
        {
            int currentNumber = time;
            remainTimeTween = DOTween.To
                (
                    () => currentNumber,
                    x => { currentNumber = x; remainTimeText.GetComponent<TMP_Text>().text = currentNumber.ToString(); },
                    0,
                    time
                ).SetEase(Ease.Linear);
        }
    }
    [ClientRpc]
    void RequestHitOrStandClientRpc(bool isDirectHit, bool hitActive, bool btnActive, ClientRpcParams rpcParams) // ���� ��Ʈ�ؾ��� �÷��̾�� �˸��� ��Ʈ�� ���ĵ� ��ư Ȱ��ȭ���ֱ�
    {
        PlayerController.LocalInstance.playerCards.IsDirectHitOrStandCycle = isDirectHit;
        PlayerController.LocalInstance.playerCards.IsHitOrStandCycle = hitActive;
        hitBtn.SetActive(btnActive);
        standBtn.SetActive(btnActive);
    }
    [ServerRpc(RequireOwnership = false)]
    public void NotifyClientStandServerRpc(ServerRpcParams rpcParams = default) // Ŭ���̾�Ʈ�� ���ĵ����� �� �˷��ִ� �޼���
    {
        IsStand = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientSendCallBackServerRpc(ServerRpcParams rpcParams = default) // Ŭ���̾�Ʈ�� ���� �Ϸ��ϸ� �˸��� �޼���
    {
        IsClientSendCallBack = true;
    }
    #endregion

    void SendPresentInfoToClient(RoundRewardEnum roundReward)
    {

    }
    List<string> SelectPresent(RoundRewardEnum roundReward)
    {
        return null;
    }
    [ClientRpc]
    void RequestDisplayPresentOrLootClientRpc(ClientRpcParams clientRpcParams)
    {

    }
    [ClientRpc]
    void RequestDisplayUIClientRpc(string uiTitle)
    {

    }
    [ClientRpc]
    void RequestEndDisplayUIClientRpc()
    {

    }
    void OnClothoPhase()
    {

    }
    void OnLachesisPhase()
    {

    }
    void OnAtroposPhase()
    {

    }
    void RoundEnd()
    {

    }

    void RequestChooseLoot()
    {

    }
    void RequestChoosePresent()
    {

    }
    void RequestDrawCards()
    {

    }
}
