using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IdleTransfigurationCE : CardEffect
{
    public IdleTransfigurationCE() : base
        (
            "IdleTransfiguration",
            "��������",
            "�й� �� : ������ ���� ���� ���� �÷��̾ �ϳ� �̻� �ִٸ� 4��带 ȹ���մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        foreach (var client in CoreGameManager.Instance.AllPlayers.Keys)
        {
            if (CoreGameManager.Instance.AllPlayers[client].playerCards.StackSum.Value == player.playerCards.StackSum.Value)
            {
                tasks.Add(new CardEffectTask(EffectTypeEnum.GetGold, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 4)); // �� �����θ� ȸ���ϴ� �۾�
                break;
            }
        }

        return tasks;
    }
}