using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MutualAidCE : CardEffect
{
    public MutualAidCE() : base
        (
            "MutualAid",
            "ǰ����",
            "��Ʈ �� : ��� �÷��̾��� ���ÿ� +1�� �մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        List<ulong> targetClients = new(); // ī���� ȿ���� ������ Ÿ�� Ŭ���̾�Ʈ ����Ʈ
        foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager�� AllPlayers �� �ڽ��� ������ Ŭ���̾�Ʈ ���̵� Ÿ�� Ŭ���̾�Ʈ ����Ʈ�� �Ҵ�
        {
            targetClients.Add(client);
        }

        tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), 1));

        return tasks; // �۾� ����Ʈ�� ������ �������� ����ǰ� �����
    }
}