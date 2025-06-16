using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_DeckInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject deckCard;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject pos_DeckBase;
    [SerializeField] GameObject text_DeckCount;
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByDeckInfo;
    [SerializeField] GameObject sprite_ViewDeck;

    [Header("컴포넌트")]
    GameObject panel_DeckBtnBase;

    [Header("덱 카드 오브젝트 리스트")]
    List<GameObject> deckCardObjs = new();

    [Header("UI")]
    Vector2 DECK_BTN_HIDE_POS = new Vector2(0, -80);
    Vector2 DECK_BTN_ORIGIN_POS = new Vector2(0, 85);
    S_GameFlowStateEnum prevState;

    [Header("연출")]
    Vector3 DECK_BASE_START_POS = new Vector3(-8.2f, 0, 0);
    Vector3 DECK_BASE_END_POS = new Vector3(8.2f, 0, 0);
    const float STACK_Z_VALUE = -0.02f;
    Vector3 STACK_CARD_ORIGIN_SCALE = new Vector3(1.7f, 1.7f, 1.7f);

    S_OrderStateEnum orderState = S_OrderStateEnum.GetOrder;

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

    public void AddDeck(S_Card card) // 덱에 카드 추가하기
    {
        GameObject go = Instantiate(deckCard); // 덱 카드 프리팹 생성

        go.GetComponent<S_DeckCardObj>().SetCardInfo(card); // 카드 정보 설정
        deckCardObjs.Add(go); // 스택 카드 오브젝트에 추가

        AlignmentDeckCards();
    }
    void AlignmentDeckCards() // 카드를 정렬하기
    {
        List<PRS> originCardPRS = SetDeckCardsPos(deckCardObjs.Count);
        List<Task> tweenTask = new List<Task>();

        switch (orderState)
        {
            case S_OrderStateEnum.GetOrder:
                List<S_Card> decks = S_PlayerCard.Instance.GetOriginPlayerDeckCards();
                deckCardObjs.Sort((a, b) =>
                {
                    var aCard = a.GetComponent<S_DeckCardObj>().CardInfo;
                    var bCard = b.GetComponent<S_DeckCardObj>().CardInfo;

                    int aIndex = decks.IndexOf(aCard);
                    int bIndex = decks.IndexOf(bCard);

                    return aIndex.CompareTo(bIndex);
                });
                break;
            case S_OrderStateEnum.WeightOrder:
                deckCardObjs.Sort((a, b) => a.GetComponent<S_DeckCardObj>().CardInfo.Num.CompareTo(b.GetComponent<S_DeckCardObj>().CardInfo.Num));
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

                    return aCard.Num.CompareTo(bCard.Num); // CardType 같으면 Num 기준 오름차순 정렬
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

        UpdateDeckCardsState();
    }
    List<PRS> SetDeckCardsPos(int cardCount) // 카드 위치 설정하는 메서드
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
    public void RemoveDeck(S_Card card) // 덱에서 카드 제거하기
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

        AlignmentDeckCards();
    }
    #region 버튼 함수
    public async void ClickViewDeckSprite() // 덱 보기 스프라이트 클릭 시
    {
        if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Hit))
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // 인게임 인터페이스를 숨기기(히트 버튼)
            S_HitBtnSystem.Instance.DisappearHitBtn();

            await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Hit);
        }
        else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.Store))
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // Store UI 퇴장
            S_StoreInfoSystem.Instance.DisappearRefreshAndExitBtn();

            // 덱 열기
            await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.Store);
        }
        else if (S_GameFlowManager.Instance.IsGameFlowState(S_GameFlowStateEnum.StoreBuying))
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // Store UI 퇴장
            S_StoreInfoSystem.Instance.DisappearRefreshAndExitBtn();
            S_StoreInfoSystem.Instance.DisappearSelectCardOrTrinketText();
            S_StoreInfoSystem.Instance.DisappearBlackBackground();

            // 덱 열기
            await OpenDeckInfoCommonProperty(S_GameFlowStateEnum.StoreBuying);
        }
    }
    public async void ClickCloseDeckInfoBtn() // 덱 보기 닫을 때 호출
    {
        if (prevState == S_GameFlowStateEnum.Hit)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // 시련 중인 UI 재등장(히트 버튼)
            S_HitBtnSystem.Instance.AppearHitBtn();

            // 카메라 이동
            Camera.main.transform.DOMove(S_GameFlowManager.InGameCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
            Camera.main.transform.DORotate(S_GameFlowManager.InGameCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            // 덱 정보 닫기
            await CloseDeckInfoCommonProperty();
        }
        else if (prevState == S_GameFlowStateEnum.Store)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // 상점 UI 재등장
            S_StoreInfoSystem.Instance.AppearRefreshAndExitBtn();

            // 카메라 이동
            Camera.main.transform.DOMove(S_GameFlowManager.StoreCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
            Camera.main.transform.DORotate(S_GameFlowManager.StoreCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            // 덱 정보 닫기
            await CloseDeckInfoCommonProperty();
        }
        else if (prevState == S_GameFlowStateEnum.StoreBuying)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // 상점 UI 재등장
            S_StoreInfoSystem.Instance.AppearRefreshAndExitBtn();
            S_StoreInfoSystem.Instance.AppearSelectCardOrTrinketText(false);
            S_StoreInfoSystem.Instance.AppearBlackBackground();

            // 카메라 이동
            Camera.main.transform.DOMove(S_GameFlowManager.StoreCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
            Camera.main.transform.DORotate(S_GameFlowManager.StoreCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            // 덱 정보 닫기
            await CloseDeckInfoCommonProperty();
        }
    }
    public void ClickGetOrderBtn() // 획득 순 정렬
    {
        orderState = S_OrderStateEnum.GetOrder;

        AlignmentDeckCards();
    }
    public void ClickWeightOrderBtn() // 무게 순 정렬
    {
        orderState = S_OrderStateEnum.WeightOrder;

        AlignmentDeckCards();
    }
    public void ClickTypeOrderBtn() // 타입 순 정렬
    {
        orderState = S_OrderStateEnum.TypeOrder;

        AlignmentDeckCards();
    }
    #endregion
    #region 보조 함수
    public void UpdateDeckCardsState() // 저주, 덱에 없는 상태, 카드의 개수 현황 업데이트
    {
        // InDeck과 Cursed 업데이트
        foreach (GameObject go in deckCardObjs)
        {
            go.GetComponent<S_DeckCardObj>().UpdateCardState();
        }

        // 텍스트 업데이트
        text_DeckCount.GetComponent<TMP_Text>().text = $"{S_PlayerCard.Instance.GetDeckCards().Count} / {S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count}";

        AlignmentDeckCards();
    }
    public void SetDeckCardInfo() // 상점 전용 업데이트
    {
        foreach (GameObject go in deckCardObjs)
        {
            go.GetComponent<S_DeckCardObj>().SetCardInfo(go.GetComponent<S_DeckCardObj>().CardInfo); // 변한 카드들을 모두 재설정 해준다.
        }

        // 텍스트 업데이트
        text_DeckCount.GetComponent<TMP_Text>().text = $"{ S_PlayerCard.Instance.GetDeckCards().Count } / { S_PlayerCard.Instance.GetOriginPlayerDeckCards().Count }";

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

        // 살짝 어두워지게 만드는 효과
        sprite_BlackBackgroundByDeckInfo.DOKill();
        sprite_BlackBackgroundByDeckInfo.DOFade(0.51f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // 카메라 이동
        Camera.main.transform.DOMove(S_GameFlowManager.DeckCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        Camera.main.transform.DORotate(S_GameFlowManager.DeckCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // 카메라 이동 대기
        await S_GameFlowManager.WaitPanelAppearTimeAsync();

        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Deck;
    }
    public async Task CloseDeckInfoCommonProperty()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 덱 버튼 퇴장
        panel_DeckBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_DeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(DECK_BTN_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_DeckBtnBase.SetActive(false));

        // 어두워진걸 다시 풀어주기
        sprite_BlackBackgroundByDeckInfo.DOKill();
        sprite_BlackBackgroundByDeckInfo.DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

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