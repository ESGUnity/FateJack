using System;
using UnityEngine;

public class S_FoeObject : MonoBehaviour
{
    [Header("적 정보 클래스")]
    public S_Foe FoeInfo;

    [Header("체력")]
    public int MaxHealth;
    public int OldHealth;
    public int CurrentHealth;

    public void SetCreatureInfo(S_Foe info, int health) // 피조물 정보 설정
    {
        FoeInfo = info;
        MaxHealth = health;
        OldHealth = MaxHealth;
        CurrentHealth = MaxHealth;
    }
}
