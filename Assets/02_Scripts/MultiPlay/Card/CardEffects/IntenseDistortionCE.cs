using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IntenseDistortionCE : CardEffect
{
    public IntenseDistortionCE() : base
        (
            "IntenseDistortion",
            "������ �ְ�",
            "�й� �� : ����Ʈ ���¶�� ���� �� ������ �ո�ŭ ü���� ȸ���մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        if (player.playerCards.StackSum.Value > CoreGameManager.Instance.TargetNumber.Value)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, player.playerCards.StackSum.Value)); // �� �����θ� ȸ���ϴ� �۾�
        }

        return tasks;
    }
}
