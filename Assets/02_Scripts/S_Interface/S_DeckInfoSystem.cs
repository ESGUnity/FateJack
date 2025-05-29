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
    [SerializeField] GameObject spadeDeckBase;
    [SerializeField] GameObject heartDeckBase;
    [SerializeField] GameObject diamondDeckBase;
    [SerializeField] GameObject cloverDeckBase;
    [SerializeField] GameObject selectCard;
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByDeckInfo;

    [SerializeField] GameObject text_SpadeCount;
    [SerializeField] GameObject text_HeartCount;
    [SerializeField] GameObject text_DiamondCount;
    [SerializeField] GameObject text_CloverCount;
    [SerializeField] GameObject text_ExclusionCount;
    [SerializeField] GameObject text_StackCount;
    [SerializeField] GameObject text_DeckCount;

    [Header("덱 카드 오브젝트 리스트")]
    List<GameObject> spadeCards = new();
    List<GameObject> heartCards = new();
    List<GameObject> diamondCards = new();
    List<GameObject> cloverCards = new();
    List<GameObject> deckCardObjects = new();

    [Header("컴포넌트")]
    GameObject panel_DeterminationHitBtnAndCancelBtnBase;
    GameObject panel_CloseDeckBtnBase;
    GameObject panel_ViewDeckInfoBtnBase;

    [Header("덱 열거나 닫는 UI")]
    Vector2 deckInfoBtnsHidePos = new Vector2(0, -80);
    Vector2 deckInfoBtnsOriginPos = new Vector2(0, 55);
    Vector2 viewDeckInfoBtnHidePos = new Vector2(-180, -10);
    Vector2 viewDeckInfoBtnOriginPos = new Vector2(10, -10);
    Vector3 deckCameraPos = new Vector3(0, -14, -15);
    Vector3 inGameCameraPos = new Vector3(0, 0, -15);
    Vector3 storeCameraPos = new Vector3(0, 10, -15);

    [Header("카드 UI")]
    Vector3 startPoint = new Vector3(-3.85f, 0, 0);
    Vector3 endPoint = new Vector3(3.85f, 0, 0);
    float zValue = -0.02f;

    [Header("의지 히트 관련")]
    bool isDeterminationHit;
    bool isSelect;
    S_Card selectedCard;

    // 싱글턴
    static S_DeckInfoSystem instance;
    public static S_DeckInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        panel_DeterminationHitBtnAndCancelBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_DeterminationHitBtnAndCancelBtnBase")).gameObject;
        panel_CloseDeckBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_CloseDeckBtnBase")).gameObject;
        panel_ViewDeckInfoBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_ViewDeckInfoBtnBase")).gameObject;

        // TMP의 소팅오더 조절
        text_SpadeCount.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        text_SpadeCount.GetComponent<MeshRenderer>().sortingOrder = 0;
        text_SpadeCount.GetComponent<TMP_Text>().raycastTarget = false;

        text_HeartCount.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        text_HeartCount.GetComponent<MeshRenderer>().sortingOrder = 0;
        text_HeartCount.GetComponent<TMP_Text>().raycastTarget = false;

        text_DiamondCount.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        text_DiamondCount.GetComponent<MeshRenderer>().sortingOrder = 0;
        text_DiamondCount.GetComponent<TMP_Text>().raycastTarget = false;

        text_CloverCount.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        text_CloverCount.GetComponent<MeshRenderer>().sortingOrder = 0;
        text_CloverCount.GetComponent<TMP_Text>().raycastTarget = false;

        text_ExclusionCount.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        text_ExclusionCount.GetComponent<MeshRenderer>().sortingOrder = 0;
        text_ExclusionCount.GetComponent<TMP_Text>().raycastTarget = false;

        text_StackCount.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        text_StackCount.GetComponent<MeshRenderer>().sortingOrder = 0;
        text_StackCount.GetComponent<TMP_Text>().raycastTarget = false;

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

        InitPos();
    }

    void InitPos()
    {
        panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().anchoredPosition = deckInfoBtnsHidePos;
        panel_DeterminationHitBtnAndCancelBtnBase.SetActive(false);
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().anchoredPosition = viewDeckInfoBtnHidePos;
        panel_ViewDeckInfoBtnBase.SetActive(false);
        sprite_BlackBackgroundByDeckInfo.DOFade(0f, 0f);
    }
    public void AppearViewDeckInfoBtn()
    {
        panel_ViewDeckInfoBtnBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOAnchorPos(viewDeckInfoBtnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearViewDeckInfoBtn()
    {
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOAnchorPos(viewDeckInfoBtnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_ViewDeckInfoBtnBase.SetActive(false));
    }

    public void AddDeck(S_Card card) // 덱에 카드 추가하기
    {
        GameObject go = Instantiate(deckCard); // 덱 카드 프리팹 생성

        go.GetComponent<S_DeckCard>().SetCardInfo(card); // 카드 정보 설정
        deckCardObjects.Add(go); // 스택 카드 오브젝트에 추가

        // 문양에 따라 베이스 정해주기
        switch (card.Suit)
        {
            case S_CardSuitEnum.Spade:
                go.transform.SetParent(spadeDeckBase.transform, true);
                spadeCards.Add(go);
                AlignmentStackCard(spadeCards);
                break;
            case S_CardSuitEnum.Heart:
                go.transform.SetParent(heartDeckBase.transform, true);
                heartCards.Add(go);
                AlignmentStackCard(heartCards);
                break;
            case S_CardSuitEnum.Diamond:
                go.transform.SetParent(diamondDeckBase.transform, true);
                diamondCards.Add(go);
                AlignmentStackCard(diamondCards);
                break;
            case S_CardSuitEnum.Clover:
                go.transform.SetParent(cloverDeckBase.transform, true);
                cloverCards.Add(go);
                AlignmentStackCard(cloverCards);
                break;
        }
    }
    void AlignmentStackCard(List<GameObject> suitCards) // 카드를 정렬하기
    {
        List<PRS> originCardPRS = SetStackCardPos(suitCards.Count);
        List<Task> tweenTask = new List<Task>();

        suitCards.Sort((a, b) => a.GetComponent<S_DeckCard>().CardInfo.Number.CompareTo(b.GetComponent<S_DeckCard>().CardInfo.Number));

        for (int i = 0; i < suitCards.Count; i++)
        {
            // 카드 위치 설정
            suitCards[i].GetComponent<S_DeckCard>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            suitCards[i].GetComponent<S_DeckCard>().OriginOrder = (i + 1) * 10;
            suitCards[i].GetComponent<S_DeckCard>().SetOrder(suitCards[i].GetComponent<S_DeckCard>().OriginOrder);

            // 카드의 위치 설정
            suitCards[i].GetComponent<Transform>().localPosition = suitCards[i].GetComponent<S_DeckCard>().OriginPRS.Pos;
            suitCards[i].GetComponent<Transform>().localScale = suitCards[i].GetComponent<S_DeckCard>().OriginPRS.Scale;
        }

        UpdateDeckCardsState();
    }
    List<PRS> SetStackCardPos(int cardCount) // 카드 위치 설정하는 메서드
    {
        float[] lerps = new float[cardCount];
        List<PRS> results = new List<PRS>(cardCount);

        if (cardCount == 1)
        {
            lerps[0] = 0;
        }
        else if (cardCount > 1)
        {
            float interval = 1f / (cardCount - 1);
            for (int i = 0; i < cardCount; i++)
            {
                lerps[i] = interval * i;
            }
        }

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(startPoint, endPoint, lerps[i]);
            pos = new Vector3(pos.x, pos.y, i * zValue);
            Vector3 rot = Vector3.zero;
            Vector3 scale = new Vector3(1, 1, 1);
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void RemoveDeck(S_Card card) // 덱에서 카드 제거하기
    {
        GameObject remove = null;
        foreach (GameObject go in deckCardObjects)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo.Equals(card))
            {
                remove = go;
                break;
            }
        }

        deckCardObjects.Remove(remove);
        switch (card.Suit)
        {
            case S_CardSuitEnum.Spade:
                spadeCards.Remove(remove);
                AlignmentStackCard(spadeCards);
                break;
            case S_CardSuitEnum.Heart:
                heartCards.Remove(remove);
                AlignmentStackCard(heartCards);
                break;
            case S_CardSuitEnum.Diamond:
                diamondCards.Remove(remove);
                AlignmentStackCard(diamondCards);
                break;
            case S_CardSuitEnum.Clover:
                cloverCards.Remove(remove);
                AlignmentStackCard(cloverCards);
                break;
        }

        Destroy(remove);

        UpdateDeckCardsState();
    }

    #region 버튼 함수
    public async void ClickViewDeckInfoBtn() // Open 덱 보기 버튼 함수
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Dialog || S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            await OpenDeckInfoCommonProperty(false);

            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.OnDeckInfo;
        }
        else if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // Store UI 전부 숨기기
            S_StoreInfoSystem.Instance.DisappearUIBySelectItem();
            S_StoreInfoSystem.Instance.DisappearUIByBuyStoreProduct();
            S_StoreInfoSystem.Instance.DisappearStoreTextInStore();
            S_StoreInfoSystem.Instance.DisappearBlackBackground();

            // 덱 열기
            await OpenDeckInfoCommonProperty(false);

            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
        }
    }
    public async void ClickDeterminationHitBtn() // Open 본 게임 화면의 의지 히트 버튼 함수
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Hit && !S_PlayerStat.Instance.IsBurst && S_PlayerStat.Instance.CanUseDetermination() && S_PlayerCard.Instance.GetPreDeckCards().Count > 0 && S_PlayerCard.Instance.GetPreStackCards().Count <= 48)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            await OpenDeckInfoCommonProperty(true);

            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.OnDeckInfo;
        }
        else if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {

        }
        else if (S_PlayerStat.Instance.IsBurst)
        {
            S_InGameUISystem.Instance.CreateLog("버스트 시엔 의지 히트도 할 수 없다는 것이네~!");
        }
        else if (!S_PlayerStat.Instance.CanUseDetermination())
        {
            S_InGameUISystem.Instance.CreateLog("의지가 부족하군.");
        }
        else if (S_PlayerCard.Instance.GetPreDeckCards().Count <= 0)
        {
            S_InGameUISystem.Instance.CreateLog("덱에 카드가 없어.");
        }
        else if (S_PlayerCard.Instance.GetPreStackCards().Count > 48)
        {
            S_InGameUISystem.Instance.CreateLog("더 이상 스택에 카드를 낼 수 없습니다. 최대 장수 : 48장");
        }
    }
    public async void ClickDecideDeterminationHitCardBtn() // Close. 의지 히트할 카드를 결정하는 버튼 함수
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.OnDeckInfo && S_PlayerStat.Instance.CanUseDetermination())
        {
            if (isSelect)
            {
                // 히트 시 의지 차감
                S_PlayerStat.Instance.UseDetermination();

                // 피조물과 전투 중엔 피조물과 히트 버튼 UI를 추가 등장해야함.
                S_FoeInfoSystem.Instance.AppearCreature();
                S_HitBtnSystem.Instance.AppearHitBtn();
                S_PlayerInfoSystem.Instance.AppearPlayerImage();

                // 카메라 이동
                Camera.main.transform.DOMove(inGameCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

                // 의지 히트 시 사용되는 selectCard 오브젝트 비활성화하고 선택 여부도 끄기
                selectCard.SetActive(false);
                isSelect = false;

                // 덱 정보 닫기
                await CloseDeckInfoCommonProperty();

                // 카드 내기
                await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(selectCard.GetComponent<S_DeckCard>().CardInfo, S_CardOrderTypeEnum.BasicHit);

                // 히트 카드 진행
                if (S_GameFlowManager.Instance.GetCardOrderQueueCount() <= 1)
                {
                    await S_GameFlowManager.Instance.StartHittingCard();
                }
            }
            else
            {
                S_InGameUISystem.Instance.CreateLog("의지 히트할 카드를 선택해.");
            }
        }
    }
    public async void ClickCloseDeckInfoBtn() // Close 덱 정보나 의지 히트 취소할 때 호출
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.OnDeckInfo)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // 피조물과 전투 중엔 피조물과 히트 버튼 UI를 추가 등장해야함.
            S_FoeInfoSystem.Instance.AppearCreature();
            S_HitBtnSystem.Instance.AppearHitBtn();
            S_PlayerInfoSystem.Instance.AppearPlayerImage();

            // 카메라 이동
            Camera.main.transform.DOMove(inGameCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            // 의지 히트 시 사용되는 selectCard 오브젝트 비활성화하고 선택 여부도 끄기
            selectCard.SetActive(false);
            isSelect = false;

            // 덱 정보 닫기
            await CloseDeckInfoCommonProperty();

            // 다시 Hit로 변환
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Hit;
        }
        else if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // 상점 UI 재정비
            S_StoreInfoSystem.Instance.AppearStoreTextInStore();
            if (S_StoreInfoSystem.Instance.CurrentBuyingProduct != null) // 구매한 아이템이 있을 때
            {
                S_StoreInfoSystem.Instance.AppearUIBySelectItem();
            }
            else // 없을 때
            {
                S_StoreInfoSystem.Instance.AppearUIByBuyStoreProduct();
            }

            // 카메라 이동
            Camera.main.transform.DOMove(storeCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            // 덱 정보 닫기
            await CloseDeckInfoCommonProperty();

            // 다시 Store로 변환
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
        }
    }
    public async Task OpenDeckInfoBySelectRemoveCardByStore()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 상점 UI 설정
        S_StoreInfoSystem.Instance.DisappearUIByBuyStoreProduct();
        S_StoreInfoSystem.Instance.AppearUIBySelectItem();
        S_StoreInfoSystem.Instance.DisappearStoreTextInStore();
        S_StoreInfoSystem.Instance.AppearSelectCardOrLootTextInStore();
        S_StoreInfoSystem.Instance.DisappearBlackBackground();

        // 덱 정보 UI 설정
        await OpenDeckInfoCommonProperty(false, false);

        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.StoreByRemove;
    }
    public async Task ClostDeckInfoByEndSelectRemoveCardByStore()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 상점 UI 설정
        S_StoreInfoSystem.Instance.AppearUIByBuyStoreProduct();
        S_StoreInfoSystem.Instance.DisappearUIBySelectItem();
        S_StoreInfoSystem.Instance.AppearStoreTextInStore();
        S_StoreInfoSystem.Instance.DisappearSelectCardOrLootTextInStore();
        S_StoreInfoSystem.Instance.DisappearBlackBackground();

        // 카메라 이동
        Camera.main.transform.DOMove(storeCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // 덱 정보 닫기
        await CloseDeckInfoCommonProperty();

        // 다시 Store로 변환
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
    }
    #endregion
    #region 의지 히트 시 호출되는 함수
    public void SelectCardByDeterminationHit(S_Card card)
    {
        if (isDeterminationHit)
        {
            isSelect = true;
            selectCard.SetActive(true);
            selectCard.GetComponent<S_DeckCard>().IsSelectedByDetermination = true;
            selectCard.GetComponent<S_DeckCard>().SetCardInfo(card);
            selectCard.GetComponent<S_DeckCard>().OriginPRS = new PRS(selectCard.transform.localPosition, selectCard.transform.eulerAngles, selectCard.transform.localScale);
            selectCard.GetComponent<S_DeckCard>().OriginOrder = 100;
            selectCard.GetComponent<S_DeckCard>().SetOrder(selectCard.GetComponent<S_DeckCard>().OriginOrder);
        }
    }
    public void CancelSelectCardByDeterminationHit()
    {
        if (isDeterminationHit)
        {
            isSelect = false;
            selectCard.SetActive(false);
            selectCard.GetComponent<S_DeckCard>().IsSelectedByDetermination = false;
        }
    }
    #endregion
    #region 보조 함수
    public void UpdateDeckCardsState() // 저주, 덱에 없는 상태, 카드의 개수 현황 업데이트
    {
        // 덱에 없다면 불투명하게 처리하고 저주받은 카드도 체크
        foreach (GameObject go in deckCardObjects)
        {
            go.GetComponent<S_DeckCard>().UpdateDeckCardState();
        }

        // 텍스트 업데이트
        int spadeC = 0;
        foreach (GameObject go in spadeCards)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo.IsInDeck)
            {
                spadeC++;
            }
        }
        text_SpadeCount.GetComponent<TMP_Text>().text = $"{spadeC} / {spadeCards.Count}";
        int heartC = 0;
        foreach (GameObject go in heartCards)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo.IsInDeck)
            {
                heartC++;
            }
        }
        text_HeartCount.GetComponent<TMP_Text>().text = $"{heartC} / {heartCards.Count}";
        int diamondC = 0;
        foreach (GameObject go in diamondCards)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo.IsInDeck)
            {
                diamondC++;
            }
        }
        text_DiamondCount.GetComponent<TMP_Text>().text = $"{diamondC} / {diamondCards.Count}";
        int cloverC = 0;
        foreach (GameObject go in cloverCards)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo.IsInDeck)
            {
                cloverC++;
            }
        }
        text_CloverCount.GetComponent<TMP_Text>().text = $"{cloverC} / {cloverCards.Count}";

        text_ExclusionCount.GetComponent<TMP_Text>().text = $"제외된 카드 개수 : {S_PlayerCard.Instance.GetPreExclusionTotalCards().Count}";
        text_StackCount.GetComponent<TMP_Text>().text = $"스택에 있는 카드 개수 : {S_PlayerCard.Instance.GetPreStackCards().Count}";
        text_DeckCount.GetComponent<TMP_Text>().text = $"덱에 있는 카드 개수 : {S_PlayerCard.Instance.GetPreDeckCards().Count}";
    }
    // 공통되는 덱 열고 닫는 로직
    public async Task OpenDeckInfoCommonProperty(bool isDetermination, bool needBtn = true)
    {
        // 인게임 인터페이스를 숨기기(피조물, 전리품, 전투 능력치, 히트 버튼, 플레이어 이미지, 덱 정보 버튼, 스택에 정렬하는 버튼)
        S_FoeInfoSystem.Instance.DisappearCreature();
        S_SkillInfoSystem.Instance.DisappearSkill();
        S_StatInfoSystem.Instance.DisappearBattleStat();
        S_HitBtnSystem.Instance.DisappearHitBtn();
        S_PlayerInfoSystem.Instance.DisappearPlayerImage();
        S_DeckInfoSystem.Instance.DisappearViewDeckInfoBtn();
        S_StackInfoSystem.Instance.DisappearStackInfoBtn();

        // 덱 정보에서 사용할 버튼 등장
        isDeterminationHit = isDetermination;
        if (needBtn)
        {
            if (isDetermination)
            {
                panel_DeterminationHitBtnAndCancelBtnBase.SetActive(true);
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                    .OnComplete(() => panel_CloseDeckBtnBase.SetActive(false));
            }
            else
            {
                panel_CloseDeckBtnBase.SetActive(true);
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                    .OnComplete(() => panel_DeterminationHitBtnAndCancelBtnBase.SetActive(false));
            }
        }

        // 살짝 어두워지게 만드는 효과
        sprite_BlackBackgroundByDeckInfo.DOFade(0.7f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // 카메라 이동
        Camera.main.transform.DOMove(deckCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // UI 이동 대기
        await Task.Delay(Mathf.RoundToInt(S_GameFlowManager.PANEL_APPEAR_TIME * 1000));
    }
    public async Task CloseDeckInfoCommonProperty()
    {
        // 기본적인 공용 인터페이스 등장(전리품, 전투 능력치, 덱 보기 버튼, 스택에 정렬하는 버튼)
        S_SkillInfoSystem.Instance.AppearSkill();
        S_StatInfoSystem.Instance.AppearBattleStat();
        S_DeckInfoSystem.Instance.AppearViewDeckInfoBtn();
        S_StackInfoSystem.Instance.AppearStackInfoBtn();

        // 덱 버튼 퇴장
        if (isDeterminationHit)
        {
            panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
            panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                .OnComplete(() => panel_DeterminationHitBtnAndCancelBtnBase.SetActive(false));
        }
        else
        {
            panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
            panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                .OnComplete(() => panel_CloseDeckBtnBase.SetActive(false));
        }

        // 어두워진걸 다시 풀어주기
        sprite_BlackBackgroundByDeckInfo.DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        await Task.Delay(Mathf.RoundToInt(S_GameFlowManager.PANEL_APPEAR_TIME * 1000));
    }
    public List<GameObject> GetDeckCardObjects()
    {
        return deckCardObjects.ToList();
    }
    #endregion
}