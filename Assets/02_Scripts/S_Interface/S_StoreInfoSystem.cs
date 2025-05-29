using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class S_StoreInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject productPrefab;
    [SerializeField] GameObject deckCardPrefab;
    [SerializeField] GameObject optionCardPrefab;
    [SerializeField] GameObject optionLootPrefab;
    [SerializeField] GameObject readOnlyCardPrefab;
    [SerializeField] GameObject readOnlyUILootPrefab;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject storeProductsBase;
    [SerializeField] GameObject selectedProductBase;
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByStore;
    [SerializeField] GameObject optionsBase;
    [SerializeField] GameObject selectedItemBase;
    [SerializeField] GameObject selectedRemoveCardBase;

    [Header("컴포넌트")]
    GameObject panel_BuyProductOrRefreshBtnBase;
    GameObject panel_SelectItemBtnBase;
    GameObject text_NotifyStrongCreatureText;
    GameObject text_CreatureTitleText;
    GameObject text_CreatureHealthAndLimitText;
    GameObject text_CreatureAbilityText;
    GameObject text_SelectProductText;
    GameObject text_SelectItemText;
    TMP_Text text_RefreshBtn;

    [Header("상품")]
    (Vector3, S_StoreSlotEnum, bool) freeSlot = (new Vector3(-5f, 1.3f, 0), S_StoreSlotEnum.Free, false);
    (Vector3, S_StoreSlotEnum, bool) card1Slot = (new Vector3(-1.67f, 1.3f, 0), S_StoreSlotEnum.Card1, false); // (위치, 슬롯 타입, 솔드아웃 여부(솔드아웃 시 true))
    (Vector3, S_StoreSlotEnum, bool) card2Slot = (new Vector3(1.67f, 1.3f, 0), S_StoreSlotEnum.Card2, false);
    (Vector3, S_StoreSlotEnum, bool) lootSlot = (new Vector3(5f, 1.3f, 0), S_StoreSlotEnum.Loot, false);
    List<(Vector3, S_StoreSlotEnum, bool)> productsSlot = new();
    List<GameObject> storeProducts = new();
    GameObject selectedProduct;

    bool isSelectRequiredProduct;
    [HideInInspector] public S_Product CurrentBuyingProduct;
    int refreshGold = 2;
    int refreshGoldIncreaseValue = 1;

    [Header("아이템 옵션")]
    bool isSelectRequiredItems;
    List<GameObject> itemOptionsList = new();
    List<GameObject> selectedItemList = new();
    Vector3 optionStartPoint = new Vector3(-4.5f, 0, 0);
    Vector3 optionEndPoint = new Vector3(4.5f, 0, 0);
    Vector3 selectedStartPoint = new Vector3(-1f, 0, 0);
    Vector3 selectedEndPoint = new Vector3(1f, 0, 0);
    float zValue = -0.2f;
    Vector3 exclusionPoint = new Vector3(0, 11f, 0);
    float exclusionScaleAmount = 1.2f;
    public const float EXCLUSION_TIME = 0.4f;

    Vector2 inStoreTextPos = new Vector2(0, 225);
    Vector2 inDeckTextPos = new Vector2(0, -105);
    Vector2 uICardPos = new Vector2(0, 50);

    [Header("버튼 UI")]
    Vector2 btnHidePos = new Vector2(0, -150);
    Vector2 btnOriginPos = new Vector2(0, 55);

    // 싱글턴
    static S_StoreInfoSystem instance;
    public static S_StoreInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        panel_BuyProductOrRefreshBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_BuyProductOrRefreshBtnBase")).gameObject;
        panel_SelectItemBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_SelectItemBtnBase")).gameObject;

        text_NotifyStrongCreatureText = Array.Find(transforms, c => c.gameObject.name.Equals("Text_NotifyStrongCreatureText")).gameObject;
        text_NotifyStrongCreatureText.GetComponent<TMP_Text>().raycastTarget = false;
        text_CreatureTitleText = Array.Find(transforms, c => c.gameObject.name.Equals("Text_CreatureTitleText")).gameObject;
        text_CreatureTitleText.GetComponent<TMP_Text>().raycastTarget = false;
        text_CreatureHealthAndLimitText = Array.Find(transforms, c => c.gameObject.name.Equals("Text_CreatureHealthAndLimitText")).gameObject;
        text_CreatureHealthAndLimitText.GetComponent<TMP_Text>().raycastTarget = false;
        text_CreatureAbilityText = Array.Find(transforms, c => c.gameObject.name.Equals("Text_CreatureAbilityText")).gameObject;
        text_CreatureAbilityText.GetComponent<TMP_Text>().raycastTarget = false;
        text_SelectProductText = Array.Find(transforms, c => c.gameObject.name.Equals("Text_SelectProductText")).gameObject;
        text_SelectProductText.GetComponent<TMP_Text>().raycastTarget = false;
        text_SelectItemText = Array.Find(transforms, c => c.gameObject.name.Equals("Text_SelectItemText")).gameObject;
        text_SelectItemText.GetComponent<TMP_Text>().raycastTarget = false;

        text_RefreshBtn = Array.Find(texts, c => c.gameObject.name.Equals("Text_RefreshBtn"));

        // 상품 목록 초기화
        productsSlot = new() { freeSlot, card1Slot, card2Slot, lootSlot };

        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 위치 초기화
        InitPos();
    }

    public void StartStore()
    {
        // 초기 세팅
        SetTextByStartStore();

        AppearUIByBuyStoreProduct();
        DisappearUIBySelectItem();
        AppearStoreTextInStore();
        DisappearSelectCardOrLootTextInStore();
        DisappearBlackBackground();

        // 새로고침 골드 초기화
        refreshGold = 2;
        text_RefreshBtn.text = $"새로고침\n{refreshGold} 골드";

        // 상품 생성
        for (int i = 0; i < productsSlot.Count; i++)
        {
            productsSlot[i] = (productsSlot[i].Item1, productsSlot[i].Item2, false);
        }
        GenerateNewProducts();
    }
    public void SetTextByStartStore()
    {
        if (S_GameFlowManager.Instance.CurrentTrial % 3 == 1)
        {
            text_NotifyStrongCreatureText.GetComponent<TMP_Text>().text = "강력한 피조물 등장까지 1마리 남았습니다.";
        }
        else if (S_GameFlowManager.Instance.CurrentTrial % 3 == 2)
        {
            text_NotifyStrongCreatureText.GetComponent<TMP_Text>().text = "다음 시련에 강력한 피조물이 등장합니다! 부디 만반의 준비를...";
        }
        else if (S_GameFlowManager.Instance.CurrentTrial % 3 == 0)
        {
            text_NotifyStrongCreatureText.GetComponent<TMP_Text>().text = "강력한 피조물 등장까지 2마리 남았습니다.";
        }

        text_CreatureHealthAndLimitText.GetComponent<TMP_Text>().text = $"체력 : {S_FoeInfoSystem.Instance.CurrentFoe.MaxHealth}\n한계 : {S_FoeInfoSystem.Instance.CurrentFoe.OriginLimit}";

        text_CreatureAbilityText.GetComponent<TMP_Text>().text = $"능력 : {S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.AbilityDescription}";
    }

    void InitPos()
    {
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().anchoredPosition = btnHidePos;
        panel_SelectItemBtnBase.GetComponent<RectTransform>().anchoredPosition = btnHidePos;
        text_SelectProductText.GetComponent<TMP_Text>().DOFade(0f, 0f);
        text_SelectProductText.GetComponent<TMP_Text>().raycastTarget = false;
        text_NotifyStrongCreatureText.GetComponent<TMP_Text>().DOFade(0f, 0f);
        text_NotifyStrongCreatureText.GetComponent<TMP_Text>().raycastTarget = false;
    }
    public void AppearUIByBuyStoreProduct()
    {
        panel_BuyProductOrRefreshBtnBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearUIByBuyStoreProduct()
    {
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_BuyProductOrRefreshBtnBase.SetActive(false));
    }
    public void AppearUIBySelectItem()
    {
        panel_SelectItemBtnBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_SelectItemBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_SelectItemBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearUIBySelectItem()
    {
        sprite_BlackBackgroundByStore.DOKill();
        panel_SelectItemBtnBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        sprite_BlackBackgroundByStore.DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        panel_SelectItemBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_SelectItemBtnBase.SetActive(false));
    }
    public void AppearStoreTextInStore()
    {
        text_NotifyStrongCreatureText.GetComponent<TMP_Text>().DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        text_CreatureTitleText.GetComponent<TMP_Text>().DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        text_CreatureHealthAndLimitText.GetComponent<TMP_Text>().DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        text_CreatureAbilityText.GetComponent<TMP_Text>().DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        text_SelectProductText.GetComponent<TMP_Text>().DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearStoreTextInStore()
    {
        text_NotifyStrongCreatureText.GetComponent<TMP_Text>().DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        text_CreatureTitleText.GetComponent<TMP_Text>().DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        text_CreatureHealthAndLimitText.GetComponent<TMP_Text>().DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        text_CreatureAbilityText.GetComponent<TMP_Text>().DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        text_SelectProductText.GetComponent<TMP_Text>().DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

    }
    public void AppearSelectCardOrLootTextInStore()
    {
        int selectCount = CurrentBuyingProduct.SelectCount;

        if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Card)
        {
            text_SelectItemText.GetComponent<TMP_Text>().text = $"카드 {selectCount}장을 선택해주세요.";
        }
        else
        {
            text_SelectItemText.GetComponent<TMP_Text>().text = $"전리품 {selectCount}개를 선택해주세요.";
        }

        if (CurrentBuyingProduct.Modify == S_ProductModifyEnum.Remove)
        {
            text_SelectItemText.GetComponent<RectTransform>().DOAnchorPos(inDeckTextPos, 0f);
        }
        else
        {
            text_SelectItemText.GetComponent<RectTransform>().DOAnchorPos(inStoreTextPos, 0f);
        }

        text_SelectItemText.GetComponent<TMP_Text>().DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearSelectCardOrLootTextInStore()
    {
        text_SelectItemText.GetComponent<TMP_Text>().DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void AppearBlackBackground()
    {
        sprite_BlackBackgroundByStore.DOKill();
        sprite_BlackBackgroundByStore.DOFade(0.8f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearBlackBackground()
    {
        sprite_BlackBackgroundByStore.DOKill();
        sprite_BlackBackgroundByStore.DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }


    // 상품 관련
    public void ClickBuy() // 상품 구매 버튼 함수
    {
        if (isSelectRequiredProduct)
        {
            // 전리품이 가득 찼다면 로그 띄우고 리턴
            if (selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Type == S_ProductTypeEnum.Loot &&
                selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Modify == S_ProductModifyEnum.Add &&
                !S_PlayerSkill.Instance.CanAddSkill())
            {
                S_InGameUISystem.Instance.CreateLog("더 이상 전리품을 구매할 수 없습니다!");
                return;
            }

            // 구매할만큼 골드가 있다면 구매
            if (S_PlayerStat.Instance.CurrentGold >= selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Price)
            {
                // 골드 사용
                S_PlayerStat.Instance.AddOrSubtractGold(-selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Price);

                // 상품 숨기기
                storeProductsBase.SetActive(false);

                // 현재 구매 중인 상품 설정
                CurrentBuyingProduct = selectedProduct.GetComponent<S_ProductObject>().ThisProduct;

                // 선택된 상품의 구매를 진행
                selectedProduct.GetComponent<S_ProductObject>().ThisProduct.BuyProduct();
          

                // 해당 슬롯을 솔드아웃 처리하기
                for (int i = 0; i < productsSlot.Count; i++)
                {
                    if (productsSlot[i].Item2 == selectedProduct.GetComponent<S_ProductObject>().SlotInfo)
                    {
                        productsSlot[i] = (productsSlot[i].Item1, productsSlot[i].Item2, true);

                        foreach (GameObject go in storeProducts)
                        {
                            if (go.GetComponent<S_ProductObject>().SlotInfo == selectedProduct.GetComponent<S_ProductObject>().SlotInfo)
                            {
                                go.GetComponent<S_ProductObject>().SetSoldOut();
                            }
                        }
                    }
                }

                // 파괴
                Destroy(selectedProduct);
                selectedProduct = null;

                // 필요한 걸 다 선택했는지 여부 초기화
                isSelectRequiredProduct = false;
            }
            else // 골드가 없다면
            {
                S_InGameUISystem.Instance.CreateLog("골드가 부족합니다.");
            }
        }
        else // 필요한 걸 다 선택하지 않았다면
        {
            S_InGameUISystem.Instance.CreateLog("상품을 선택해주세요!");
        }
    }
    void GenerateNewProducts() // 상품 생성
    {
        foreach (var slot in productsSlot)
        {
            S_Product product = null;
            switch (slot.Item2)
            {
                case S_StoreSlotEnum.Free:
                    product = S_ProductList.PickFreeProduct(S_GameFlowManager.Instance.CurrentTrial);
                    break;
                case S_StoreSlotEnum.Card1:
                    product = S_ProductList.PickRandomProductByType(S_ProductTypeEnum.Card);
                    break;
                case S_StoreSlotEnum.Card2:
                    product = S_ProductList.PickRandomProductByType(S_ProductTypeEnum.Card);
                    break;
                case S_StoreSlotEnum.Loot:
                    product = S_ProductList.PickRandomProductByType(S_ProductTypeEnum.Loot);
                    break;
            }

            GameObject go = Instantiate(productPrefab, storeProductsBase.transform);
            storeProducts.Add(go);
            go.transform.localPosition = slot.Item1;
            if (!slot.Item3)
            {
                go.GetComponent<S_ProductObject>().SetProductInfo(product, false, slot.Item2);
            }
            else
            {
                go.GetComponent<S_ProductObject>().SetSoldOut();
            }
        }
    }
    public void SelectProduct(S_Product product, S_StoreSlotEnum slot) // ProductInfo에서 호출
    {
        if (selectedProduct != null)
        {
            Destroy(selectedProduct);
            selectedProduct = null;
        }

        selectedProduct = Instantiate(productPrefab, selectedProductBase.transform);
        selectedProduct.transform.localPosition = Vector3.zero;
        selectedProduct.GetComponent<S_ProductObject>().SetProductInfo(product, true, slot);

        if (selectedProduct != null)
        {
            isSelectRequiredProduct = true;
        }
    }
    public void CancelSelectedProduct() // ProductInfo에서 호출. 상품 선택 취소
    {
        Destroy(selectedProduct);
        selectedProduct = null;

        if (selectedProduct == null)
        {
            isSelectRequiredProduct = false;
        }
    }


    // 상품 중 추가할 카드 선택 관련
    public void StartSelectOptionCards(List<S_Card> cards) // 카드를 추가하는 상품 함수
    {
        // 버튼 변경
        DisappearUIByBuyStoreProduct();
        AppearUIBySelectItem();
        DisappearStoreTextInStore();
        AppearSelectCardOrLootTextInStore();
        AppearBlackBackground();

        // 상품 카드 생성
        foreach (S_Card card in cards)
        {
            GenerateOptionCard(card, optionsBase.transform, itemOptionsList);
        }

        // 정렬
        AlignmentProductCard(itemOptionsList, true); // true로 해야 optionCards용으로 정렬된다.
    }
    public void GenerateOptionCard(S_Card card, Transform parent, List<GameObject> list) // 덱에 추가할 카드 선택이나 제외할 카드 선택 시에도 사용 예정
    {
        // 옵션 카드 생성
        GameObject go = Instantiate(optionCardPrefab);
        go.GetComponent<S_OptionCard>().SetCardInfo(card);
        go.transform.SetParent(parent, true);
        go.transform.localPosition = Vector3.zero;

        // 리스트에 추가(추후 관리용)
        list.Add(go);
    }
    void AlignmentProductCard(List<GameObject> cards, bool isOption) // 카드를 정렬하기
    {
        List<PRS> originCardPRS = SetCardPos(cards.Count, isOption);

        for (int i = 0; i < cards.Count; i++)
        {
            // 카드 위치 설정
            cards[i].GetComponent<S_DeckCard>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            cards[i].GetComponent<S_DeckCard>().OriginOrder = (i + 1) * 10;
            cards[i].GetComponent<S_DeckCard>().SetOrder(cards[i].GetComponent<S_DeckCard>().OriginOrder);

            // 카드의 위치 설정
            cards[i].GetComponent<Transform>().localPosition = cards[i].GetComponent<S_DeckCard>().OriginPRS.Pos;
            cards[i].GetComponent<Transform>().localScale = cards[i].GetComponent<S_DeckCard>().OriginPRS.Scale;
        }
    }
    List<PRS> SetCardPos(int cardCount, bool isOption) // 카드 위치 설정하는 메서드
    {
        float[] lerps = new float[cardCount];
        List<PRS> results = new List<PRS>(cardCount);

        if (cardCount == 1)
        {
            lerps[0] = 0.5f;
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
            Vector3 pos;
            if (isOption)
            {
                pos = Vector3.Lerp(optionStartPoint, optionEndPoint, lerps[i]);
            }
            else
            {
                pos = Vector3.Lerp(selectedStartPoint, selectedEndPoint, lerps[i]);
            }
            pos = new Vector3(pos.x, pos.y, i * zValue);
            Vector3 rot = Vector3.zero;
            Vector3 scale = new Vector3(1, 1, 1);
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void SelectCardByOption(S_Card card) // 덱에 추가할 카드 선택 시 
    {
        // 선택한 카드 중에 card가 있는지 검사
        bool same = false;
        foreach (GameObject go in selectedItemList)
        {
            if (go.GetComponent<S_OptionCard>().CardInfo == card)
            {
                same = true;
                break;
            }
            else
            {
                same = false;
            }
        }

        // card가 없었다면 고른 카드 개수에 따른 처리
        if (!same)
        {
            if (CurrentBuyingProduct.SelectCount > selectedItemList.Count)
            {
                GenerateOptionCard(card, selectedItemBase.transform, selectedItemList);
            }
            else if (CurrentBuyingProduct.SelectCount == selectedItemList.Count) // 골라야하는 개수가 같다면 이전에 선택한 것을 제거하고 추가
            {
                Destroy(selectedItemList[0]);
                selectedItemList.RemoveAt(0);

                GenerateOptionCard(card, selectedItemBase.transform, selectedItemList);
            }
        }

        // 선택된 옵션 카드임을 체크
        foreach (GameObject go in selectedItemList)
        {
            go.GetComponent<S_OptionCard>().IsSelectedOptionCard = true;
        }

        // 정렬
        AlignmentProductCard(selectedItemList, false);

        // 골라야하는걸 다 골랐는지 체크
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }
    public void CancelSelectCardByStore(S_Card card)
    {
        GameObject exceptedCard = null;

        // 선택 취소한 카드를 selectedCards에서 찾기
        foreach (GameObject go in selectedItemList)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo == card)
            {
                exceptedCard = go;
            }
        }

        // 찾은 카드를 리스트에서 빼고 디스트로이
        selectedItemList.Remove(exceptedCard);
        Destroy(exceptedCard);

        // 정렬
        AlignmentProductCard(selectedItemList, false);

        // 선택할 수 있는지 여부 활성화
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }


    // 상품 중 카드 제외 관련
    public async void StartSelectCardInDeck()
    {
        // 덱 정보 UI 설정
        await S_DeckInfoSystem.Instance.OpenDeckInfoBySelectRemoveCardByStore();
    }
    public void SelectCardByRemoveCard(S_Card card)
    {
        bool same = false;
        foreach (GameObject go in selectedItemList)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo == card)
            {
                same = true;
                break;
            }
            else
            {
                same = false;
            }
        }

        if (!same)
        {
            if (CurrentBuyingProduct.SelectCount > selectedItemList.Count) // 골라야하는 개수가 아직 남았다면 선택한 카드를 그냥 추가
            {
                GenerateOptionCard(card, selectedRemoveCardBase.transform, selectedItemList);
            }
            else if (CurrentBuyingProduct.SelectCount == selectedItemList.Count) // 골라야하는 개수가 같다면 이전에 선택한 것을 제거하고 추가
            {
                Destroy(selectedItemList[0]);
                selectedItemList.RemoveAt(0);

                GenerateOptionCard(card, selectedRemoveCardBase.transform, selectedItemList);
            }
        }

        // 선택된 카드로 변경
        foreach (GameObject go in selectedItemList)
        {
            go.GetComponent<S_DeckCard>().IsSelectedByRemoveCard = true;
        }

        // 정렬
        AlignmentProductCard(selectedItemList, false);

        // 선택할 수 있는지 여부 활성화
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }


    // 상품 중 전리품 선택 관련
    public void StartSelectOptionLoots(List<S_Skill> loots)
    {
        // 버튼 변경
        DisappearUIByBuyStoreProduct();
        AppearUIBySelectItem();
        DisappearStoreTextInStore();
        AppearSelectCardOrLootTextInStore();
        AppearBlackBackground();

        // 상품 전리품 생성
        foreach (S_Skill loot in loots)
        {
            if (loot.Condition == S_ActivationConditionEnum.Loot_Growth)
            {
                loot.UpdateActivatedCountByGrowthCondition();
            }
            else if (loot.Condition == S_ActivationConditionEnum.Overflow)
            {
                loot.UpdateActivatedCountByGrowthCondition();
            }

            GenerateOptionLoot(loot, optionsBase.transform, itemOptionsList);
        }

        // 정렬
        AlignmentProductLoot(itemOptionsList, true); // true로 해야 optionCards용으로 정렬된다.
    }
    public void GenerateOptionLoot(S_Skill loot, Transform parent, List<GameObject> list) // 덱에 추가할 카드 선택이나 제외할 카드 선택 시에도 사용 예정
    {
        // 카드 생성
        GameObject go = Instantiate(optionLootPrefab);
        go.GetComponent<S_ReadOnlyLoot>().SetLootInfo(loot);
        go.transform.SetParent(parent, true);
        go.transform.localPosition = Vector3.zero;

        // 리스트에 추가(추후 관리용)
        list.Add(go);
    }
    void AlignmentProductLoot(List<GameObject> cards, bool isOption) // 카드를 정렬하기
    {
        List<PRS> originCardPRS = SetLootPos(cards.Count, isOption);

        for (int i = 0; i < cards.Count; i++)
        {
            // 카드 위치 설정
            cards[i].GetComponent<S_ReadOnlyLoot>().OriginPRS = originCardPRS[i];

            // 카드의 위치 설정
            cards[i].GetComponent<Transform>().localPosition = cards[i].GetComponent<S_ReadOnlyLoot>().OriginPRS.Pos;
            cards[i].GetComponent<Transform>().localScale = cards[i].GetComponent<S_ReadOnlyLoot>().OriginPRS.Scale;
        }
    }
    List<PRS> SetLootPos(int count, bool isOption) // 카드 위치 설정하는 메서드
    {
        float[] lerps = new float[count];
        List<PRS> results = new List<PRS>(count);

        if (count == 1)
        {
            lerps[0] = 0.5f;
        }
        else if (count > 1)
        {
            float interval = 1f / (count - 1);
            for (int i = 0; i < count; i++)
            {
                lerps[i] = interval * i;
            }
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 pos;
            if (isOption)
            {
                pos = Vector3.Lerp(optionStartPoint, optionEndPoint, lerps[i]);
            }
            else
            {
                pos = Vector3.Lerp(selectedStartPoint, selectedEndPoint, lerps[i]);
            }
            pos = new Vector3(pos.x, pos.y, i * zValue);
            Vector3 rot = Vector3.zero;
            Vector3 scale = new Vector3(1, 1, 1);
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void SelectSkillByOption(S_Skill loot) // 덱에 추가할 카드 선택 시
    {
        bool same = false;
        foreach (GameObject go in selectedItemList)
        {
            if (go.GetComponent<S_ReadOnlyLoot>().LootInfo == loot)
            {
                same = true;
                break;
            }
            else
            {
                same = false;
            }
        }

        if (!same)
        {
            if (CurrentBuyingProduct.SelectCount > selectedItemList.Count) // 골라야하는 개수가 아직 남았다면 선택한 카드를 그냥 추가
            {
                GenerateOptionLoot(loot, selectedItemBase.transform, selectedItemList);
            }
            else if (CurrentBuyingProduct.SelectCount == selectedItemList.Count) // 골라야하는 개수가 같다면 이전에 선택한 것을 제거하고 추가
            {
                Destroy(selectedItemList[0]);
                selectedItemList.RemoveAt(0);

                GenerateOptionLoot(loot, selectedItemBase.transform, selectedItemList);
            }
        }

        // 선택된 전리품은 선택됨 처리
        foreach (GameObject go in selectedItemList)
        {
            go.GetComponent<S_ReadOnlyLoot>().IsSelectedOption = true;
        }

        // 정렬
        AlignmentProductLoot(selectedItemList, false);

        // 선택할 수 있는지 여부 활성화
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }
    public void CancelSelectLootByOption(S_Skill loot)
    {
        GameObject excepted = null;

        // 선택 취소한 전리품을 selected에서 찾기
        foreach (GameObject go in selectedItemList)
        {
            if (go.GetComponent<S_ReadOnlyLoot>().LootInfo.Equals(loot))
            {
                excepted = go;
            }
        }

        // 찾은 전리품을 리스트에서 빼고 디스트로이
        selectedItemList.Remove(excepted);
        Destroy(excepted);

        // 정렬
        AlignmentProductLoot(selectedItemList, false);

        // 선택할 수 있는지 여부 활성화
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }


    // 상품 중 전리품 제외 관련
    public void StartRemoveLoot()
    {
        // 버튼 변경
        DisappearUIByBuyStoreProduct();
        AppearUIBySelectItem();
        DisappearStoreTextInStore();
        AppearSelectCardOrLootTextInStore();
        AppearBlackBackground();

        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.StoreByRemove;
    }


    // 획득한 카드 보여주는 UI
    public void GenerateUICard(S_Card card)
    {
        GameObject go = Instantiate(readOnlyCardPrefab, transform);
        go.GetComponent<S_UICard>().SetCardInfo(card);
        go.GetComponent<S_UICard>().GetCardVFX();
    }
    public void GenerateUILoot(S_Skill loot, bool isRemove = false)
    {
        GameObject go = Instantiate(readOnlyUILootPrefab, transform);
        go.GetComponent<S_ReadOnlyUILoot>().SetLootInfo(loot);

        if (isRemove)
        {
            go.GetComponent<S_ReadOnlyUILoot>().RemoveLootVFX();
        }
        else
        {
            go.GetComponent<S_ReadOnlyUILoot>().GetLootVFX();
        }
    }


    // 버튼 함수
    public void ClickRefresh()
    {
        if (S_PlayerStat.Instance.CurrentGold >= refreshGold)
        {
            // 골드 사용
            S_PlayerStat.Instance.AddOrSubtractGold(-refreshGold);

            // 새로고침 비용 증가
            refreshGold += refreshGoldIncreaseValue;
            text_RefreshBtn.text = $"새로고침\n{refreshGold} 골드";

            // 기존에 있던 상품 제거
            foreach (GameObject go in storeProducts)
            {
                Destroy(go);
            }
            storeProducts.Clear();

            // 각종 비활성화
            CurrentBuyingProduct = null;
            isSelectRequiredProduct = false;

            // 선택한 상품이 있다면 없애기
            if (selectedProduct != null)
            {
                Destroy(selectedProduct);
                selectedProduct = null;
            }

            // 새로운 상품 등록
            GenerateNewProducts();
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("골드가 부족합니다.");
        }
    }
    public void ClickExit()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        DisappearUIByBuyStoreProduct();
        DisappearStoreTextInStore();

        for (int i = 0; i < productsSlot.Count; i++)
        {
            productsSlot[i] = (productsSlot[i].Item1, productsSlot[i].Item2, false);
        }

        foreach (Transform go in storeProductsBase.transform)
        {
            Destroy(go.gameObject);
        }

        foreach (Transform go in selectedProductBase.transform)
        {
            Destroy(go.gameObject);
        }
        foreach (Transform go in optionsBase.transform)
        {
            Destroy(go.gameObject);
        }
        foreach (Transform go in selectedItemBase.transform)
        {
            Destroy(go.gameObject);
        }
        foreach (Transform go in selectedRemoveCardBase.transform)
        {
            Destroy(go.gameObject);
        }
        storeProducts.Clear();
        selectedProduct = null;


        S_GameFlowManager.Instance.StartTrial();
    }
    public async void ClickSelect()
    {
        // 필수 선택을 모두 했다면
        if (isSelectRequiredItems)
        {
            if (CurrentBuyingProduct.Modify == S_ProductModifyEnum.Add) // 전리품이나 카드를 추가하는 경우
            {
                if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Card) // 카드인 경우
                {
                    foreach (GameObject go in selectedItemList)
                    {
                        S_PlayerCard.Instance.AddCard(go.GetComponent<S_DeckCard>().CardInfo);
                        GenerateUICard(go.GetComponent<S_DeckCard>().CardInfo);
                        Destroy(go);
                    }
                }
                else if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Loot) // 전리품인 경우
                {
                    foreach (GameObject go in selectedItemList)
                    {
                        S_PlayerSkill.Instance.AddSkill(go.GetComponent<S_ReadOnlyLoot>().LootInfo);
                        GenerateUILoot(go.GetComponent<S_ReadOnlyLoot>().LootInfo);
                        Destroy(go);
                    }

                    S_SkillInfoSystem.Instance.UpdateSkillObject();
                }

                // 선택한 목록 비우기
                selectedItemList.Clear();

                // 생성했던 아이템 옵션 목록도 비우기
                foreach (GameObject go in itemOptionsList)
                {
                    Destroy(go);
                }
                itemOptionsList.Clear();

                // UI 되돌리기
                AppearUIByBuyStoreProduct();
                DisappearUIBySelectItem();
                AppearStoreTextInStore();
                DisappearSelectCardOrLootTextInStore();
                DisappearBlackBackground();

                // 상품 다시 활성화
                storeProductsBase.SetActive(true);
            }
            else if (CurrentBuyingProduct.Modify == S_ProductModifyEnum.Remove) // 전리품이나 카드를 제거하는 경우
            {
                if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Card) // 카드인 경우
                {
                    // 상품 키에 따라 다르게 처리하기
                    switch (CurrentBuyingProduct.Key)
                    {
                        case "P_DreamingDetermination":
                            // 같은 문양의 카드 생성
                            S_Card addCard1 = S_CardManager.Instance.GenerateRandomCard(-1, selectedItemList[0].GetComponent<S_DeckCard>().CardInfo.Suit);

                            // 카드 추가하고 추가된 카드 보여주는 VFX까지
                            S_PlayerCard.Instance.AddCard(addCard1);
                            GenerateUICard(addCard1);

                            // selected 초기화
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_RebelliousDetermination":
                            // 같은 숫자의 카드 생성
                            S_Card addCard2 = S_CardManager.Instance.GenerateRandomCard(selectedItemList[0].GetComponent<S_DeckCard>().CardInfo.Number);

                            // 카드 추가하고 추가된 카드 보여주는 VFX까지
                            S_PlayerCard.Instance.AddCard(addCard2);
                            GenerateUICard(addCard2);

                            // selected 초기화
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_ImmortalDetermination":
                            // 같은 카드 생성
                            S_Card addCard3 = S_CardManager.Instance.GenerateCopyCard(selectedItemList[0].GetComponent<S_DeckCard>().CardInfo);

                            // 카드 추가하고 추가된 카드 보여주는 VFX까지
                            S_PlayerCard.Instance.AddCard(addCard3);
                            GenerateUICard(addCard3);

                            // selected 초기화
                            foreach (GameObject go in selectedItemList)
                            {
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_TwistedDetermination":
                            // 정해진 숫자와 문양의 카드 생성
                            S_Card addCard4 = S_CardManager.Instance.GenerateRandomCard(selectedItemList[0].GetComponent<S_DeckCard>().CardInfo.Number, selectedItemList[1].GetComponent<S_DeckCard>().CardInfo.Suit);

                            // 카드 추가하고 추가된 카드 보여주는 VFX까지
                            S_PlayerCard.Instance.AddCard(addCard4);
                            GenerateUICard(addCard4);

                            // selected 초기화
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_BurningDetermination":
                            // selected 초기화
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_CreatureLost":
                            // 같은 문양의 카드 생성
                            S_Card addCard5 = S_CardManager.Instance.GenerateRandomCard();

                            // 카드 추가하고 추가된 카드 보여주는 VFX까지
                            S_PlayerCard.Instance.AddCard(addCard5);
                            GenerateUICard(addCard5);

                            // selected 초기화
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_CreatureThickThread":
                            S_Card addCard6 = null;
                            switch (selectedItemList[0].GetComponent<S_DeckCard>().CardInfo.CardEffect.Grade)
                            {
                                case S_CardEffectGradeEnum.Normal:
                                    addCard6 = S_CardManager.Instance.GenerateGradeCard(S_CardEffectGradeEnum.Superior);
                                    break;
                                case S_CardEffectGradeEnum.Superior:
                                    addCard6 = S_CardManager.Instance.GenerateGradeCard(S_CardEffectGradeEnum.Rare);
                                    break;
                                case S_CardEffectGradeEnum.Rare:
                                    addCard6 = S_CardManager.Instance.GenerateGradeCard(S_CardEffectGradeEnum.Mythic);
                                    break;
                                case S_CardEffectGradeEnum.Mythic:
                                    addCard6 = S_CardManager.Instance.GenerateMythicCard();
                                    break;
                            }

                            // 카드 추가하고 추가된 카드 보여주는 VFX까지
                            S_PlayerCard.Instance.AddCard(addCard6);
                            GenerateUICard(addCard6);

                            // selected 초기화
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                    }

                    // 덱 정보 UI 설정
                    await S_DeckInfoSystem.Instance.ClostDeckInfoByEndSelectRemoveCardByStore();
                }
                else if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Loot) // 전리품인 경우
                {
                    foreach (GameObject go in selectedItemList)
                    {
                        S_PlayerSkill.Instance.RemoveSkill(go.GetComponent<S_ReadOnlyLoot>().LootInfo);
                        GenerateUILoot(go.GetComponent<S_ReadOnlyLoot>().LootInfo, true);
                        Destroy(go);
                    }

                    selectedItemList.Clear();

                    // UI 되돌리기
                    AppearUIByBuyStoreProduct();
                    DisappearUIBySelectItem();
                    AppearStoreTextInStore();
                    DisappearSelectCardOrLootTextInStore();
                    DisappearBlackBackground();
                }
            }

            // 상품 다시 활성화
            storeProductsBase.SetActive(true);

            // 아이템 옵션 선택에 관한 변수 초기화
            CurrentBuyingProduct = null;
            isSelectRequiredItems = false;
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("카드 혹은 전리품을 선택해주세요!");
        }
    }
}

public enum S_StoreSlotEnum
{
    Free,
    Card1,
    Card2,
    Loot,
}