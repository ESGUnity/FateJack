using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DoppelgangerCE : CardEffect
{
    public DoppelgangerCE() : base
        (
            "Doppelganger",
            "도플갱어",
            "승리 혹은 패배 시 : 스택의 합이 나와 같은 플레이어 하나 당 체력을 30 회복합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        )
    { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        // 회복량 정하기
        int heal = 0;
        foreach (ulong clientId in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager의 AllPlayers 중 자신을 제외한 클라이언트 아이디를 타겟 클라이언트 리스트에 할당
        {
            if (CoreGameManager.Instance.AllPlayers[clientId].playerCards.StackSum.Value == player.playerCards.StackSum.Value)
            {
                heal += 30;
            }
        }

        // CoreGameManager을 호출
        if (heal > 0)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, heal)); // 나 스스로를 회복하는 작업
            
        }

        return tasks;
    }
}