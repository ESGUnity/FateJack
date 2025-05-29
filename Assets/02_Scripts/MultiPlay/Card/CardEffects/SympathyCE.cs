using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SympathyCE : CardEffect
{
    public SympathyCE() : base
        (
            "Sympathy",
            "동정",
            "히트 시 : 현재 내 스택의 합이 목표 점수보다 작다면 내 스택의 합에 +2를 합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        if (player.playerCards.StackSum.Value < CoreGameManager.Instance.TargetNumber.Value)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 2));
        }

        return tasks;
    }
}
