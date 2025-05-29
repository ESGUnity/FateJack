using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IntenseDistortionCE : CardEffect
{
    public IntenseDistortionCE() : base
        (
            "IntenseDistortion",
            "강력한 왜곡",
            "패배 시 : 버스트 상태라면 현재 내 스택의 합만큼 체력을 회복합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        if (player.playerCards.StackSum.Value > CoreGameManager.Instance.TargetNumber.Value)
        {
            tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, player.playerCards.StackSum.Value)); // 나 스스로를 회복하는 작업
        }

        return tasks;
    }
}
