using System;
using System.Net;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerStats : NetworkBehaviour
{
    // 네트워크 관리 변수
    [HideInInspector] public NetworkVariable<int> Health = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // 초기값 : 100
    [HideInInspector] public NetworkVariable<int> Determination = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // 초기값 : 0
    [HideInInspector] public NetworkVariable<int> Gold = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // 초기값 : 0

    // 시각효과 관련
    AsyncOperationHandle<GameObject> getDamageMPVFXOpHandle;
    AsyncOperationHandle<GameObject> getGoldVFXOpHandle;


    #region 각종 스탯 값 변화 시
    [ClientRpc]
    public void DamagedClientRpc(int damage)
    {
        if (IsOwner)
        {
            // 데미지를 입어 패널이 흔들리거나 파티클이 튀는 효과 로직
            Health.Value -= damage;
        }
    }
    [ClientRpc]
    public void HealedClientRpc(int heal)
    {
        if (IsOwner)
        {
            // 회복되는 파티클 등이 나오는 시각 효과 로직
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

        // 데미지 배율 얻는 효과
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
    #region 잡다한 메서드

    #endregion
    void OnDisable()
    {
        getDamageMPVFXOpHandle.Completed -= GetDamageMPVFXLoadComplete;
    }
}
