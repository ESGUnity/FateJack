using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WorldUIPlayerPanel : NetworkBehaviour
{
    // ���� �÷��̾� ��Ʈ�ѷ�
    PlayerController ownerPlayer; 

    // ���� ���� �ؽ�Ʈ
    TMP_Text healthText;
    TMP_Text determinationText;
    TMP_Text goldText;
    TMP_Text stackSumText;

    // �÷��̾� ���� ���� �ؽ�Ʈ
    TMP_Text nickNameText;
    TMP_Text readyStateText;
    SpriteRenderer playerIconSprite;

    // �ΰ��� ���� ���� �ؽ�Ʈ ������Ʈ
    GameObject standText;
    GameObject burstText;
    GameObject cleanHitText;

    // �غ� ���� ����
    bool previousReadyState;

    public override void OnNetworkSpawn() // ��� Ŭ���̾�Ʈ���� �������� ����Ǿ��ϱ⿡ ��Ʈ��ũ ������ ���
    {
        // ���� ������Ʈ ��������
        Transform[] transformComponents = GetComponentsInChildren<RectTransform>(true);
        TMP_Text[] tMP_TextComponents = GetComponentsInChildren<TMP_Text>(true);
        SpriteRenderer[] spriteRendererComponents = GetComponentsInChildren<SpriteRenderer>(true);

        // ���� ���� �ؽ�Ʈ
        healthText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("HealthText"));
        determinationText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("DeterminationText"));
        goldText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("GoldText"));
        stackSumText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("StackSumText"));

        // �÷��̾� ���� ���� �ؽ�Ʈ
        nickNameText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("NickNameText"));
        readyStateText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("ReadyStateText"));
        playerIconSprite = Array.Find(spriteRendererComponents, c => c.gameObject.name.Equals("PlayerIconSprite"));

        // �ΰ��� ���� ���� �ؽ�Ʈ
        standText = Array.Find(transformComponents, c => c.gameObject.name.Equals("StandText")).gameObject;
        burstText = Array.Find(transformComponents, c => c.gameObject.name.Equals("BurstText")).gameObject;
        cleanHitText = Array.Find(transformComponents, c => c.gameObject.name.Equals("CleanHitText")).gameObject;

        // ������ ConnectedClientCallBack ����
        CoreGameManager.Instance.ConnectedClientCallBack += SyncTextToConnectedClientClientRpc;
    }
    void Start() // Start�� ������ �� �� �� �� �ߵ��Ѵ�. ��Ʈ��ũ ����ó�� �÷��̾ ���� ������ �ߵ��ϴ°� �ƴ� ������ �� �� �� �� �ߵ��ϸ� �Ǵ� �κ�
    {
        SubscribeOwnerPlayerStatsAsync();
    }

    void Update()
    {
        if (IsOwner)
        {
            // �κ� ���¿��� �غ� �� ��� ���� ���� ����
            if (ownerPlayer != null)
            {
                bool currentReadyState = ownerPlayer.IsThisPlayerReady;

                // ���� �ٲ���� ���� ServerRpc ȣ��
                if (currentReadyState != previousReadyState)
                {
                    string stateText = currentReadyState ? "�غ�" : "���";
                    DisplayReadyStateServerRpc(stateText);
                    previousReadyState = currentReadyState; 
                }
            }
        }
    }

    #region ���� ���� �� �޼���
    public async void SubscribeOwnerPlayerStatsAsync() // �÷��̾� �г��� ���� Ŭ���̾�Ʈ ���� ������ ����
    {
        if (IsOwner)
        {
            while (PlayerController.LocalInstance == null) // �÷��̾� ��Ʈ�ѷ��� �Ҵ�� ������ ���
            {
                await Task.Delay(500);
            }

            ownerPlayer = PlayerController.LocalInstance; // Ŭ���̾�Ʈ�� ���� ������ �����ϴ� ������ PlayerController�� �Ҵ�

            // ��� Ŭ��(���� ����)�� �гο� �ִ� �г��� �� �غ� ���� �ؽ�Ʈ�� ����ȭ�ϴ� ����
            DisplayNickNameServerRpc(ownerPlayer.NickName.Value.ToString());
            if (ownerPlayer.IsThisPlayerReady)
            {
                DisplayReadyStateServerRpc("�غ�");
            }
            else
            {
                DisplayReadyStateServerRpc("���");
            }

            // �� ���� �ؽ�Ʈ ����
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
    void SyncTextToConnectedClientClientRpc() // ���� �� �ٸ� Ŭ���̾�Ʈ ���� �ִ� �г��� �ؽ�Ʈ�� �� �гΰ� ����ȭ
    {
        if (IsOwner)
        {
            DisplayNickNameServerRpc(PlayerController.LocalInstance.NickName.Value.ToString());

            if (PlayerController.LocalInstance.IsThisPlayerReady)
            {
                DisplayReadyStateServerRpc("�غ�");
            }
            else
            {
                DisplayReadyStateServerRpc("���");
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
    #region UI ���÷��� ���� �޼���
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
    void DisplayGoldValueClientRpc(int oldValue, int newValue) // ����� ��� �ð�ȿ�� ������ 0.5�� ������ ����
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
            // ������ �͵� ���
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
