using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class TrinityCE : CardEffect
{
    public TrinityCE() : base
        (
            "Trinity",
            "삼위일체",
            "승리 혹은 패배 시 : 내 스택에 숫자가 같은 카드가 정확히 3장이 있다면 그 숫자만큼 나를 제외한 모든 플레이어에게 데미지를 주고 그 숫자의 3배만큼 내 체력을 회복합니다.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        Dictionary<int, int> count = new(); // 동일 카드를 저장할 변수
        List<CardEffectTask> tasks = new(); // CoreGameManager에 보낼 작업들 리스트

        // 내 스택의 모든 카드를 순회
        foreach (Card card in player.playerCards.StackCards)
        {
            if (count.Keys.Contains(card.Number))
            {
                count[card.Number]++;

                // 만약 숫자가 같은 카드가 3장 이상일 경우
                if (count[card.Number] >= 3)
                {
                    List<ulong> targetClients = new(); // 카드의 효과를 시전할 타겟 클라이언트 리스트
                    foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager의 AllPlayers 중 자신을 제외한 클라이언트 아이디를 타겟 클라이언트 리스트에 할당
                    {
                        if (client != NetworkManager.Singleton.LocalClientId)
                        {
                            targetClients.Add(client);
                        }
                    }

                    tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), card.Number)); // 타겟 클라이언트들에게 데미지 주는 작업
                    tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, card.Number * 3)); // 나 스스로를 회복하는 작업
                    break;
                }
            }
            else
            {
                count[card.Number] = 1;
            }
        }

        return tasks;
    }
}