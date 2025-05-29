using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class JackBlackCE : CardEffect
{
    public JackBlackCE() : base
        (
            "JackBlack",
            "잭 블랙",
            "패배 시 : 현재 내 스택의 합이 12라면 나를 제외한 모든 플레이어에게 12의 데미지를 주고 체력을 12 회복합니다.",
            CardEffectRankEnum.Void, 
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        if (player.playerCards.StackSum.Value == 12)
        {
            List<ulong> targetClients = new(); // 카드의 효과를 시전할 타겟 클라이언트 리스트
            foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager의 AllPlayers 중 자신을 제외한 클라이언트 아이디를 타겟 클라이언트 리스트에 할당
            {
                if (client != NetworkManager.Singleton.LocalClientId)
                {
                    targetClients.Add(client);
                }
            }

            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), 12)); // 타겟 클라이언트들에게 데미지 주는 작업
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 12)); // 나 스스로를 회복하는 작업
        }

        return tasks;
    }
}