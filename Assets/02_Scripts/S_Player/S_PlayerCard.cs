using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerCard : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerTrinket pTri;
    S_PlayerStat pStat;

    [Header("카드 관련")]
    List<S_Card> originPlayerDeck = new(); // 시련 진행 중엔 절대 바뀌지 않는 불변 덱

    // pre는 카드 효과 계산 시 카드가 내진 순간의 조건을 검사하기 위한 것
    List<S_Card> deckCards = new(); // 덱에 있는 카드
    List<S_Card> stackCards = new(); // 스택에 있는 카드
    List<S_Card> exclusionCards = new(); // 환상 카드 합쳐서 제외된 카드

    // 싱글턴
    static S_PlayerCard instance;
    public static S_PlayerCard Instance { get { return instance; } }

    void Awake()
    {
        // 컴포넌트 할당
        pTri = GetComponent<S_PlayerTrinket>();
        pStat = GetComponent<S_PlayerStat>();

        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 카드 뽑기 및 덱 관리 부분
    public void AddCard(S_Card card) // 덱에 카드 추가(대부분 상점 혹은 게임 시작 시)
    {
        originPlayerDeck.Add(card);
        S_DeckInfoSystem.Instance.AddDeck(card);
    }
    public void RemoveCard(S_Card card) // 덱에서 카드 제거
    {
        originPlayerDeck.Remove(card);
        S_DeckInfoSystem.Instance.RemoveDeck(card);
    }
    public List<S_Card> DrawRandomCard(int drawCount) // 카드를 낼 때.
    {
        List<S_Card> remainDeck = GetDeckCards();
        List<S_Card> selected = new();

        if (remainDeck.Count >= drawCount)
        {
            for (int i = 0; i < drawCount; i++)
            {
                S_Card pickedCard;

                if (pStat.IsFirst)
                {
                    int diff = S_PlayerStat.Instance.CurrentLimit - S_PlayerStat.Instance.CurrentWeight;
                    pickedCard = remainDeck.Where(x => x.Num <= diff).OrderBy(x => Mathf.Abs(x.Num - diff)).FirstOrDefault();

                    if (pickedCard == null)
                    {
                        pickedCard = remainDeck.Where(x => x.Num > diff).OrderBy(x => Mathf.Abs(x.Num - diff)).FirstOrDefault();
                    }
                }
                else
                {
                    pickedCard = remainDeck.OrderBy(x => Random.value).FirstOrDefault();
                }

                selected.Add(pickedCard);
                remainDeck.Remove(pickedCard);
            }

            return selected;
        }

        // 카드 수 부족 시 전체 반환
        return new List<S_Card>(remainDeck);
    }
    public List<S_Card> GetValidCardsByFirst() // 우선 대상 카드 찾기
    {
        List<S_Card> deckCards = GetDeckCards();
        List<S_Card> pickedCards = new();
        if (deckCards.Count <= 0) return pickedCards;

        if (!pStat.IsFirst)
        {
            return deckCards;
        }
        else
        {
            int diff = S_PlayerStat.Instance.CurrentLimit - S_PlayerStat.Instance.CurrentWeight;

            return deckCards.Where(x => x.Num > diff).OrderBy(x => Mathf.Abs(x.Num - diff)).ToList();
        }
    }
    #endregion
    #region 시련 관련 메서드
    public void InitDeckByStartGame() // 게임 첫 시작 시 카드 추가
    {
        foreach (S_Card card in S_CardManager.Instance.GenerateCardByStartGame())
        {
            card.IsInDeck = true;
            card.IsGenerated = false;
            card.IsCurrentTurn = false;
            card.IsCursed = false;
            AddCard(card);
        }
    }
    public async Task UpdateCardsByStartTrial() // 시작 시 preDeck이랑 immediateDeck 채우고 속전속결 각인 카드도 발동(카드 내기)
    {
        deckCards = GetOriginPlayerDeckCards();

        List<S_Card> cards = GetDeckCards();
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].Engraving == S_EngravingEnum.QuickAction)
            {
                await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(cards[i], S_CardOrderTypeEnum.Hit);
            }
        }
    }
    public void UpdateCardByStartNewTurn() // 새로운 턴 시작 시 업데이트 해야하는 카드. 무작위 능력치인 카드와 한 턴에~ 카드 초기화
    {
        foreach (S_Card card in GetStackCards())
        {
            // 매 턴마다 능력치가 변경되는 효과의 능력치를 바꿔주기
            if (card.CardEffect == S_CardEffectEnum.Common_Balance)
            {
                List<S_BattleStatEnum> stat = new() { S_BattleStatEnum.Str, S_BattleStatEnum.Mind, S_BattleStatEnum.Luck };
                card.Stat = stat[Random.Range(0, stat.Count)];
            }

            // 매 턴마다 능력치가 변경되는 효과의 능력치를 바꿔주기
            if (card.CardEffect == S_CardEffectEnum.Common_Berserk)
            {
                List<S_BattleStatEnum> stat = new() { S_BattleStatEnum.Str_Mind, S_BattleStatEnum.Str_Luck, S_BattleStatEnum.Mind_Luck };
                card.Stat = stat[Random.Range(0, stat.Count)];
            }

            // 대혼돈 각인 전용(유일한 한 턴에 ~ 효과)
            if (card.Engraving == S_EngravingEnum.GrandChaos || card.Engraving == S_EngravingEnum.GrandChaos_Flip ||
                card.Engraving == S_EngravingEnum.Crush || card.Engraving == S_EngravingEnum.Crush_Flip)
            {
                card.ActivatedCount = 0;
                card.IsMeetCondition = false;
            }
        }

        // 조건 검사 한 번 해주기
        S_EffectActivator.Instance.CheckCardMeetCondition();

        // ActivatedCount와 IsMeetCondition이 변경되었음으로 상태 업데이트
        S_StackInfoSystem.Instance.UpdateStackCardState();
    }
    public void ResetCardsByTwist(out List<S_Card> stacks, out List<S_Card> exclusions) // 비틀기에 의한 카드 제외 및 제외된 카드 돌아오기
    {
        // 낸 카드 덱으로 돌려보내기
        stacks = GetStackCards().Where(x => x.IsCurrentTurn).ToList();
        foreach (S_Card card in stacks)
        {
            stackCards.Remove(card);
            card.IsCurrentTurn = false;

            if (!card.IsGenerated)
            {
                deckCards.Add(card);
                card.IsInDeck = true;
            }
        }
        foreach (S_Card card in stackCards)
        {
            card.IsCurrentTurn = false;
        }

        // 이번 턴에 제외된 카드는 덱으로 되돌아오기
        exclusions = exclusionCards.ToList().Where(x => x.IsCurrentTurn).ToList();
        foreach (S_Card card in exclusions)
        {
            if (card.IsGenerated)
            {
                exclusionCards.Remove(card);
            }
            else
            {
                exclusionCards.Remove(card);
                deckCards.Add(card);
                card.IsInDeck = true;
                card.IsCurrentTurn = false;
            }
        }
        foreach (S_Card card in exclusionCards)
        {
            card.IsCurrentTurn = false;
        }

        // 덱, 스택 인포 업데이트
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
        S_StackInfoSystem.Instance.UpdateStackCardState();
    }
    public void FixCardsByStand()
    {
        foreach (S_Card card in stackCards)
        {
            card.IsCurrentTurn = false;
        }
        foreach (S_Card card in exclusionCards)
        {
            card.IsCurrentTurn = false;
        }

        // 덱, 스택 인포 업데이트
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
        S_StackInfoSystem.Instance.UpdateStackCardState();
    }
    public void ResetCardsByEndTrial()
    {
        // 덱 카드 정상화
        foreach (S_Card card in originPlayerDeck)
        {
            card.ActivatedCount = 0;
            card.Stat = S_BattleStatEnum.None;
            card.ExpectedValue = 0;

            card.IsInDeck = true;
            card.IsCurrentTurn = false;
            card.IsGenerated = false;
            card.IsCursed = false;
            card.IsMeetCondition = false;
        }

        stackCards.Clear();
        exclusionCards.Clear();
        deckCards = GetOriginPlayerDeckCards();

        // 덱 인포 업데이트
        S_DeckInfoSystem.Instance.UpdateDeckCardsState();
    }
    #endregion
    #region 히트 및 제외
    public void HitCard(S_Card hitCard) // 덱에서 히트. 스택과 덱 업데이트
    {
        deckCards.Remove(hitCard);
        stackCards.Add(hitCard);

        hitCard.IsInDeck = false;
        hitCard.IsCurrentTurn = true;
        hitCard.IsGenerated = false;
    }
    public void GenCard(S_Card card) // 새롭게 생성하여 히트. 스택과 덱 업데이트
    {
        stackCards.Add(card);

        card.IsInDeck = false;
        card.IsCurrentTurn = true;
        card.IsGenerated = true;
    }
    public void ExclusionCard(S_Card card) // 제외에 의한 덱 카드 제외
    {
        if (stackCards.Contains(card))
        {
            stackCards.Remove(card);
        }
        if (deckCards.Contains(card))
        {
            deckCards.Remove(card);
        }
        exclusionCards.Add(card);

        card.IsInDeck = false;
        card.IsCurrentTurn = true;
    }
    #endregion
    #region 보조 메서드
    public List<S_Card> GetOriginPlayerDeckCards()
    {
        return originPlayerDeck.ToList();
    }
    public List<S_Card> GetDeckCards()
    {
        return deckCards.ToList();
    }
    public List<S_Card> GetStackCards()
    {
        return stackCards.ToList();
    }
    public List<S_Card> GetExclusionCards()
    {
        return exclusionCards.ToList();
    }

    public string GetCardEffectDescription(S_Card card) // 설명에 추가로 붙은 것들 메서드
    {
        StringBuilder sb = new();

        sb.Append(S_CardEffectMetadata.GetDescription(card.CardEffect));

        // 무작위 능력치 
        if (card.CardEffect == S_CardEffectEnum.Common_Balance)
        {
            switch (card.Stat)
            {
                case S_BattleStatEnum.Str:
                    sb.Replace("능력치가", "힘이");
                    break;
                case S_BattleStatEnum.Mind:
                    sb.Replace("능력치가", "정신력이");
                    break;
                case S_BattleStatEnum.Luck:
                    sb.Replace("능력치가", "행운이");
                    break;
            }
        }
        if (card.CardEffect == S_CardEffectEnum.Common_Berserk)
        {
            switch (card.Stat)
            {
                case S_BattleStatEnum.Str_Mind:
                    sb.Replace("능력치 2개를", "힘과 정신력을");
                    break;
                case S_BattleStatEnum.Str_Luck:
                    sb.Replace("능력치 2개를", "힘과 행운을");
                    break;
                case S_BattleStatEnum.Mind_Luck:
                    sb.Replace("능력치 2개를", "정신력과 행운을");
                    break;
            }
        }

        if (!card.IsInDeck) // 덱에 없는 카드라면 추가 표기하기
        {
            HashSet<S_CardEffectEnum> expectedStatIncreases = new()
            {
                S_CardEffectEnum.Str_ZenithBreak, S_CardEffectEnum.Str_CalamityApproaches, S_CardEffectEnum.Str_UntappedPower, S_CardEffectEnum.Str_UnjustSacrifice,
                S_CardEffectEnum.Mind_DeepInsight, S_CardEffectEnum.Mind_PerfectForm, S_CardEffectEnum.Mind_Unshackle, S_CardEffectEnum.Mind_Drain, S_CardEffectEnum.Mind_WingsOfFreedom, S_CardEffectEnum.Mind_Accept,
                S_CardEffectEnum.Luck_Disorder, S_CardEffectEnum.Luck_Composure, S_CardEffectEnum.Luck_Grill,
                S_CardEffectEnum.Common_Balance
            };
            HashSet<S_CardEffectEnum> expectedHarmValue = new()
            {
                S_CardEffectEnum.Str_WrathStrike, S_CardEffectEnum.Str_EngulfInFlames, S_CardEffectEnum.Str_FinishingStrike, S_CardEffectEnum.Str_FlowingSin, S_CardEffectEnum.Str_BindingForce, S_CardEffectEnum.Str_Grudge,
                S_CardEffectEnum.Mind_PreciseStrike, S_CardEffectEnum.Mind_SharpCut, S_CardEffectEnum.Mind_Split, S_CardEffectEnum.Mind_Dissolute, S_CardEffectEnum.Mind_Awakening,
                S_CardEffectEnum.Luck_SuddenStrike, S_CardEffectEnum.Luck_CriticalBlow, S_CardEffectEnum.Luck_ForcedTake, S_CardEffectEnum.Luck_Shake, S_CardEffectEnum.Luck_FatalBlow,
                S_CardEffectEnum.Common_Berserk, S_CardEffectEnum.Common_Carnage, S_CardEffectEnum.Common_LastStruggle
            };

            if (expectedStatIncreases.Contains(card.CardEffect))
            {
                sb.Append($"\n(예상 증가량 : {card.ExpectedValue})");
            }
            else if (expectedHarmValue.Contains(card.CardEffect))
            {
                sb.Append($"\n(예상 피해량 : {card.ExpectedValue})");
            }
        }

        return sb.ToString();
    }
    public string GetEngravingDescription(S_Card card) // 설명에 추가로 붙은 것들 메서드
    {
        StringBuilder sb = new();

        sb.Append(S_CardEffectMetadata.GetDescription(card.Engraving));

        switch (card.Engraving)
        {
            case S_EngravingEnum.Legion:
                sb.Append($"\n(스택의 카드 무게 합 : {card.ActivatedCount}"); break;
            case S_EngravingEnum.Legion_Flip:
                sb.Append($"\n(스택의 카드 무게 합 : {card.ActivatedCount}"); break;
            case S_EngravingEnum.AllOut:
                sb.Append($"\n(스택의 카드 무게 합 : {card.ActivatedCount}"); break;
            case S_EngravingEnum.AllOut_Flip:
                sb.Append($"\n(스택의 카드 무게 합 : {card.ActivatedCount}"); break;
            case S_EngravingEnum.Delicacy:
                sb.Append($"\n(스택의 카드 : {card.ActivatedCount}장)"); break;
            case S_EngravingEnum.Delicacy_Flip:
                sb.Append($"\n(스택의 카드 : {card.ActivatedCount}장)"); break;
            case S_EngravingEnum.Precision:
                sb.Append($"\n(스택의 카드 : {card.ActivatedCount}장)"); break;
            case S_EngravingEnum.Precision_Flip:
                sb.Append($"\n(스택의 카드 : {card.ActivatedCount}장)"); break;
            case S_EngravingEnum.Resection:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.Resection_Flip:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.Patience:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.Patience_Flip:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.Overflow:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.Overflow_Flip:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.Fierce:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.Fierce_Flip:
                sb.Append($"\n(이번 턴에 낸 카드 : {card.ActivatedCount}장 째)"); break;
            case S_EngravingEnum.GrandChaos:
                sb.Append($"\n(만족한 카드 타입 : {card.ActivatedCount}개)"); break;
            case S_EngravingEnum.GrandChaos_Flip:
                sb.Append($"\n(만족한 카드 타입 : {card.ActivatedCount}개)"); break;
            case S_EngravingEnum.Crush:
                sb.Append($"\n(만족한 카드 타입 : {card.ActivatedCount}개)"); break;
            case S_EngravingEnum.Crush_Flip:
                sb.Append($"\n(만족한 카드 타입 : {card.ActivatedCount}개)"); break;
            case S_EngravingEnum.Finale:
                sb.Append($"\n(덱의 남은 카드 : {card.ActivatedCount}장"); break;
            case S_EngravingEnum.Climax:
                sb.Append($"\n(덱의 남은 카드 : {card.ActivatedCount}장"); break;
        }
        return sb.ToString();
    }

    public void SwapCardObjIndex(S_Card card, bool isMoveRight) // 오브젝트 인덱스 교환하는 메서드
    {
        if (isMoveRight)
        {
            int index = stackCards.IndexOf(card);

            if (index == stackCards.Count - 1) // 마지막 인덱스라면
            {
                S_Card lastCard = stackCards[index];

                // 리스트의 요소들을 뒤에서부터 한 칸씩 뒤로 밀기
                for (int i = index; i > 0; i--)
                {
                    stackCards[i] = stackCards[i - 1];
                }

                // 맨 앞에 마지막 카드를 넣기
                stackCards[0] = lastCard;
            }
            else // 아니라면
            {
                // 오른쪽 요소와 자리 바꾸기
                S_Card tempGo = stackCards[index + 1];
                stackCards[index + 1] = stackCards[index];
                stackCards[index] = tempGo;
            }
        }
        else
        {
            int index = stackCards.IndexOf(card);

            if (index == 0)
            {
                S_Card firstCard = stackCards[0];

                // 요소들을 왼쪽으로 한 칸씩 이동
                for (int i = 0; i < stackCards.Count - 1; i++)
                {
                    stackCards[i] = stackCards[i + 1];
                }

                // 마지막 자리에 처음 요소 넣기
                stackCards[stackCards.Count - 1] = firstCard;
            }
            else
            {
                // 일반적인 왼쪽 교체
                S_Card tempGo = stackCards[index - 1];
                stackCards[index - 1] = stackCards[index];
                stackCards[index] = tempGo;
            }
        }
    }
    #endregion
}