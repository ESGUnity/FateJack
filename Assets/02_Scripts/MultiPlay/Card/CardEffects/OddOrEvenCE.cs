using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OddOrEvenCE : CardEffect
{
    public OddOrEvenCE() : base
        (
            "OddOrEven",
            "Ȧ¦",
            "��Ʈ �� : ���� �� ������ ���� Ȧ����� +1, ¦����� +2�� �մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        if (player.playerCards.StackSum.Value == 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 2)); // ���� �߰��ϴ� �۾�
        }
        else if (player.playerCards.StackSum.Value % 2 == 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 2)); 
        }
        else if (player.playerCards.StackSum.Value % 2 != 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 1)); 
        }

        return tasks; // �۾� ����Ʈ�� ������ �������� ����ǰ� �����
    }
}
