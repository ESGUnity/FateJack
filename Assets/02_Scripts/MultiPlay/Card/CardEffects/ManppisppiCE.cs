using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ManppisppiCE : CardEffect
{
    public ManppisppiCE() : base
        (
            "Manppisppi",
            "�����",
            "�й� �� : �� ü���� 20 ȸ���մϴ�. �� ���ÿ� �Ⱑ ���尡 �ִٸ� �߰��� ü���� 40 ȸ���մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 20)); // �� �����θ� ȸ���ϴ� �۾�

        foreach (Card card in player.playerCards.StackCards)
        {
            if (card.CardEffectKey == "GigaChad")
            {
                tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 40)); // �� �����θ� ȸ���ϴ� �۾�
                break;
            }
        }

        return tasks;
    }
}