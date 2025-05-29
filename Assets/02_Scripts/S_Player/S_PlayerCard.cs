using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerCard : MonoBehaviour
{
    [Header("������Ʈ")]
    S_PlayerSkill pSkill;
    S_PlayerStat pStat;

    [Header("ī�� ����")]
    List<S_Card> originPlayerDeck = new(); // �÷� ���� �߿� ���� �ٲ��� �ʴ� �Һ� ��

    // pre�� ī�� ȿ�� ��� �� ī�尡 ���� ������ ������ �˻��ϱ� ���� ��.
    // immediate�� ī�����ť�� �� �� ��� ���Ǵ� ����Ʈ. �� ī�忡 �ִ� ���� �ε��ϰų� ������ �� �ʿ��� ��.
    List<S_Card> preDeckCards = new(); // ���� �ִ� ī��
    List<S_Card> immediateDeckCards = new(); // ���� �ִ� ī��

    List<S_Card> preStackCards = new(); // ���ÿ� �ִ� ī��
    List<S_Card> immediateStackCards = new(); // ���ÿ� �ִ� ī��

    List<S_Card> preExclusionDeckCards = new(); // ���ܵ� ī��
    List<S_Card> immediateExclusionDeckCards = new(); // ���ܵ� ī��

    List<S_Card> preExclusionTotalCards = new(); // ȯ�� ī�� ���ļ� ���ܵ� ī��
    List<S_Card> immediateExclusionTotalCards = new(); // ȯ�� ī�� ���ļ� ���ܵ� ī��

    // �̱���
    static S_PlayerCard instance;
    public static S_PlayerCard Instance { get { return instance; } }

    void Awake()
    {
        // ������Ʈ �Ҵ�
        pSkill = GetComponent<S_PlayerSkill>();
        pStat = GetComponent<S_PlayerStat>();

        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

#region ī�� �̱� �� �� ���� �κ�
    public void InitDeckByStartGame() // ���� �� ī�� �߰�
    {
        foreach (S_Card card in S_CardManager.Instance.GenerateCardByStartGame())
        {
            card.IsInDeck = true;
            card.IsIllusion = false;
            card.IsCurrentTurnHit = false;
            card.IsCursed = false;
            AddCard(card);
        }
    }
    public void AddCard(S_Card card) // ���� ī�� �߰�
    {
        originPlayerDeck.Add(card);
        S_DeckInfoSystem.Instance.AddDeck(card);
    }
    public void RemoveCard(S_Card card) // ������ ī�� ����
    {
        originPlayerDeck.Remove(card);
        S_DeckInfoSystem.Instance.RemoveDeck(card);
    }
    public List<S_Card> DrawRandomCard(int drawCount)
    {
        List<S_Card> remainDeck = GetPreDeckCards();
        List<S_Card> pickedCards = new();
        List<S_Card> selected = new();

        if (remainDeck.Count > drawCount)
        {
            while (selected.Count < drawCount)
            {
                // �켱 üũ
                switch (S_PlayerStat.Instance.IsFirst)
                {
                    case S_FirstEffectEnum.None: break;
                    case S_FirstEffectEnum.Spade: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Spade).ToList(); break;
                    case S_FirstEffectEnum.Heart: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Heart).ToList(); break;
                    case S_FirstEffectEnum.Diamond: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Diamond).ToList(); break;
                    case S_FirstEffectEnum.Clover: pickedCards = remainDeck.Where(x => x.Suit == S_CardSuitEnum.Clover).ToList(); break;
                    case S_FirstEffectEnum.LeastSuit:
                        var grouped = remainDeck.GroupBy(c => c.Suit)
                            .OrderBy(x => x.Count())
                            .ToList();

                        // ���� ��� ����
                        int minCount = grouped.First().Count();
                        var leastSuitGroups = grouped.Where(g => g.Count() == minCount).ToList();
                        var chosenGroup = leastSuitGroups[UnityEngine.Random.Range(0, leastSuitGroups.Count)];

                        // ���� ���� ���� ī��� ��ȯ
                        pickedCards = chosenGroup.ToList();
                        break;
                    case S_FirstEffectEnum.One: pickedCards = remainDeck.Where(x => x.Number == 1).ToList(); break;
                    case S_FirstEffectEnum.Two: pickedCards = remainDeck.Where(x => x.Number == 2).ToList(); break;
                    case S_FirstEffectEnum.Three: pickedCards = remainDeck.Where(x => x.Number == 3).ToList(); break;
                    case S_FirstEffectEnum.Four: pickedCards = remainDeck.Where(x => x.Number == 4).ToList(); break;
                    case S_FirstEffectEnum.Five: pickedCards = remainDeck.Where(x => x.Number == 5).ToList(); break;
                    case S_FirstEffectEnum.Six: pickedCards = remainDeck.Where(x => x.Number == 6).ToList(); break;
                    case S_FirstEffectEnum.Seven: pickedCards = remainDeck.Where(x => x.Number == 7).ToList(); break;
                    case S_FirstEffectEnum.Eight: pickedCards = remainDeck.Where(x => x.Number == 8).ToList(); break;
                    case S_FirstEffectEnum.Nine: pickedCards = remainDeck.Where(x => x.Number == 9).ToList(); break;
                    case S_FirstEffectEnum.Ten: pickedCards = remainDeck.Where(x => x.Number == 10).ToList(); break;
                    case S_FirstEffectEnum.CleanHitNumber:
                        int diff = S_PlayerStat.Instance.CurrentLimit - S_PlayerStat.Instance.StackSum;
                        if (diff <= 0)
                        {
                            pickedCards = remainDeck.Where(x => x.Number == 1).ToList();
                        }
                        else if (diff > 0 && diff <= 10)
                        {
                            pickedCards = remainDeck.Where(x => x.Number == diff).ToList();
                        }
                        else if (diff > 10)
                        {
                            pickedCards = remainDeck.Where(x => x.Number == 10).ToList();
                        }
                        break;
                }

                // cards�� ����ٸ�, �� �켱���� ���� ī�尡 ���ٸ�
                if (pickedCards.Count <= 0)
                {
                    pickedCards = remainDeck.ToList();
                }

                int randomIndex = Random.Range(0, pickedCards.Count);
                S_Card pick = pickedCards[randomIndex];

                selected.Add(pick);
                remainDeck.Remove(pick);
            }

            return selected;
        }
        else
        {
            return remainDeck;
        }
    }
    public void InitCardsByStartTrial() // ���� �� preDeck�̶� immediateDeck ä���
    {
        preDeckCards = GetOriginPlayerDeckCards();
        immediateDeckCards = GetOriginPlayerDeckCards();
    }
