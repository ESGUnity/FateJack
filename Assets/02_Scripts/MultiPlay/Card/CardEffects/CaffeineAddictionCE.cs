using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CaffeineAddictionCE : CardEffect
{
    public CaffeineAddictionCE() : base
        (
            "CaffeineAddiction",
            "ī���� �ߵ�",
            "�й� �� : �� ������ ���� 40 �̻��̶�� ü���� 40 ȸ���մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new();

        if (player.playerCards.StackSum.Value >= 40)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 40)); // �� �����θ� ȸ���ϴ� �۾�

        }

        return tasks;
    }
}
