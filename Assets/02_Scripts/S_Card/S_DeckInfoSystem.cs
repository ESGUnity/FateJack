using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_DeckInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_DeckCard;
    [SerializeField] GameObject prefab_ShowingCard;
    [SerializeField] Material mat_Origin;
    [SerializeField] Material mat_BrightEffect;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject pos_DeckBase;
    [SerializeField] GameObject obj_ViewDeck;
    [SerializeField] GameObject pos_UsedBase;
    [SerializeField] GameObject obj_ViewUsed;
    [SerializeField] GameObject text_DeckCount;

    [Header("컴포넌트")]
    GameObject panel_DeckBtnBase;

    [Header("덱 카드 오브젝트 리스트")]
    List<GameObject> deckCardObjs = new();
    List<GameObject> usedCardObjs = new();

    [Header("UI")]
    Vector2 DECK_BTN_HIDE_POS = new Vector2(0, -80);
    Vector2 DECK_BTN_ORIGIN_POS = new Vector2(0, 89);
    S_GameFlowStateEnum prevState;

    [Header("연출")]
    Vector3 DECK_BASE_START_POS = new Vector3(-8.2f, 0, 0);
    Vector3 DECK_BASE_END_POS = new Vector3(8.2f, 0, 0);
    const float STACK_Z_VALUE = -0.002f;
    Vector3 STACK_CARD_ORIGIN_SCALE = new Vector3(1.7f, 1.7f, 1.7f);
    Vector3 CARDS_ORIGIN_POS = new Vector3(0, -1.5f, 0);
    Vector3 CARDS_HIDE_POS = new Vector3(0, -10f, 0);
    Vector3 CARD_SPAWN_POS = new Vector3(0, 0, -9f);

    S_OrderStateEnum orderState;
    const float LEAN_VALUE = 3.5f;

    // 싱글턴
    static S_DeckInfoSystem instance;
    public static S_DeckInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        panel_DeckBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_DeckBtnBase")).gameObject;

        // TMP의 소팅오더 조절
        text_DeckCount.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        text_DeckCount.GetComponent<MeshRenderer>().sortingOrder = 0;
        text_DeckCount.GetComponent<TMP_Text>().raycastTarget = false;

        prevState = S_GameFlowStateEnum.None;
        orderState = S_OrderStateEnum.GetOrder;

        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 섹션의 스프라이트에 포인터 엔터 바인딩하기.
        EventTrigger spriteTrigger = obj_ViewDeck.GetComponent<EventTrigger>();
        EventTrigger.Entry spritePointerEnterEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);
        EventTrigger.Entry spritePointerExitEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerExit);
        EventTrigger.Entry spritePointerClickEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
        // 바인딩
        spritePointerEnterEntry.callback.AddListener((eventData) => { PointerEnterViewDeckObj((PointerEventData)eventData); });
        spritePointerExitEntry.callback.AddListener((eventData) => { PointerExitViewDeckObj((PointerEventData)eventData); });
        spritePointerClickEntry.callback.AddListener((eventData) => { ClickViewDeckObj((PointerEventData)eventData); });

        EventTrigger usedSprite = obj_ViewUsed.GetComponent<EventTrigger>();
        EventTrigger.Entry usedEnterEntry = usedSprite.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);
        EventTrigger.Entry usedExitEntry = usedSprite.triggers.Find(e => e.eventID == EventTriggerType.PointerExit);
        EventTrigger.Entry usedClickEntry = usedSprite.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
        // 바인딩
        usedEnterEntry.callback.AddListener((eventData) => { PointerEnterViewUsedObj((PointerEventData)eventData); });
        usedExitEntry.callback.AddListener((eventData) => { PointerExitViewUsedObj((PointerEventData)eventData); });
        usedClickEntry.callback.AddListener((eventData) => { ClickViewUsedObj((PointerEventData)eventData); });
    }

    #region 덱 관련
    public void AddDeck(S_CardBase card) // 덱에 카드 추가
    {
        GameObject go = Instantiate(prefab_DeckCard, pos_DeckBase.transform, false); // 덱 카드 프리팹 생성
        go.GetComponent<S_DeckCardObj>().SetCardInfo(card); // 카드 정보 설정
        deckCardObjs.Add(go); // 덱 카드 오브젝트에 추가

        AlignmentDeckCards();

        UpdateCardsState();
    }
    public void AddDecks(List<S_CardBase> cards)
    {
        foreach (GameObject go in deckCardObjs)
        {
            Destroy(go);
        }
        deckCardObjs.Clear();

        foreach (GameObject go in usedCardObjs)
        {
            Destroy(go);
        }
        usedCardObjs.Clear();

        foreach (S_CardBase card in cards)
        {
            AddDeck(card);
        }
    }
    public void RemoveDeckCard(S_CardBase card) // 덱에서 카드 제거
    {
        GameObject removeObj = null;
        foreach (GameObject go in deckCardObjs)
        {
            if (go.GetComponent<S_DeckCardObj>().CardInfo == card)
            {
                removeObj = go;
                break;
            }
        }

        deckCardObjs.Remove(removeObj);
        Destroy(removeObj);

        CountViewDeckObj();

        AlignmentDeckCards();
    }
    void AlignmentDeckCards() // 카드를 정렬
    {
        List<PRS> originCardPRS = SetCardsPos(deckCardObjs.Count);
        List<Task> tweenTask = new List<Task>();

        switch (orderState)
        {
            case S_OrderStateEnum.GetOrder:
                List<S_CardBase> decks = S_PlayerCard.Instance.GetOriginPlayerDeckCards();
                deckCardObjs.Sort((a, b) =>
                {
                    var aCard = a.GetComponent<S_DeckCardObj>().CardInfo;
                    var bCard = b.GetComponent<S_DeckCardObj>().CardInfo;

                    int aIndex = decks.IndexOf(aCard);
                    int bIndex = decks.IndexOf(bCard);

                    // 못 찾은 경우는 맨 뒤로 보내기 (IndexOf가 -1이면 int.MaxValue로 대체)
                    if (aIndex == -1) aIndex = int.MaxValue;
                    if (bIndex == -1) bIndex = int.MaxValue;

                    return aIndex.CompareTo(bIndex);
                });
                break;
            case S_OrderStateEnum.WeightOrder:
                deckCardObjs.Sort((a, b) => a.GetComponent<S_DeckCardObj>().CardInfo.Weight.CompareTo(b.GetComponent<S_DeckCardObj>().CardInfo.Weight));
                break;
            case S_OrderStateEnum.TypeOrder:
                // 우선 CardTypeEnum 순서 정해서 인덱스로 비교할 거임
                List<S_CardTypeEnum> order = new() { S_CardTypeEnum.Str, S_CardTypeEnum.Mind, S_CardTypeEnum.Luck, S_CardTypeEnum.Common };

                deckCardObjs.Sort((a, b) =>
                {
                    var aCard = a.GetComponent<S_DeckCardObj>().CardInfo;
                    var bCard = b.GetComponent<S_DeckCardObj>().CardInfo;

                    int aTypeOrder = order.IndexOf(aCard.CardType);
                    int bTypeOrder = order.IndexOf(bCard.CardType);

                    // CardType 순서로 먼저 비교
                    int typeCompare = aTypeOrder.CompareTo(bTypeOrder);

                    if (typeCompare != 0)
                    {
                        return typeCompare;  // 타입 순서 다르면 그 순서대로
                    }

                    return aCard.Weight.CompareTo(bCard.Weight); // CardType 같으면 Num 기준 오름차순 정렬
                });
                break;
        }

        for (int i = 0; i < deckCardObjs.Count; i++)
        {
            // PRS 설정
            deckCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            deckCardObjs[i].GetComponent<S_DeckCardObj>().OriginOrder = (i + 1) * 10;
            deckCardObjs[i].GetComponent<S_DeckCardObj>().SetOrder(deckCardObjs[i].GetComponent<S_DeckCardObj>().OriginOrder);

            // 카드의 위치 설정
            deckCardObjs[i].transform.DOKill();
            deckCardObjs[i].transform.DOLocalMove(deckCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            deckCardObjs[i].transform.DOLocalRotate(deckCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            deckCardObjs[i].transform.DOScale(deckCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }

        UpdateCardsState();
    }
    #endregion
    #region 사용한 카드 더미 관련
    public void AddUsedByStand(S_CardBase card) // 사용한 카드 더미에 카드 추가(스탠드 시)
    {
        // 연출
        GameObject fieldObj = S_FieldInfoSystem.Instance.GetFieldCardObj(card);
        GameObject usedObj = Instantiate(prefab_ShowingCard, obj_ViewUsed.transform);
        usedObj.GetComponent<S_ShowingCardObj>().SetCardInfo(card);
        usedObj.transform.position = fieldObj.transform.position;
        usedObj.transform.eulerAngles = fieldObj.transform.eulerAngles;
        usedObj.transform.localScale = fieldObj.transform.localScale;
        usedObj.GetComponent<S_ShowingCardObj>().CARD_ROT = new Vector3(0, 0, UnityEngine.Random.Range(-LEAN_VALUE, LEAN_VALUE));
        S_FieldInfoSystem.Instance.RemoveFieldCardObj(card);

        // 실제 사용한 더미 카드에 추가
        GameObject go = Instantiate(prefab_DeckCard, pos_UsedBase.transform, false); // 덱 카드 프리팹 생성
        go.GetComponent<S_DeckCardObj>().SetCardInfo(card); // 카드 정보 설정
        usedCardObjs.Add(go); // 사용한 카드 더미 오브젝트에 추가
        AlignmentUsedCards();
    }
    public void RemoveUsed(S_CardBase card) // 사용한 카드 더미에서 카드 제거
    {
        GameObject removeObj = null;
        foreach (GameObject go in usedCardObjs)
        {
            if (go.GetComponent<S_DeckCardObj>().CardInfo == card)
            {
                removeObj = go;
                break;
            }
        }

        usedCardObjs.Remove(removeObj);
        Destroy(removeObj);

        AlignmentUsedCards();
    }
    void AlignmentUsedCards() // 카드를 정렬
    {
        List<PRS> originCardPRS = SetCardsPos(usedCardObjs.Count);
        List<Task> tweenTask = new List<Task>();

        switch (orderState)
        {
            case S_OrderStateEnum.GetOrder:
                List<S_CardBase> decks = S_PlayerCard.Instance.GetOriginPlayerDeckCards();
                usedCardObjs.Sort((a, b) =>
                {
                    var aCard = a.GetComponent<S_DeckCardObj>().CardInfo;
                    var bCard = b.GetComponent<S_DeckCardObj>().CardInfo;

                    int aIndex = decks.IndexOf(aCard);
                    int bIndex = decks.IndexOf(bCard);

                    // 못 찾은 경우는 맨 뒤로 보내기 (IndexOf가 -1이면 int.MaxValue로 대체)
                    if (aIndex == -1) aIndex = int.MaxValue;
                    if (bIndex == -1) bIndex = int.MaxValue;

                    return aIndex.CompareTo(bIndex);
                });
                break;
            case S_OrderStateEnum.WeightOrder:
                usedCardObjs.Sort((a, b) => a.GetComponent<S_DeckCardObj>().CardInfo.Weight.CompareTo(b.GetComponent<S_DeckCardObj>().CardInfo.Weight));
                break;
            case S_OrderStateEnum.TypeOrder:
                // 우선 CardTypeEnum 순서 정해서 인덱스로 비교할 거임
                List<S_CardTypeEnum> order = new() { S_CardTypeEnum.Str, S_CardTypeEnum.Mind, S_CardTypeEnum.Luck, S_CardTypeEnum.Common };

                usedCardObjs.Sort((a, b) =>
                {
                    var aCard = a.GetComponent<S_DeckCardObj>().CardInfo;
                    var bCard = b.GetComponent<S_DeckCardObj>().CardInfo;

                    int aTypeOrder = order.IndexOf(aCard.CardType);
                    int bTypeOrder = order.IndexOf(bCard.CardType);

                    // CardType 순서로 먼저 비교
                    int typeCompare = aTypeOrder.CompareTo(bTypeOrder);

                    if (typeCompare != 0)
                    {
                        return typeCompare;  // 타입 순서 다르면 그 순서대로
                    }

                    return aCard.Weight.CompareTo(bCard.Weight); // CardType 같으면 Num 기준 오름차순 정렬
                });
                break;
        }

        for (int i = 0; i < usedCardObjs.Count; i++)
        {
            // PRS 설정
            usedCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            usedCardObjs[i].GetComponent<S_DeckCardObj>().OriginOrder = (i + 1) * 10;
            usedCardObjs[i].GetComponent<S_DeckCardObj>().SetOrder(usedCardObjs[i].GetComponent<S_DeckCardObj>().OriginOrder);

            // 카드의 위치 설정
            usedCardObjs[i].transform.DOKill();
            usedCardObjs[i].transform.DOLocalMove(usedCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            usedCardObjs[i].transform.DOLocalRotate(usedCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            usedCardObjs[i].transform.DOScale(usedCardObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }

        UpdateCardsState();
    }
    public void AlignmentViewUsedCards()
    {
        int count = 0;
        foreach (Transform t in obj_ViewUsed.transform)
        {
            // 소팅오더 설정
            t.GetComponent<S_ShowingCardObj>().OriginOrder = (count + 1) * 10;
            t.GetComponent<S_ShowingCardObj>().SetOrder((count + 1) * 10);

            // 카드의 위치 설정
            t.transform.DOKill();
            t.transform.DOLocalMove(Vector3.zero, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            t.transform.DOLocalRotate(t.GetComponent<S_ShowingCardObj>().CARD_ROT, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);

            count++;
        }
    }
    public void VacateViewUsedCards() // 시련 종료나 덱의 카드를 다 써서 순환 시 사용
    {
        // 사용한 카드 더미 덱 뭉치 제거
        foreach (Transform t in obj_ViewUsed.transform)
        {
            // 카드의 위치 설정
            t.transform.DOKill();
            t.transform.DOMove(CARD_SPAWN_POS, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }
        foreach (Transform t in obj_ViewUsed.transform)
        {
            Destroy(t.gameObject);
        }

        foreach (GameObject go in usedCardObjs)
        {
            go.GetComponent<S_DeckCardObj>().transform.SetParent(pos_DeckBase.transform);
        }

        deckCardObjs.AddRange(usedCardObjs.ToList());
        usedCardObjs.Clear();

        AlignmentDeckCards();
    }
    #endregion
    #region 버튼 함수
    public async void ClickViewDeckObj(PointerEventData eventData) // 덱 보기 스프라이트 클릭 시
    {
        // Dialog 시스템 전용. 다이얼로그에서 GameFlowState를 Hit으로 바꿔줘야한다.
        S_DialogInfoSystem.Instance.ClickNextBtn();

        if (!S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Deck)) // 덱이 아니라면 덱 열기
        {
            pos_DeckBase.transform.DOLocalMove(CARDS_ORIGIN_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
            pos_UsedBase.transform.DOLocalMove(CARDS_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Used))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 덱에 오기 전 State를 저장
                prevState = S_GameFlowStateEnum.Used;

                // 대기
                await S_GameFlowManager.WaitPanelAppearTimeAsync();
            }
            else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Hit))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                S_HitBtnSystem.Instance.DisappearHitBtn();

                await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Hit);
            }
            else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Store))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                S_RewardInfoSystem.Instance.DisappearExitBtn();

                await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Store);
            }
            else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.StoreBuying))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                S_RewardInfoSystem.Instance.DisappearMonologByBuiedProduct();

                await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.StoreBuying);
            }

            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Deck;

        }
        else // 덱이라면 덱 닫기
        {
            if (prevState == S_GameFlowStateEnum.Hit || prevState == S_GameFlowStateEnum.Used)
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 시련 중인 UI 재등장(히트 버튼)
                S_HitBtnSystem.Instance.AppearHitBtn();

                // 덱 정보 닫기
                await CloseDeckInfoCommonProperty();

                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Hit;
            }
            else if (prevState == S_GameFlowStateEnum.Store)
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 상점 UI 재등장
                S_RewardInfoSystem.Instance.AppearExitBtn();

                // 덱 정보 닫기
                await CloseDeckInfoCommonProperty();
            }
            else if (prevState == S_GameFlowStateEnum.StoreBuying)
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 상점 UI 재등장
                S_RewardInfoSystem.Instance.AppearMonlogByBuiedProduct();

                // 덱 정보 닫기
                await CloseDeckInfoCommonProperty();
            }
        }

        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Deck)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetDeckCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }
        else if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Used)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetUsedCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }
    }
    public void PointerEnterViewDeckObj(PointerEventData eventData)
    {
        foreach (Transform t in obj_ViewDeck.transform)
        {
            t.GetComponent<SpriteRenderer>().material = mat_BrightEffect;
        }
    }
    public void PointerExitViewDeckObj(PointerEventData eventData)
    {
        foreach (Transform t in obj_ViewDeck.transform)
        {
            t.GetComponent<SpriteRenderer>().material = mat_Origin;
        }
    }
    public void CountViewDeckObj()
    {
        int count = 0;
        foreach (Transform t in obj_ViewDeck.transform)
        {
            if (count >= deckCardObjs.Count)
            {
                t.gameObject.SetActive(false);
            }
            else
            {
                t.gameObject.SetActive(true);
            }

            count++;
        }
    }
    public void ActivateViewDeckObj()
    {
        foreach (Transform t in obj_ViewDeck.transform)
        {
            t.gameObject.SetActive(true);
        }
    }
    public async void ClickViewUsedObj(PointerEventData eventData) // 사용한 카드 더미 보기 스프라이트 클릭 시
    {
        if (usedCardObjs.Count == 0) return;

        // Dialog 시스템 전용. 다이얼로그에서 GameFlowState를 Hit으로 바꿔줘야한다.
        S_DialogInfoSystem.Instance.ClickNextBtn();

        if (!S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Used)) // 덱이 아니라면 덱 열기
        {
            pos_UsedBase.transform.DOLocalMove(CARDS_ORIGIN_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
            pos_DeckBase.transform.DOLocalMove(CARDS_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Deck))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 덱에 오기 전 State를 저장
                prevState = S_GameFlowStateEnum.Deck;

                // 대기
                await S_GameFlowManager.WaitPanelAppearTimeAsync();
            }
            else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Hit))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                S_HitBtnSystem.Instance.DisappearHitBtn();

                await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Hit);
            }
            else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Store))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                S_RewardInfoSystem.Instance.DisappearExitBtn();

                await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Store);
            }
            else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.StoreBuying))
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                S_RewardInfoSystem.Instance.DisappearMonologByBuiedProduct();

                await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.StoreBuying);
            }

            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Used;
        }
        else // 덱이라면 덱 닫기
        {
            if (prevState == S_GameFlowStateEnum.Hit || prevState == S_GameFlowStateEnum.Deck)
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 시련 중인 UI 재등장(히트 버튼)
                S_HitBtnSystem.Instance.AppearHitBtn();

                // 덱 정보 닫기
                await CloseDeckInfoCommonProperty();

                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Hit;
            }
            else if (prevState == S_GameFlowStateEnum.Store)
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 상점 UI 재등장
                S_RewardInfoSystem.Instance.AppearExitBtn();

                // 덱 정보 닫기
                await CloseDeckInfoCommonProperty();
            }
            else if (prevState == S_GameFlowStateEnum.StoreBuying)
            {
                S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

                // 상점 UI 재등장
                S_RewardInfoSystem.Instance.AppearMonlogByBuiedProduct();

                // 덱 정보 닫기
                await CloseDeckInfoCommonProperty();
            }
        }

        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Deck)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetDeckCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }
        else if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Used)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetUsedCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }
    }
    public void PointerEnterViewUsedObj(PointerEventData eventData)
    {
        if (usedCardObjs.Count == 0) return;

        foreach (Transform t in obj_ViewUsed.transform)
        {
            t.GetComponent<S_ShowingCardObj>().sprite_CardEffect.material = mat_BrightEffect;
            t.GetComponent<S_ShowingCardObj>().sprite_CardFrame.material = mat_BrightEffect;
            t.GetComponent<S_ShowingCardObj>().sprite_Engraving.material = mat_BrightEffect;
        }
    }
    public void PointerExitViewUsedObj(PointerEventData eventData)
    {
        if (usedCardObjs.Count == 0) return;

        foreach (Transform t in obj_ViewUsed.transform)
        {
            t.GetComponent<S_ShowingCardObj>().sprite_CardEffect.material = mat_Origin;
            t.GetComponent<S_ShowingCardObj>().sprite_CardFrame.material = mat_Origin;
            t.GetComponent<S_ShowingCardObj>().sprite_Engraving.material = mat_Origin;
        }
    }
    public void ClickGetOrderBtn() // 획득 순 정렬
    {
        orderState = S_OrderStateEnum.GetOrder;

        AlignmentDeckCards();
        AlignmentUsedCards();
    }
    public void ClickWeightOrderBtn() // 무게 순 정렬
    {
        orderState = S_OrderStateEnum.WeightOrder;

        AlignmentDeckCards();
        AlignmentUsedCards();
    }
    public void ClickTypeOrderBtn() // 타입 순 정렬
    {
        orderState = S_OrderStateEnum.TypeOrder;

        AlignmentDeckCards();
        AlignmentUsedCards();
    }
    #endregion
    #region 보조 함수
    List<PRS> SetCardsPos(int cardCount) // 카드 위치 설정하는 메서드
    {
        List<PRS> results = new List<PRS>(cardCount);
        float interval = cardCount == 1 ? 0 : 1f / (cardCount - 1);

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(DECK_BASE_START_POS, DECK_BASE_END_POS, interval * i);
            pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);

            Vector3 rot = Vector3.zero;

            Vector3 scale = STACK_CARD_ORIGIN_SCALE;

            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void UpdateCardsState() // 저주, 덱에 없는 상태, 카드의 개수 현황 업데이트
    {
        // InDeck과 Cursed 업데이트
        foreach (GameObject go in deckCardObjs)
        {
            go.GetComponent<S_DeckCardObj>().UpdateCardState();
        }

        // 텍스트 업데이트
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Deck)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetDeckCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }
        else if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Used)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetUsedCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }
    }
    public void SetDeckCardInfo() // 상점 전용 업데이트
    {
        foreach (GameObject go in deckCardObjs)
        {
            go.GetComponent<S_DeckCardObj>().SetCardInfo(go.GetComponent<S_DeckCardObj>().CardInfo); // 변한 카드들을 모두 재설정 해준다.
            go.GetComponent<S_DeckCardObj>().UpdateCardState();
        }

        // 텍스트 업데이트
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Deck)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetDeckCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }
        else if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Used)
        {
            text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetUsedCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";
        }

        AlignmentDeckCards();
    }
    public async Task OpenDeckInfoCommonProperty(S_GameFlowStateEnum prev)
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 덱에 오기 전 State를 저장
        prevState = prev;

        // 덱 정보에서 사용할 버튼 등장
        panel_DeckBtnBase.SetActive(true);
        panel_DeckBtnBase.GetComponent<RectTransform>().DOKill();
        panel_DeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(DECK_BTN_ORIGIN_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // 카메라 이동
        S_CameraManager.Instance.MoveToPosition(S_CameraManager.DeckCameraPos, S_CameraManager.DeckCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME);

        // 카메라 이동 대기
        await S_GameFlowManager.WaitPanelAppearTimeAsync();
    }
    public async Task CloseDeckInfoCommonProperty()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 덱 버튼 퇴장
        panel_DeckBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_DeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(DECK_BTN_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_DeckBtnBase.SetActive(false));

        // 카메라 이동
        S_CameraManager.Instance.MoveToPosition(S_CameraManager.InGameCameraPos, S_CameraManager.InGameCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME);

        // UI 연출 대기
        await S_GameFlowManager.WaitPanelAppearTimeAsync();

        S_GameFlowManager.Instance.GameFlowState = prevState;
    }
    public List<GameObject> GetDeckCardObjs()
    {
        return deckCardObjs.ToList();
    }
    #endregion
}

public enum S_OrderStateEnum
{
    None, GetOrder, WeightOrder, TypeOrder
}