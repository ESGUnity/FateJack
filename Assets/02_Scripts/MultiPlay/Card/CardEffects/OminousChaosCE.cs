using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OminousChaosCE : CardEffect
{
    public OminousChaosCE() : base
        (
            "OminousChaos",
            "�ұ��� ȥ��",
            "��Ʈ �� : �� ������ �տ� �������� +5���� -5�� �մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        System.Random random = new System.Random();
        int randomNumber = random.Next(-5, 6);
        tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, randomNumber));

        return tasks; // �۾� ����Ʈ�� ������ �������� ����ǰ� �����
    }
}