#endregion
#region ��Ʈ �� ���� ��� �κ�
    public void HitCardByDeckPre(S_Card hitCard) // ������ ��Ʈ. ���ð� �� ������Ʈ
    {
        preDeckCards.Remove(hitCard);
        preStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = false;
    }
    public void HitCardByDeckImmediate(S_Card hitCard) // ������ ��Ʈ. ���ð� �� ������Ʈ
    {
        immediateDeckCards.Remove(hitCard);
        immediateStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = false;
    }
    public void HitCardByIllusionPre(S_Card hitCard) // ���Ӱ� �����Ͽ� ��Ʈ. ���ð� �� ������Ʈ
    {
        preStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = true;
    }
    public void HitCardByIllusionImmediate(S_Card hitCard) // ���Ӱ� �����Ͽ� ��Ʈ. ���ð� �� ������Ʈ
    {
        immediateStackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurnHit = true;
        hitCard.IsIllusion = true;
    }
    public void ExclusionCardByExclusionPre(S_Card exclusionCard) // ���ܿ� ���� �� ī�� ����
    {
        preDeckCards.Remove(exclusionCard);
        preExclusionTotalCards.Add(exclusionCard);
        preExclusionDeckCards.Add(exclusionCard);

        exclusionCard.IsInDeck = false;
        exclusionCard.IsCurrentTurnHit = true;
        exclusionCard.IsIllusion = false;
    }
    public void ExclusionCardByExclusionImmediate(S_Card exclusionCard) // ���ܿ� ���� �� ī�� ����
    {
        immediateDeckCards.Remove(exclusionCard);
        immediateExclusionTotalCards.Add(exclusionCard);
        immediateExclusionDeckCards.Add(exclusionCard);

        exclusionCard.IsInDeck = false;
        exclusionCard.IsCurrentTurnHit = true;
        exclusionCard.IsIllusion = false;
    }
    public void ResetCardsByTwist(out List<S_Card> stacks, out List<S_Card> exclusions) // ��Ʋ�⿡ ���� ī�� ���� �� ���ܵ� ī�� ���ƿ���
    {
        // �� ī�� �����ϱ�
        stacks = GetPreStackCards().Where(x => x.IsCurrentTurnHit).ToList();
        foreach (S_Card card in stacks)
        {
            preStackCards.Remove(card);
            immediateStackCards.Remove(card);

            preExclusionTotalCards.Add(card);
            immediateExclusionTotalCards.Add(card);

            if (!card.IsIllusion)
            {
                preExclusionDeckCards.Add(card);
                immediateExclusionDeckCards.Add(card);
            }

            card.IsCurrentTurnHit = false;
        }
        foreach (S_Card card in preStackCards)
        {
            card.IsCurrentTurnHit = false;
        }

        // ���ܵ� ī��� ������ �ǵ��ƿ���
        exclusions = preExclusionDeckCards.ToList().Where(x => x.IsCurrentTurnHit).ToList();
        foreach (S_Card card in exclusions)
        {
            preDeckCards.Add(card);
            immediateDeckCards.Add(card);

            preExclusionTotalCards.Remove(card);
            immediateExclusionTotalCards.Remove(card);

            if (!card.IsIllusion)
            {
                preExclusionDeckCards.Remove(card);
                immediateExclusionDeckCards.Remove(card);
            }

            card.IsInDeck = true;
            card.IsCurrentTurnHit = false;
        }
        foreach (S_Card card in preExclusionTotalCards)
        {
            card.IsCurrentTurnHit = false;
        }

        // ��, ���� ���� ������Ʈ
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
        S_StackInfoSystem.Instance.UpdateStackCardsState();
    }
    public void FixCardsByStand()
    {
        foreach (S_Card card in preStackCards)
        {
            card.IsCurrentTurnHit = false;
        }
        foreach (S_Card card in preExclusionTotalCards)
        {
            card.IsCurrentTurnHit = false;
        }

        // ��, ���� ���� ������Ʈ
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
        S_StackInfoSystem.Instance.UpdateStackCardsState();
    }
    public void ResetCardsByEndTrial()
    {
        // �� ī�� ����ȭ
        foreach (S_Card card in originPlayerDeck)
        {
            card.IsInDeck = true;
            card.IsCurrentTurnHit = false;
            card.IsIllusion = false;
            card.IsCursed = false;
            card.CanActivateEffect = false;
        }

        preStackCards.Clear();
        immediateStackCards.Clear();
        preExclusionDeckCards.Clear();
        immediateExclusionDeckCards.Clear();
        preExclusionTotalCards.Clear();
        immediateExclusionTotalCards.Clear();

        // �� ���� ������Ʈ
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
    }
    #endregion
    #region ���� �޼���
    public List<S_Card> GetOriginPlayerDeckCards()
    {
        return originPlayerDeck.ToList();
    }
    public List<S_Card> GetPreDeckCards()
    {
        return preDeckCards.ToList();
    }
    public List<S_Card> GetImmediateDeckCards()
    {
        return immediateDeckCards.ToList();
    }
    public List<S_Card> GetPreStackCards()
    {
        return preStackCards.ToList();
    }
    public List<S_Card> GetImmediateStackCards()
    {
        return immediateStackCards.ToList();
    }
    public List<S_Card> GetPreExclusionTotalCards()
    {
        return preExclusionTotalCards.ToList();
    }
    public List<S_Card> GetImmediateExclusionTotalCards()
    {
        return immediateExclusionTotalCards.ToList();
    }
    public void CheckCardMeetCondition(S_Card hitCard)
    {
        // ī�带 ���� ���� ī���� ���� üũ
        foreach (S_Card card in GetPreStackCards())
        {
            if (card.BasicCondition != S_CardBasicConditionEnum.Unleash)
            {
                card.CanActivateEffect = S_EffectActivator.Instance.IsMeetAdditiveCondition(card, hitCard);
            }
        }
    }
    #endregion
}
