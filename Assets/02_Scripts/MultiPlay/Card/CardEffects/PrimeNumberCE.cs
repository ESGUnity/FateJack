using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PrimeNumberCE : CardEffect
{
    public PrimeNumberCE() : base
        (
            "PrimeNumber",
            "������ �ѹ�", 
            "��Ʈ �� : ���� �� ������ ���� ��ǥ ������ ���� ������������ +2,  -5, +11 �� �ϳ��� �� ���ÿ� ����մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ
        int a = Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - player.playerCards.StackSum.Value + 2);
        int b = Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - player.playerCards.StackSum.Value - 5);
        int c = Mathf.Abs(CoreGameManager.Instance.TargetNumber.Value - player.playerCards.StackSum.Value + 11);

        int[] array = new int[] { a, b, c };
        int minNum = Mathf.Min(array);

        if (minNum == a)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 2));
        }
        else if (minNum == b)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, -5));
        }
        else if (minNum == c)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 11));
        }

        return tasks;
    }
}
