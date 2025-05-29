using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GigaChadCE : CardEffect
{
    public GigaChadCE() : base
        (
            "GigaChad",
            "�Ⱑ ����",
            "�¸� �� : �� ü���� 20 ȸ���մϴ�. �� ���ÿ� ����߰� �ִٸ� ���� ������ ��� �÷��̾�� �������� 20 �ݴϴ�.",
            CardEffectRankEnum.Void, 
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 20)); // �� �����θ� ȸ���ϴ� �۾�

        foreach (Card card in player.playerCards.StackCards)
        {
            if (card.CardEffectKey == "Manppisppi")
            {
                List<ulong> targetClients = new(); // ī���� ȿ���� ������ Ÿ�� Ŭ���̾�Ʈ ����Ʈ
                foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager�� AllPlayers �� �ڽ��� ������ Ŭ���̾�Ʈ ���̵� Ÿ�� Ŭ���̾�Ʈ ����Ʈ�� �Ҵ�
                {
                    if (client != NetworkManager.Singleton.LocalClientId)
                    {
                        targetClients.Add(client);
                    }
                }

                tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), 20)); // Ÿ�� Ŭ���̾�Ʈ�鿡�� ������ �ִ� �۾�
                break;
            }
        }

        return tasks;
    }
}