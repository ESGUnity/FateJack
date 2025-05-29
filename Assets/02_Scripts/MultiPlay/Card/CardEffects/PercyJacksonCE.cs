using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PercyJacksonCE : CardEffect
{
    public PercyJacksonCE() : base
        (
            "PercyJackson",
            "퍼시 잭슨",
            "승리 시 : 나를 제외한 모든 플레이어의 데미지 배율을 1 훔칩니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        List<ulong> targetClients = new(); // 카드의 효과를 시전할 타겟 클라이언트 리스트
        foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager의 AllPlayers 중 자신을 제외한 클라이언트 아이디를 타겟 클라이언트 리스트에 할당
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