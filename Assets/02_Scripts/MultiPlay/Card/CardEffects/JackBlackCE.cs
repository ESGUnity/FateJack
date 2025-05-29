using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class JackBlackCE : CardEffect
{
    public JackBlackCE() : base
        (
            "JackBlack",
            "�� ��",
            "�й� �� : ���� �� ������ ���� 12��� ���� ������ ��� �÷��̾�� 12�� �������� �ְ� ü���� 12 ȸ���մϴ�.",
            CardEffectRankEnum.Void, 
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        if (player.playerCards.StackSum.Value == 12)
        {
            List<ulong> targetClients = new(); // ī���� ȿ���� ������ Ÿ�� Ŭ���̾�Ʈ ����Ʈ
            foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager�� AllPlayers �� �ڽ��� ������ Ŭ���̾�Ʈ ���̵� Ÿ�� Ŭ���̾�Ʈ ����Ʈ�� �Ҵ�
            {
                if (client != NetworkManager.Singleton.LocalClientId)
                {
                    targetClients.Add(client);
                }
            }

            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), 12)); // Ÿ�� Ŭ���̾�Ʈ�鿡�� ������ �ִ� �۾�
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 12)); // �� �����θ� ȸ���ϴ� �۾�
        }

        return tasks;
    }
}