using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class S_PlayerCard : MonoBehaviour
{
    [Header("컴포넌트")]
    S_PlayerStat pStat;

    [Header("카드 관련")]
    List<S_CardBase> originPlayerDeck = new(); // 시련 진행 중엔 절대 바뀌지 않는 불변 덱

    // pre는 카드 효과 계산 시 카드가 내진 순간의 조건을 검사하기 위한 것
    List<S_CardBase> deckCards = new(); // 덱
    List<S_CardBase> usedCards = new(); // 사용한 카드 더미
    List<S_CardBase> fieldCards = new(); // 필드

    // 싱글턴
    static S_PlayerCard instance;
    public static S_PlayerCard Instance { get { return instance; } }

    void Awake()
    {
        // 컴포넌트 할당
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
    public void AddCard(S_CardBase card) // 덱에 카드 추가(대부분 상점 혹은 게임 시작 시)
    {
        card.IsInDeck = true;
        card.IsInUsed = false;
        card.IsCurrentTurn = false;
        card.IsCursed = false;

        originPlayerDeck.Add(card);
        deckCards.Add(card);
        S_DeckInfoSystem.Instance.AddDeck(card);
    }
    public void RemoveCard(S_CardBase card) // 덱에서 카드 제거
    {
        originPlayerDeck.Remove(card);
        S_DeckInfoSystem.Instance.RemoveDeckCard(card);
    }
    public List<S_CardBase> DrawRandomCard(int drawCount) // 카드를 낼 때 카드 보기를 뽑는 메서드
    {
        List<S_CardBase> remainDeck = GetDeckCards();
        List<S_CardBase> selected = new();

        // 덱에 카드가 없을 땐 사용한 카드 더미에서 가져와서 보충하기
        if (remainDeck.Count == 0)
        {
            // 실제 카드를 덱으로 옮김
            deckCards = usedCards.ToList();
            usedCards.Clear();
            foreach (S_CardBase card in deckCards)
            {
                card.IsInDeck = true;
                card.IsInUsed = false;
            }

            // 연출 로직. 사용한 카드 더미가 사라지고 사용한 카드가 덱으로 바뀜
            S_DeckInfoSystem.Instance.VacateViewUsedCards();
            S_DeckInfoSystem.Instance.AddDecks(deckCards);

            remainDeck = GetDeckCards();
        }

        if (remainDeck.Count > drawCount)
        {
            for (int i = 0; i < drawCount; i++)
            {
                S_CardBase pickedCard;

                if (pStat.IsFirst > 0)
                {
                    int diff = S_PlayerStat.Instance.CurrentLimit - S_PlayerStat.Instance.CurrentWeight;
                    pickedCard = remainDeck.Where(x => x.Weight <= diff).OrderBy(x => Mathf.Abs(x.Weight - diff)).FirstOrDefault();

                    if (pickedCard == null)
                    {
                        pickedCard = remainDeck.Where(x => x.Weight > diff).OrderBy(x => Mathf.Abs(x.Weight - diff)).FirstOrDefault();
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
        return new List<S_CardBase>(remainDeck);
    }
    public List<S_CardBase> GetValidCardsByFirst() // 우선 대상 카드 찾기
    {
        List<S_CardBase> deckCards = GetDeckCards();
        List<S_CardBase> pickedCards = new();
        if (deckCards.Count <= 0) return pickedCards;

        if (pStat.IsFirst > 0)
        {
            int diff = S_PlayerStat.Instance.CurrentLimit - S_PlayerStat.Instance.CurrentWeight;

            if (deckCards.Where(x => x.Weight == diff).Count() > 0)
            {
                return deckCards.Where(x => x.Weight == diff).ToList();
            }
            else if (deckCards.Where(x => x.Weight < diff).Count() > 0)
            {
                return deckCards.Where(x => x.Weight < diff).ToList();
            }
            else
            {
                return deckCards;
            }
        }
        else
        {
            return deckCards;
        }
    }
    #endregion
    #region 시련 관련 메서드
    public void InitDeckByStartGame() // 게임 첫 시작 시 카드 추가
    {
        foreach (S_CardBase card in S_CardList.GetInitCardsByStartGame())
        {
            AddCard(card);
        }
    }
    public async Task UpdateCardsByStartTrial() // 덱 카드 채우기, 속전속결 카드 내기
    {
        deckCards = GetOriginPlayerDeckCards();
        List<S_CardBase> cards = GetDeckCards();
        S_DeckInfoSystem.Instance.AddDecks(cards);

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].OriginEngraving.Contains(S_EngravingEnum.QuickAction) || cards[i].Engraving.Contains(S_EngravingEnum.QuickAction))
            {
                await S_GameFlowManager.Instance.StartHitCardAsync(cards[i]);
            }
        }
    }
    public void UpdateCardsByStartTrialByTutorial() // 덱 카드 채우기
    {
        deckCards = GetOriginPlayerDeckCards();
    }
    public async Task UpdateCardsByStand() // 사용한 카드 더미로 카드 보내기
    {
        List<S_CardBase> temp = new();
        foreach (S_CardBase card in fieldCards)
        {
            card.IsCurrentTurn = false;

            // 고정이라면
            if (card.OriginEngraving.Contains(S_EngravingEnum.Fix) || card.Engraving.Contains(S_EngravingEnum.Fix)) 
            {
                // 넘어가기
            }
            // 지속 효과 : 저주받은 카드는 고정
            else if (GetPersistCardsInField(S_PersistEnum.Fix_FixCursedCard).Count > 0 && card.IsCursed)
            {
                // 넘어가기
            }
            else // 아니라면 사용한 카드 더미로 이동
            {
                card.IsCurrentTurn = false;
                card.IsInDeck = false;
                card.IsInUsed = true;
                temp.Add(card);
                usedCards.Add(card);
                S_DeckInfoSystem.Instance.AddUsedByStand(card);
                S_FieldInfoSystem.Instance.RemoveFieldCard(card);
            }
        }
        foreach (S_CardBase card in temp)
        {
            fieldCards.Remove(card);
        }
        S_DeckInfoSystem.Instance.AlignmentViewUsedCards(); // 사용한 카드 더미 정렬
        await S_FieldInfoSystem.Instance.AlignmentFieldCards();
    }
    public async Task ResetCardsByEndTrial()
    {
        // 사용한 카드 더미 비우기 연출
        S_DeckInfoSystem.Instance.VacateViewUsedCards();
        // 덱 카드 채우기 연출
        S_DeckInfoSystem.Instance.AddDecks(GetOriginPlayerDeckCards());
        S_DeckInfoSystem.Instance.UpdateCardsState();
        S_DeckInfoSystem.Instance.ActivateViewDeckObj();

        // 덱 카드 정상화
        foreach (S_CardBase card in originPlayerDeck)
        {
            card.IsInDeck = true;
            card.IsInUsed = false;
            card.IsCurrentTurn = false;
            card.IsCursed = false;

            card.ExpectedValue = 0;
            card.HitCount = 0;
            card.ReboundCount = 0;
        }

        fieldCards.Clear();
        usedCards.Clear();
        deckCards = GetOriginPlayerDeckCards();

        // 필드에 있는 모든 카드가 돌아오는 연출
        await S_FieldInfoSystem.Instance.ResetCardsByEndTrialAsync();
    }
    #endregion
    #region 카드
    public async Task HitCard(S_CardBase card) // 카드를 내는 메서드
    {
        deckCards.Remove(card);
        fieldCards.Add(card);

        card.IsInDeck = false;
        card.IsInUsed = false;
        card.IsCurrentTurn = true;

        card.HitCount++;

        S_DeckInfoSystem.Instance.RemoveDeckCard(card);
        await S_FieldInfoSystem.Instance.HitFieldCard(card);
        UpdateCardObjsState();
    }
    public void FlexibleCard(S_CardBase card)
    {
        int index = fieldCards.IndexOf(card);
        if (index == -1 || index == fieldCards.Count - 1) return; // 없거나 이미 마지막이면 할 필요 없음

        fieldCards.RemoveAt(index);
        fieldCards.Add(card);
    }
    public void LeapCard(S_CardBase card)
    {
        int index = fieldCards.IndexOf(card);
        if (index <= 0) return; // 없거나 이미 첫 번째면 무시

        fieldCards.RemoveAt(index);
        fieldCards.Insert(0, card);
    }
    public void UpdateCardObjsState()
    {
        S_FieldInfoSystem.Instance.UpdateCardState();
        S_DeckInfoSystem.Instance.UpdateCardsState();
    }
    #endregion
    #region 보조 메서드
    public List<S_CardBase> GetOriginPlayerDeckCards()
    {
        return originPlayerDeck.ToList();
    }
    public List<S_CardBase> GetDeckCards()
    {
        return deckCards.ToList();
    }
    public List<S_CardBase> GetUsedCards()
    {
        return usedCards.ToList();
    }
    public List<S_CardBase> GetFieldCards()
    {
        return fieldCards.ToList();
    }
    public List<S_CardBase> GetPersistCardsInField(S_PersistEnum persist)
    {
        List<S_CardBase> result = new();
        foreach (S_CardBase card in GetFieldCards().Where(x => !x.IsCursed).ToList())
        {
            foreach (S_PersistStruct pers in card.Persist)
            {
                if (pers.Persist == persist)
                {
                    result.Add(card);
                    break;
                }
            }
        }

        return result;
    }
    public List<S_CardBase> GetPersistCardsInRight(S_CardBase card, S_PersistEnum persist)
    {
        List<S_CardBase> result = new();

        int startIndex = fieldCards.IndexOf(card);
        if (startIndex == -1) return result; // 안전장치: card가 없으면 빈 리스트 반환

        for (int i = startIndex + 1; i < fieldCards.Count; i++) // 오른쪽 카드만 검사
        {
            S_CardBase fieldCard = fieldCards[i];
            if (fieldCard.IsCursed) continue;
            foreach (S_PersistStruct pers in fieldCard.Persist)
            {
                if (pers.Persist == persist)
                {
                    result.Add(fieldCard);
                    break;
                }
            }
        }

        return result;
    }
    public List<S_CardBase> GetPersistCardsInLeft(S_CardBase card, S_PersistEnum persist)
    {
        List<S_CardBase> result = new();

        int startIndex = fieldCards.IndexOf(card);
        if (startIndex == -1) return result; // 안전장치: card가 없으면 빈 리스트 반환

        for (int i = startIndex - 1; i >= 0; i--) // 왼쪽 카드만 검사
        {
            S_CardBase fieldCard = fieldCards[i];
            if (fieldCard.IsCursed) continue;
            foreach (S_PersistStruct pers in fieldCard.Persist)
            {
                if (pers.Persist == persist)
                {
                    result.Add(fieldCard);
                    break;
                }
            }
        }

        return result;
    }
    public S_CardBase GetPersistCardInRight(S_CardBase card, S_PersistEnum persist)
    {
        S_CardBase result = null;

        int index = fieldCards.IndexOf(card);
        if (index == -1 || index >= fieldCards.Count - 1) return result; // 카드가 첫 번째거나 없으면 왼쪽 없음

        S_CardBase rightCard = fieldCards[index + 1];

        if (rightCard.IsCursed) return result;

        foreach (S_PersistStruct pers in rightCard.Persist)
        {
            if (pers.Persist == persist)
            {
                result = rightCard;
                break;
            }
        }

        return result;
    }
    public S_CardBase GetPersistCardInLeft(S_CardBase card, S_PersistEnum persist)
    {
        S_CardBase result = null;

        int index = fieldCards.IndexOf(card);
        if (index <= 0) return result; // 카드가 첫 번째거나 없으면 왼쪽 없음

        S_CardBase leftCard = fieldCards[index - 1];

        if (leftCard.IsCursed) return result;

        foreach (S_PersistStruct pers in leftCard.Persist)
        {
            if (pers.Persist == persist)
            {
                result = leftCard;
                break;
            }
        }

        return result;
    }
    #endregion
}