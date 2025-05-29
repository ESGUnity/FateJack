using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PercyJacksonCE : CardEffect
{
    public PercyJacksonCE() : base
        (
            "PercyJackson",
            "�۽� �轼",
            "�¸� �� : ���� ������ ��� �÷��̾��� ������ ������ 1 ��Ĩ�ϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        List<ulong> targetClients = new(); // ī���� ȿ���� ������ Ÿ�� Ŭ���̾�Ʈ ����Ʈ
        foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager�� AllPlayers �� �ڽ��� ������ Ŭ���̾�Ʈ ���̵� Ÿ�� Ŭ���̾�Ʈ ����Ʈ�� �Ҵ�
        {
            if (client != NetworkManager.Singleton.LocalClientId)
            {
                targetClients.Add(client);
            }
        }

        tasks.Add(new CardEffectTask(EffectTypeEnum.StealDamageMP, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), 1));

        return tasks;
    }
}