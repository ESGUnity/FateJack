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
    // 게임 시작 전 로비
    List<ulong> readyPlayers = new(); // 준비 완료한 클라이언트 아이디
    [HideInInspector] public event Action InGameStart; // 게임 시작 시 Invoke하는 델리게이트

    // 라운드 시간
    const float CURRENT_ROUND_UI_LIFE_TIME = 3f; // 라운드 UI의 생존 시간
    const float CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME = 0.3f; // UI의 등장 및 퇴장에 걸리는 시간
    const float TERM_TIME = 1f; // 약간의 릴렉스 시간(카드를 받고나서 히트로 넘어가거나 모두가 스탠드하고 승자 결정 단계에 넘어가는 등)
    const float TOSS_CARD_TIME = 0.5f; // 카드 한 장을 토스해주는 시간
    const float HIT_TIME = 20f; // 히트하는데 걸리는 시간
    const float ChoicePresentTime = 40f; // 선물 고를 시간
    const float PurchaseLootTime = 60f; // 전리품 구매 시간

    // 라운드 시작 시 스탯 증가량
    const int DETERMINATION_AMOUNT_ON_ROUND_START = 2; // 의지 증가량
    const int GOLD_AMOUNT_ON_ROUND_START = 4; // 골드 증가량

    // 라운드 진행 관련
    [HideInInspector] public NetworkVariable<int> CurrentRound = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 1로 초기화
    [HideInInspector] public bool IsClientSendCallBack = false;
    [HideInInspector] public bool IsStand = false;
    //[HideInInspector] public NetworkVariable<int> GameFlowState = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 0으로 초기화

    // 선물 및 상점 관련
    RoundRewardEnum[] roundRewards = { RoundRewardEnum.ClothoPresent, RoundRewardEnum.LachesisPresent, RoundRewardEnum.AtroposPresent };

    // 연출 관련
    bool isCardTossed = false; // 라운드 시작 시 카드를 나눠주는 시각 효과에 사용되는 변수
    [SerializeField] GameObject remainTimeTitle;
    [SerializeField] GameObject remainTimeText;
    [SerializeField] GameObject currentHitPlayerTitle;
    [SerializeField] GameObject currentHitPlayerText;
    [SerializeField] GameObject hitBtn;
    [SerializeField] GameObject standBtn;

    // 싱글턴
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

    #region 게임 시작 전
    [ServerRpc(RequireOwnership = false)]
    public void NotifyPlayerReadyServerRpc(ServerRpcParams rpcParams = default) // 클라이언트가 서버에 준비 완료했다고 알리는 Rpc
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
    public void RequestStartGameServerRpc() // 호스트의 게임 시작 버튼이 호출하는 인게임 시작 메서드
    {
        if (readyPlayers.Count == CoreGameManager.Instance.AllPlayers.Keys.Count) // 준비한 클라와 연결된 클라가 같을 경우
        {
            CoreGameManager.Instance.StartAllPlayers(); // 각 클라이언트의 인게임 시작 시 로직을 실행하기
            InvokeInGameStartClientRpc(); // InGameStart 델리게이트 실행
            ActiveHitPlayerInfoTitleClientRpc(true); // 현재 히트 중인 플레이어 타이틀 표시
            CoreGameManager.Instance.ActiveTargetNumberTextClientRpc(); // 목표 점수 표시
            CoreGameManager.Instance.SetTargetNumberTextClientRpc(0, CoreGameManager.Instance.TargetNumber.Value); // 목표 점수 표시
            GetComponent<CardManager>().GenerateRootNumber(); // 루트 숫자 생성
            GetComponent<CardManager>().ActiveProbabilityTableClientRpc(); // 카드 효과 확률표 표시

            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys) // 연출관련
            {
                CoreGameManager.Instance.AllPlayers[clientId].playerCards.PlayerDeck.OnListChanged += PlayerDeckChangedCallBack;
            }

            StartRound(); // 본격적인 인게임 플로우 시작하기
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
    #region 게임 시작 시
    void StartRound()
    {
        if (IsServer)
        {
            if (CurrentRound.Value == 1) // 1 라운드 시 기본 요소 초기화
            {
                StartCoroutine(StartPhaseCoroutine(RoundRewardEnum.None));
            }
            else if (CurrentRound.Value % 2 == 0) // 짝수 라운드 시
            {
                //int randomIndex = UnityEngine.Random.Range(0, roundRewards.Length);
                //RoundRewardEnum reward = roundRewards[randomIndex];

                StartCoroutine(StartPhaseCoroutine(RoundRewardEnum.None));
            }
            else // 1 라운드를 제외한 홀수 라운드 시
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
            // 현재 라운드를 표시하게 요청하기
            RequestDisplayPhraseClientRpc($"{CurrentRound.Value} 라운드");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // 현재 라운드 표시 UI를 숨기라고 요청하기
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 카드 나눠주기
            int receiveCompleteCount = 0;
            while (receiveCompleteCount < CoreGameManager.Instance.AllPlayers.Keys.Count) // 카드를 다 받은 플레이어 숫자와 모든 플레이어 숫자가 같으면 반복문 종료
            {
                foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
                {
                    if (CoreGameManager.Instance.AllPlayers[clientId].playerCards.PlayerDeck.Count < 12)
                    {
                        GetComponent<CardManager>().TossCardToPlayerClientRpc(clientId);

                        // 실제로 카드를 받을 때까지 대기
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


            // 의지 제공
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                CoreGameManager.Instance.AllPlayers[clientId].playerStats.ChangeDamageMPClientRpc(DETERMINATION_AMOUNT_ON_ROUND_START);
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 골드 제공
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                CoreGameManager.Instance.AllPlayers[clientId].playerStats.ChangeGoldClientRpc(GOLD_AMOUNT_ON_ROUND_START);
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);




            // 히트 페이즈 시작

            // 히트 페이즈가 시작됨을 표시하게 요청하기
            RequestDisplayPhraseClientRpc($"히트 또는 스탠드!");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI를 숨기라고 요청하기
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 체력이 가장 높은 플레이어부터 시작. 1 - 1 라운드의 경우 무작위 플레이어 시작
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

            // 체력이 가장 높은 플레이어부터 들어온 순서대로 돌아가기
            int cycleCount = 1;
            Queue<ulong> cycleQueue = new();
            Queue<ulong> standQueue = new();

            // 먼저 시작하는 플레이어부터 순서대로 돌아가는 큐 만들기
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

                    // 현재 히트 중인 플레이어 UI 표시
                    RequestDisplayPhraseClientRpc($"나의 차례", rpcParams); // 나의 차례 상단 UI
                    SetCurrentHitPlayerInfoClientRpc(true, CoreGameManager.Instance.AllPlayers[pop].NickName.Value.ToString()); // 현재 히트 중인 플레이어 표시

                    // cycleCount에 따라서 히트 또는 다이렉트 히트를 클라이언트에 요청
                    float startTime = Time.time;
                    if (cycleCount < 3)
                    {
                        RequestHitOrStandClientRpc(true, true, true, rpcParams); // 다이렉트 히트 요청

                        RequestCountRemainTimeClientRpc(true, (int)HIT_TIME); // 타이머
                        yield return new WaitUntil(() => IsClientSendCallBack || Time.time >= startTime + HIT_TIME);
                    }
                    else
                    {
                        RequestHitOrStandClientRpc(false, true, true, rpcParams); // 그냥 히트 요청

                        RequestCountRemainTimeClientRpc(true, (int)HIT_TIME); // 타이머
                        yield return new WaitUntil(() => IsClientSendCallBack || Time.time >= startTime + HIT_TIME);
                    }

                    // UI 숨기기
                    RequestHidePhraseClientRpc(rpcParams); // 나의 차례 UI 숨기기
                    RequestCountRemainTimeClientRpc(true); // 남은 시간을 "-"로 표시하기
                    RequestHitOrStandClientRpc(true, true, false, rpcParams); // 버튼 숨기기
                    yield return new WaitForSecondsRealtime(0.5f); // TODO : 네트워크 배리어블 동기화 전에 실행이 되려 해서 일부러 0.5초 기다림. 그렇지만 하드 코딩은 안된다.. 의지를 가지렴!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                    // 플레이어가 버튼을 눌렀는지 안 눌렀는지에 따라
                    if (IsClientSendCallBack) // 콜백이 왔을 시
                    {
                        if (IsStand) // 스탠드 버튼을 눌렀다면
                        {
                            CoreGameManager.Instance.AllPlayers[pop].playerCards.ChangeIsStandClientRpc(true);
                            IsClientSendCallBack = true; // 클라이언트가 콜백할 게 없음으로 true
                        }
                        else // 히트를 눌렀다면 히트 카드를 시키고 콜백을 기다리기
                        {
                            IsClientSendCallBack = false;
                            CoreGameManager.Instance.AllPlayers[pop].playerCards.HitCardClientRpc(CoreGameManager.Instance.AllPlayers[pop].playerCards.hitCard.Value);
                        }
                    }
                    else // 타임아웃 시
                    {
                        Debug.Log("타임아웃함");
                        IsStand = true; // 스탠드 처리
                        CoreGameManager.Instance.AllPlayers[pop].playerCards.ChangeIsStandClientRpc(true);

                        IsClientSendCallBack = true; // 클라이언트가 콜백할 게 없음으로 true
                    }
                    yield return new WaitUntil(() => IsClientSendCallBack); // 콜백이 올 때까지 기다리기. 카드 이펙트 계산 등 필요
                    RequestHitOrStandClientRpc(false, false, false, rpcParams); // 히트했던 플레이어의 히트 권한 뺏기

                    // 스탠드인 클라이언트는 별도 큐에 저장하여 관리하기
                    if (IsStand)
                    {
                        standQueue.Enqueue(pop);
                    }
                    else
                    {
                        cycleQueue.Enqueue(pop);
                    }

                    // 콜백 변수들 초기화
                    IsClientSendCallBack = false;
                    IsStand = false;
                }
                cycleCount++;
            }

            // 히트 페이즈 종료
            SetCurrentHitPlayerInfoClientRpc(false); // 현재 히트 중인 플레이어 숨기기
            RequestCountRemainTimeClientRpc(false); // 남은 시간 UI 숨기기
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 스택 계산 페이즈 시작

            // 스택 계산 페이즈가 시작됨을 표시하게 요청하기
            RequestDisplayPhraseClientRpc($"이번 라운드 승자는...");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI를 숨기라고 요청하기
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 승자 선정
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
            foreach (ulong winnerId in winners) // 승자 중 제일 높은 걸로 데미지 배율 선정
            {
                if (CoreGameManager.Instance.AllPlayers[winnerId].playerStats.Determination.Value > winnerDamageMP)
                {
                    winnerDamageMP = CoreGameManager.Instance.AllPlayers[winnerId].playerStats.Determination.Value;
                }
            }
            if (winnerScore == 0) // 승자의 스코어가 0이라면, 즉 클린 히트라면 데미지 배율 2배
            {
                winnerDamageMP *= 2;
            }

            // 패배자 리스트 만들기
            List<ulong> loser = new();
            foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys)
            {
                if (!winners.Contains(clientId))
                {
                    loser.Add(clientId);
                }
            }

            // 승자 발표
            string winnerText = "";
            foreach (ulong winnerId in winners)
            {
                winnerText += $"{CoreGameManager.Instance.AllPlayers[winnerId].NickName.Value}, ";
            }
            winnerText = winnerText.TrimEnd(',', ' ');
            RequestDisplayPhraseClientRpc($"{winnerText}이(가) 승리했습니다!");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI를 숨기라고 요청하기
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 승자의 승리 시 카드효과 발동
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
                CoreGameManager.Instance.ExecuteCardEffectsServerRpc(winnerCardEffectTasks.ToArray()); // 작업 리스트를 보내어 서버에서 실행되게 만들기
                yield return new WaitUntil(() => IsClientSendCallBack); // 콜백이 올 때까지 기다리기. 카드 이펙트 진행 중.
                IsClientSendCallBack = false; // 콜백 변수 초기화
            }

            // 패배자 청산 시간
            RequestDisplayPhraseClientRpc($"패배자 청산 시간");
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_LIFE_TIME);

            // UI를 숨기라고 요청하기
            RequestHidePhraseClientRpc();
            yield return new WaitForSecondsRealtime(CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME);
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 패배자를 공격하고 패배자의 카드 효과 발동시키기
            foreach (ulong clientId in loser)
            {
                List<CardEffectTask> loserCardEffectTasks = new(); // 패배자의 태스크

                // 패배자 공격 애님을 태스크에 추가
                int damage = Mathf.Abs(Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value) - winnerScore) * winnerDamageMP; // 승자의 제일 높은 데미지 배율 곱하기!
                if (CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value > CoreGameManager.Instance.TargetNumber.Value) { damage *= 2; } // 버스트라면 받는 데미지를 2배로
                loserCardEffectTasks.Add(new CardEffectTask(EffectTypeEnum.LoseDamage, NetworkManager.Singleton.LocalClientId, new ulong[] {clientId}, damage));

                // 패배 카드 효과도 태스크에 추가
                foreach (Card card in CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackCards)
                {
                    CardEffect cardEffect = CardEffectList.FindCardEffectToKey(card.CardEffectKey.ToString());
                    if (cardEffect.Type == CardEffectTypeEnum.RoundEnd)
                    {
                        loserCardEffectTasks.AddRange(cardEffect.ActiveCardEffect(CoreGameManager.Instance.AllPlayers[clientId]));
                    }
                }
                CoreGameManager.Instance.ExecuteCardEffectsServerRpc(loserCardEffectTasks.ToArray()); // 작업 리스트를 보내어 서버에서 실행되게 만들기
                yield return new WaitUntil(() => IsClientSendCallBack); // 콜백이 올 때까지 기다리기. 카드 이펙트 진행 중.
                IsClientSendCallBack = false; // 콜백 변수 초기화
            }
            yield return new WaitForSecondsRealtime(TERM_TIME);

            // 스택 초기화
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
    #region UI 표시 관련
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
    #region 히트 및 스탠드 관련
    [ClientRpc]
    void ActiveHitPlayerInfoTitleClientRpc(bool active) // 히트 페이즈 시 현재 히트 중인 플레이어 타이틀과 남은 시간 타이틀을 미리 활성화하기
    {
        currentHitPlayerTitle.SetActive(active);
        remainTimeTitle.SetActive(active);
    }
    [ClientRpc]
    void SetCurrentHitPlayerInfoClientRpc(bool active, string nickName = "") // 현재 히트 중인 플레이어를 표시
    {
        currentHitPlayerText.SetActive(active);
        currentHitPlayerText.GetComponent<TMP_Text>().text = nickName;
    }
    Tween remainTimeTween;
    [ClientRpc]
    void RequestCountRemainTimeClientRpc(bool active, int time = 0) // 남은 시간 표시
    {
        remainTimeText.SetActive(active);

        if (remainTimeTween != null)
        {
            remainTimeTween.Kill(); // 기존 두트윈 중단
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
    void RequestHitOrStandClientRpc(bool isDirectHit, bool hitActive, bool btnActive, ClientRpcParams rpcParams) // 현재 히트해야할 플레이어에게 알리고 히트와 스탠드 버튼 활성화해주기
    {
        PlayerController.LocalInstance.playerCards.IsDirectHitOrStandCycle = isDirectHit;
        PlayerController.LocalInstance.playerCards.IsHitOrStandCycle = hitActive;
        hitBtn.SetActive(btnActive);
        standBtn.SetActive(btnActive);
    }
    [ServerRpc(RequireOwnership = false)]
    public void NotifyClientStandServerRpc(ServerRpcParams rpcParams = default) // 클라이언트가 스탠드했을 때 알려주는 메서드
    {
        IsStand = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientSendCallBackServerRpc(ServerRpcParams rpcParams = default) // 클라이언트가 무언갈 완료하면 알리는 메서드
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
