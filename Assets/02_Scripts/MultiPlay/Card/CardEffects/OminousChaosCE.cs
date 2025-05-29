using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OminousChaosCE : CardEffect
{
    public OminousChaosCE() : base
        (
            "OminousChaos",
            "불길한 혼돈",
            "히트 시 : 내 스택의 합에 무작위로 +5부터 -5를 합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.Hit
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        System.Random random = new System.Random();
        int randomNumber = random.Next(-5, 6);
        tasks.Add(new CardEffectTask(EffectTypeEnum.StackSum, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, randomNumber));

        return tasks; // 작업 리스트를 보내어 서버에서 실행되게 만들기
    }
}
