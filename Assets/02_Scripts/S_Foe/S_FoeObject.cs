using System;
using UnityEngine;

public class S_FoeObject : MonoBehaviour
{
    [Header("�� ���� Ŭ����")]
    public S_Foe FoeInfo;

    [Header("ü��")]
    public int MaxHealth;
    public int OldHealth;
    public int CurrentHealth;

    public void SetCreatureInfo(S_Foe info, int health) // ������ ���� ����
    {
        FoeInfo = info;
        MaxHealth = health;
        OldHealth = MaxHealth;
        CurrentHealth = MaxHealth;
    }
}
