using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SympathyCE : CardEffect
{
    public SympathyCE() : base
        (
            "Sympathy",
            "����",
            "��Ʈ �� : ���� �� ������ ���� ��ǥ �������� �۴ٸ� �� ������ �տ� +2�� �մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        if (player.playerCards.StackSum.Value < CoreGameManager.Instance.TargetNumber.Value)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 2));
        }

        return tasks;
    }
}
