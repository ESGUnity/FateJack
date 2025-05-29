using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CaffeineAddictionCE : CardEffect
{
    public CaffeineAddictionCE() : base
        (
            "CaffeineAddiction",
            "카페인 중독",
            "패배 시 : 내 스택의 합이 40 이상이라면 체력을 40 회복합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new();

        if (player.playerCards.StackSum.Value >= 40)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 40)); // 나 스스로를 회복하는 작업

        }

        return tasks;
    }
}
