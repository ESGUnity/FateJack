using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OverthinkingAddictionCE : CardEffect
{
    public OverthinkingAddictionCE() : base
        (
            "OverthinkingAddiction",
            "고민 중독",
            "히트 시 : 현재 내 스택에 숫자가 홀수인 카드 하나 당 +1을 합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        int oddCount = 0;
        foreach (Card card in player.playerCards.StackCards)
        {
            if (card.Number % 2 != 0)
            {
                oddCount++;
            }
        }

        if (oddCount == 0)
        {
            // 추가된 게 없으면 tasks에 추가하지 않기
        }
        else
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, oddCount));
        }

        return tasks; // 작업 리스트를 보내어 서버에서 실행되게 만들기
    }
}