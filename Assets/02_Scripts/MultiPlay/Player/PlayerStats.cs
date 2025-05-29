using System;
using System.Net;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerStats : NetworkBehaviour
{
    // ��Ʈ��ũ ���� ����
    [HideInInspector] public NetworkVariable<int> Health = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // �ʱⰪ : 100
    [HideInInspector] public NetworkVariable<int> Determination = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // �ʱⰪ : 0
    [HideInInspector] public NetworkVariable<int> Gold = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // �ʱⰪ : 0

    // �ð�ȿ�� ����
    AsyncOperationHandle<GameObject> getDamageMPVFXOpHandle;
    AsyncOperationHandle<GameObject> getGoldVFXOpHandle;


    #region ���� ���� �� ��ȭ ��
    [ClientRpc]
    public void DamagedClientRpc(int damage)
    {
        if (IsOwner)
        {
            // �������� �Ծ� �г��� ��鸮�ų� ��ƼŬ�� Ƣ�� ȿ�� ����
            Health.Value -= damage;
        }
    }
    [ClientRpc]
    public void HealedClientRpc(int heal)
    {
        if (IsOwner)
        {
            // ȸ���Ǵ� ��ƼŬ ���� ������ �ð� ȿ�� ����
            Health.Value += heal;
        }
    }
    [ClientRpc]
    public void ChangeGoldClientRpc(int gold)
    {
        if (IsOwner)
        {
            Gold.Value += gold;
        }

        GoldVFX(gold);
    }
    async void GoldVFX(int gold)
    {
        if (gold > 0)
        {
            int delay = 1000 / gold;
            for (int i = 0; i < gold; i++)
            {
                getGoldVFXOpHandle = Addressables.LoadAssetAsync<GameObject>("GetGoldVFXPrefab");
                getGoldVFXOpHandle.Completed += GetGoldVFXLoadComplete;

                await Task.Delay(delay);
            }
        }
    }
    void GetGoldVFXLoadComplete(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = Instantiate(handle.Result, new Vector3(GetComponent<PlayerController>().PlayerCharacterPos.Value.x, handle.Result.transform.position.y, GetComponent<PlayerController>().PlayerCharacterPos.Value.z), handle.Result.transform.rotation);
            go.GetComponent<GetGoldVFX>().TriggerVFX();
        }
    }

    [ClientRpc]
    public void ChangeDamageMPClientRpc(int damageMP)
    {
        if (IsOwner)
        {
            Determination.Value += damageMP;
        }

        // ������ ���� ��� ȿ��
        if (damageMP > 0)
        {
            getDamageMPVFXOpHandle = Addressables.LoadAssetAsync<GameObject>("GetDamageMPVFXPrefab");
            getDamageMPVFXOpHandle.Completed += GetDamageMPVFXLoadComplete;
        }
    }
    void GetDamageMPVFXLoadComplete(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject go = Instantiate(handle.Result, GetComponent<PlayerController>().PlayerCharacterPos.Value, handle.Result.transform.rotation);
            go.GetComponent<GetDamageMPVFX>().TriggerVFX();
        }
    }
    #endregion
    #region ����� �޼���

    #endregion
    void OnDisable()
    {
        getDamageMPVFXOpHandle.Completed -= GetDamageMPVFXLoadComplete;
    }
}
