using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OverthinkingAddictionCE : CardEffect
{
    public OverthinkingAddictionCE() : base
        (
            "OverthinkingAddiction",
            "��� �ߵ�",
            "��Ʈ �� : ���� �� ���ÿ� ���ڰ� Ȧ���� ī�� �ϳ� �� +1�� �մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        int oddCount = 0;
        foreach (Card card in player.playerCards.StackCards)
        {
            if (card.Number % 2 != 0)
            {
                oddCount++;
            }
        }

        if (oddCount == 0)
        {
            // �߰��� �� ������ tasks�� �߰����� �ʱ�
        }
        else
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, oddCount));
        }

        return tasks; // �۾� ����Ʈ�� ������ �������� ����ǰ� �����
    }
}