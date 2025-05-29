using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EvenOrOddCE : CardEffect
{
    public EvenOrOddCE() : base
        (
            "EvenOrOdd",
            "¦Ȧ",
            "��Ʈ �� : ���� �� ������ ���� ¦����� -2, Ȧ����� +3�� �մϴ�.",
            CardEffectRankEnum.Void, 
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        if (player.playerCards.StackSum.Value == 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, -2)); // ���� �߰��ϴ� �۾�
        }
        else if (player.playerCards.StackSum.Value % 2 == 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, -2));
        }
        else if (player.playerCards.StackSum.Value % 2 != 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 3));
        }

        return tasks; // �۾� ����Ʈ�� ������ �������� ����ǰ� �����
    }
}