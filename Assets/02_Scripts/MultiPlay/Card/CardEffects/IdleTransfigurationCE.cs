using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IdleTransfigurationCE : CardEffect
{
    public IdleTransfigurationCE() : base
        (
            "IdleTransfiguration",
            "무위전변",
            "패배 시 : 스택의 합이 나와 같은 플레이어가 하나 이상 있다면 4골드를 획득합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        foreach (var client in CoreGameManager.Instance.AllPlayers.Keys)
        {
            if (CoreGameManager.Instance.AllPlayers[client].playerCards.StackSum.Value == player.playerCards.StackSum.Value)
            {
                tasks.Add(new CardEffectTask(EffectTypeEnum.GetGold, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 4)); // 나 스스로를 회복하는 작업
                break;
            }
        }

        return tasks;
    }
}