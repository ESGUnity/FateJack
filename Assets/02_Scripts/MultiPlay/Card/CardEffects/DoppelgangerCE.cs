using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DoppelgangerCE : CardEffect
{
    public DoppelgangerCE() : base
        (
            "Doppelganger",
            "���ð���",
            "�¸� Ȥ�� �й� �� : ������ ���� ���� ���� �÷��̾� �ϳ� �� ü���� 30 ȸ���մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        )
    { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        // ȸ���� ���ϱ�
        int heal = 0;
        foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager�� AllPlayers �� �ڽ��� ������ Ŭ���̾�Ʈ ���̵� Ÿ�� Ŭ���̾�Ʈ ����Ʈ�� �Ҵ�
        {
            if (CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value == player.playerCards.StackSum.Value)
            {
                heal += 30;
            }
        }

        // CoreGameManager�� ȣ��
        if (heal > 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, heal)); // �� �����θ� ȸ���ϴ� �۾�
            
        }

        return tasks;
    }
}