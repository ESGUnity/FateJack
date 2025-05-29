using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ManppisppiCE : CardEffect
{
    public ManppisppiCE() : base
        (
            "Manppisppi",
            "만삣삐",
            "패배 시 : 내 체력을 20 회복합니다. 내 스택에 기가 차드가 있다면 추가로 체력을 40 회복합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 20)); // 나 스스로를 회복하는 작업

        foreach (Card card in player.playerCards.StackCards)
        {
            if (card.CardEffectKey == "GigaChad")
            {
                tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 40)); // 나 스스로를 회복하는 작업
                break;
            }
        }

        return tasks;
    }
}