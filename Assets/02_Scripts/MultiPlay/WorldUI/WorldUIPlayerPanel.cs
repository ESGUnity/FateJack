using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WorldUIPlayerPanel : NetworkBehaviour
{
    // 로컬 플레이어 컨트롤러
    PlayerController ownerPlayer; 

    // 스탯 관련 텍스트
    TMP_Text healthText;
    TMP_Text determinationText;
    TMP_Text goldText;
    TMP_Text stackSumText;

    // 플레이어 정보 관련 텍스트
    TMP_Text nickNameText;
    TMP_Text readyStateText;
    SpriteRenderer playerIconSprite;

    // 인게임 정보 관련 텍스트 오브젝트
    GameObject standText;
    GameObject burstText;
    GameObject cleanHitText;

    // 준비 상태 관련
    bool previousReadyState;

    public override void OnNetworkSpawn() // 모든 클라이언트에게 공통으로 적용되야하기에 네트워크 스폰을 사용
    {
        // 하위 오브젝트 가져오기
        Transform[] transformComponents = GetComponentsInChildren<RectTransform>(true);
        TMP_Text[] tMP_TextComponents = GetComponentsInChildren<TMP_Text>(true);
        SpriteRenderer[] spriteRendererComponents = GetComponentsInChildren<SpriteRenderer>(true);

        // 스탯 관련 텍스트
        healthText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("HealthText"));
        determinationText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("DeterminationText"));
        goldText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("GoldText"));
        stackSumText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("StackSumText"));

        // 플레이어 정보 관련 텍스트
        nickNameText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("NickNameText"));
        readyStateText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("ReadyStateText"));
        playerIconSprite = Array.Find(spriteRendererComponents, c => c.gameObject.name.Equals("PlayerIconSprite"));

        // 인게임 정보 관련 텍스트
        standText = Array.Find(transformComponents, c => c.gameObject.name.Equals("StandText")).gameObject;
        burstText = Array.Find(transformComponents, c => c.gameObject.name.Equals("BurstText")).gameObject;
        cleanHitText = Array.Find(transformComponents, c => c.gameObject.name.Equals("CleanHitText")).gameObject;

        // 별도의 ConnectedClientCallBack 구독
        CoreGameManager.Instance.ConnectedClientCallBack += SyncTextToConnectedClientClientRpc;
    }
    void Start() // Start는 스폰될 때 단 한 번 발동한다. 네트워크 스폰처럼 플레이어가 들어올 때마다 발동하는게 아닌 스폰될 때 딱 한 번 발동하면 되는 부분
    {
        SubscribeOwnerPlayerStatsAsync();
    }

    void Update()
    {
        if (IsOwner)
        {
            // 로비 상태에서 준비 및 대기 글자 띄우는 로직
            if (ownerPlayer != null)
            {
                bool currentReadyState = ownerPlayer.IsThisPlayerReady;

                // 값이 바뀌었을 때만 ServerRpc 호출
                if (currentReadyState != previousReadyState)
                {
                    string stateText = currentReadyState ? "준비" : "대기";
                    DisplayReadyStateServerRpc(stateText);
                    previousReadyState = currentReadyState; 
                }
            }
        }
    }

    #region 게임 시작 전 메서드
    public async void SubscribeOwnerPlayerStatsAsync() // 플레이어 패널의 오너 클라이언트 스탯 정보를 구독
    {
        if (IsOwner)
        {
            while (PlayerController.LocalInstance == null) // 플레이어 컨트롤러가 할당될 때까지 대기
            {
                await Task.Delay(500);
            }

            ownerPlayer = PlayerController.LocalInstance; // 클라이언트의 로컬 씬에만 존재하는 유일한 PlayerController를 할당

            // 모든 클라(오너 포함)의 패널에 있는 닉네임 및 준비 상태 텍스트를 동기화하는 로직
            DisplayNickNameServerRpc(ownerPlayer.NickName.Value.ToString());
            if (ownerPlayer.IsThisPlayerReady)
            {
                DisplayReadyStateServerRpc("준비");
            }
            else
            {
                DisplayReadyStateServerRpc("대기");
            }

            // 각 스탯 텍스트 구독
            ownerPlayer.playerStats.Health.OnValueChanged += DisplayHealthValueServerRpc;
            ownerPlayer.playerStats.Gold.OnValueChanged += DisplayGoldValueServerRpc;
            ownerPlayer.playerStats.Determination.OnValueChanged += DisplayDeterminationValueServerRpc;
            ownerPlayer.playerCards.StackSum.OnValueChanged += DisplayStackSumValueServerRpc;
            ownerPlayer.playerCards.IsStand.OnValueChanged += DisplayStandStateServerRpc;
            ownerPlayer.playerCards.IsBurst.OnValueChanged += DisplayBurstStateServerRpc;
            ownerPlayer.playerCards.IsCleanHit.OnValueChanged += DisplayCleanHitStateServerRpc;
        }
    }
    [ClientRpc]
    void SyncTextToConnectedClientClientRpc() // 입장 시 다른 클라이언트 씬에 있는 패널의 텍스트를 내 패널과 동기화
    {
        if (IsOwner)
        {
            DisplayNickNameServerRpc(PlayerController.LocalInstance.NickName.Value.ToString());

            if (PlayerController.LocalInstance.IsThisPlayerReady)
            {
                DisplayReadyStateServerRpc("준비");
            }
            else
            {
                DisplayReadyStateServerRpc("대기");
            }
        }
    }
    [ClientRpc]
    public void InitValueClientRpc()
    {
        healthText.text = "100";
        determinationText.text = "0";
        goldText.text = "0";
        stackSumText.text = "0";
        readyStateText.gameObject.SetActive(false);
    }
    #endregion
    #region UI 디스플레이 관련 메서드
    [ServerRpc(RequireOwnership = false)]
    void DisplayNickNameServerRpc(string nickName)
    {
        DisplayNickNameClientRpc(nickName);
    }
    [ClientRpc]
    void DisplayNickNameClientRpc(string nickName)
    {
        nickNameText.text = nickName;
    }

    [ServerRpc(RequireOwnership = false)]
    void DisplayReadyStateServerRpc(string readyState)
    {
        DisplayReadyStateClientRpc(readyState);
    }
    [ClientRpc]
    void DisplayReadyStateClientRpc(string readyState)
    {
        readyStateText.text = readyState;
    }

    [ServerRpc(RequireOwnership = false)]
    void DisplayHealthValueServerRpc(int oldValue, int newValue)
    {
        DisplayHealthValueClientRpc(oldValue, newValue);
    }
    [ClientRpc]
    void DisplayHealthValueClientRpc(int oldValue, int newValue)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; healthText.text = currentNumber.ToString(); },
                newValue,
                0.7f
            ).SetEase(Ease.OutQuart);
    }

    [ServerRpc(RequireOwnership = false)]
    void DisplayGoldValueServerRpc(int oldValue, int newValue)
    {
        DisplayGoldValueClientRpc(oldValue, newValue);
    }
    [ClientRpc]
    void DisplayGoldValueClientRpc(int oldValue, int newValue) // 골드의 경우 시각효과 때문에 0.5초 딜레이 존재
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; goldText.text = currentNumber.ToString(); },
                newValue,
                1f
            )
            .SetDelay(0.5f);
    }

    [ServerRpc(RequireOwnership = false)]
    void DisplayDeterminationValueServerRpc(int oldValue, int newValue)
    {
        DisplayDeterminationValueClientRpc(oldValue, newValue);
    }
    [ClientRpc]
    void DisplayDeterminationValueClientRpc(int oldValue, int newValue)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; determinationText.text = currentNumber.ToString(); },
                newValue,
                0.5f
            ).SetEase(Ease.OutQuart);
    }

    [ServerRpc(RequireOwnership = false)]
    void DisplayStackSumValueServerRpc(int oldValue, int newValue)
    {
        DisplayStackSumValueClientRpc(oldValue, newValue);
    }
    [ClientRpc]
    void DisplayStackSumValueClientRpc(int oldValue, int newValue)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; stackSumText.text = currentNumber.ToString(); },
                newValue,
                0.4f
            ).SetEase(Ease.OutQuart);
    }

    [ServerRpc(RequireOwnership = false)]
    void DisplayStandStateServerRpc(bool oldValue, bool newValue)
    {
        DisplayStandStateClientRpc(oldValue, newValue);
    }
    [ClientRpc]
    void DisplayStandStateClientRpc(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            standText.SetActive(true);
        }
        else
        {
            standText.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DisplayBurstStateServerRpc(bool oldValue, bool newValue)
    {
        DisplayBurstStateClientRpc(oldValue, newValue);
    }
    [ClientRpc]
    void DisplayBurstStateClientRpc(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            burstText.SetActive(true);
        }
        else
        {
            burstText.SetActive(false);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void DisplayCleanHitStateServerRpc(bool oldValue, bool newValue)
    {
        DisplayCleanHitStateClientRpc(oldValue, newValue);
    }
    [ClientRpc]
    void DisplayCleanHitStateClientRpc(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            cleanHitText.SetActive(true);
        }
        else
        {
            cleanHitText.SetActive(false);
        }
    }
    #endregion

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            // 구독한 것들 취소
            ownerPlayer.playerStats.Health.OnValueChanged -= DisplayHealthValueServerRpc;
            ownerPlayer.playerStats.Gold.OnValueChanged -= DisplayGoldValueServerRpc;
            ownerPlayer.playerStats.Determination.OnValueChanged -= DisplayDeterminationValueServerRpc;
            ownerPlayer.playerCards.StackSum.OnValueChanged -= DisplayStackSumValueServerRpc;
            ownerPlayer.playerCards.IsStand.OnValueChanged -= DisplayStandStateServerRpc;
            ownerPlayer.playerCards.IsBurst.OnValueChanged -= DisplayBurstStateServerRpc;
            ownerPlayer.playerCards.IsCleanHit.OnValueChanged -= DisplayCleanHitStateServerRpc;
            CoreGameManager.Instance.ConnectedClientCallBack += SyncTextToConnectedClientClientRpc;
        }
    }
}
