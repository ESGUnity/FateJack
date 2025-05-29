using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class TrinityCE : CardEffect
{
    public TrinityCE() : base
        (
            "Trinity",
            "������ü",
            "�¸� Ȥ�� �й� �� : �� ���ÿ� ���ڰ� ���� ī�尡 ��Ȯ�� 3���� �ִٸ� �� ���ڸ�ŭ ���� ������ ��� �÷��̾�� �������� �ְ� �� ������ 3�踸ŭ �� ü���� ȸ���մϴ�.",
            CardEffectRankEnum.Void,
            CardEffectTypeEnum.RoundEnd
        ) { }

    public override List<CardEffectTask> ActiveCardEffect(PlayerController player)
    {
        Dictionary<int, int> count = new(); // ���� ī�带 ������ ����
        List<CardEffectTask> tasks = new(); // CoreGameManager�� ���� �۾��� ����Ʈ

        // �� ������ ��� ī�带 ��ȸ
        foreach (Card card in player.playerCards.StackCards)
        {
            if (count.Keys.Contains(card.Number))
            {
                count[card.Number]++;

                // ���� ���ڰ� ���� ī�尡 3�� �̻��� ���
                if (count[card.Number] >= 3)
                {
                    List<ulong> targetClients = new(); // ī���� ȿ���� ������ Ÿ�� Ŭ���̾�Ʈ ����Ʈ
                    foreach (var client in CoreGameManager.Instance.AllPlayers.Keys) // CoreGameManager�� AllPlayers �� �ڽ��� ������ Ŭ���̾�Ʈ ���̵� Ÿ�� Ŭ���̾�Ʈ ����Ʈ�� �Ҵ�
                    {
                        if (client != NetworkManager.Singleton.LocalClientId)
                        {
                            targetClients.Add(client);
                        }
                    }

                    tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, targetClients.ToArray(), card.Number)); // Ÿ�� Ŭ���̾�Ʈ�鿡�� ������ �ִ� �۾�
                    tasks.Add(new CardEffectTask(EffectTypeEnum.Damage, NetworkManager.Singleton.LocalClientId, new ulong[] { NetworkManager.Singleton.LocalClientId }, card.Number * 3)); // �� �����θ� ȸ���ϴ� �۾�
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