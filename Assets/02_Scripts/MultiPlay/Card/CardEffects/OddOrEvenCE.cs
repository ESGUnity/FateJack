using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OddOrEvenCE : CardEffect
{
    public OddOrEvenCE() : base
        (
            "OddOrEven",
            "홀짝",
            "히트 시 : 현재 내 스택의 합이 홀수라면 +1, 짝수라면 +2를 합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        if (player.playerCards.StackSum.Value == 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 2)); // 스택 추가하는 작업
        }
        else if (player.playerCards.StackSum.Value % 2 == 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 2)); 
        }
        else if (player.playerCards.StackSum.Value % 2 != 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, 1)); 
        }

        return tasks; // 작업 리스트를 보내어 서버에서 실행되게 만들기
    }
}
