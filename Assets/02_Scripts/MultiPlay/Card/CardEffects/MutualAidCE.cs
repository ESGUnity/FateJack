using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MutualAidCE : CardEffect
{
    public MutualAidCE() : base
        (
            "MutualAid",
            "품앗이",
            "히트 시 : 모든 플레이어의 스택에 +1을 합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        List<ulong> targetClients = new(); // 카드의 효과를 시전할 타겟 클라이언트 리스트
        foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager의 AllPlayers 중 자신을 제외한 클라이언트 아이디를 타겟 클라이언트 리스트에 할당
        {
            targetClients.Add(client);
        }

        tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), 1));

        return tasks; // 작업 리스트를 보내어 서버에서 실행되게 만들기
    }
}