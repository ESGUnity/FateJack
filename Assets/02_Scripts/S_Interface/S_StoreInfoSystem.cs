using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class S_StoreInfoSystem : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject productPrefab;
    [SerializeField] GameObject deckCardPrefab;
    [SerializeField] GameObject optionCardPrefab;
    [SerializeField] GameObject optionLootPrefab;
    [SerializeField] GameObject readOnlyCardPrefab;
    [SerializeField] GameObject readOnlyUILootPrefab;

    [Header("�� ������Ʈ")]
    [SerializeField] GameObject storeProductsBase;
    [SerializeField] GameObject selectedProductBase;
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByStore;
    [SerializeField] GameObject optionsBase;
    [SerializeField] GameObject selectedItemBase;
    [SerializeField] GameObject selectedRemoveCardBase;

    [Header("������Ʈ")]
    GameObject panel_BuyProductOrRefreshBtnBase;
    GameObject panel_SelectItemBtnBase;
    GameObject text_NotifyStrongCreatureText;
    GameObject text_CreatureTitleText;
    GameObject text_CreatureHealthAndLimitText;
    GameObject text_CreatureAbilityText;
    GameObject text_SelectProductText;
    GameObject text_SelectItemText;
    TMP_Text text_RefreshBtn;

    [Header("��ǰ")]
    (Vector3, S_StoreSlotEnum, bool) freeSlot = (new Vector3(-5f, 1.3f, 0), S_StoreSlotEnum.Free, false);
    (Vector3, S_StoreSlotEnum, bool) card1Slot = (new Vector3(-1.67f, 1.3f, 0), S_StoreSlotEnum.Card1, false); // (��ġ, ���� Ÿ��, �ֵ�ƿ� ����(�ֵ�ƿ� �� true))
    (Vector3, S_StoreSlotEnum, bool) card2Slot = (new Vector3(1.67f, 1.3f, 0), S_StoreSlotEnum.Card2, false);
    (Vector3, S_StoreSlotEnum, bool) lootSlot = (new Vector3(5f, 1.3f, 0), S_StoreSlotEnum.Loot, false);
    List<(Vector3, S_StoreSlotEnum, bool)> productsSlot = new();
    List<GameObject> storeProducts = new();
    GameObject selectedProduct;

    bool isSelectRequiredProduct;
    [HideInInspector] public S_Product CurrentBuyingProduct;
    int refreshGold = 2;
    int refreshGoldIncreaseValue = 1;

    [Header("������ �ɼ�")]
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

    [Header("��ư UI")]
    Vector2 btnHidePos = new Vector2(0, -150);
    Vector2 btnOriginPos = new Vector2(0, 55);

    // �̱���
    static S_StoreInfoSystem instance;
    public static S_StoreInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
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

        // ��ǰ ��� �ʱ�ȭ
        productsSlot = new() { freeSlot, card1Slot, card2Slot, lootSlot };

        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // ��ġ �ʱ�ȭ
        InitPos();
    }

    public void StartStore()
    {
        // �ʱ� ����
        SetTextByStartStore();

        AppearUIByBuyStoreProduct();
        DisappearUIBySelectItem();
        AppearStoreTextInStore();
        DisappearSelectCardOrLootTextInStore();
        DisappearBlackBackground();

        // ���ΰ�ħ ��� �ʱ�ȭ
        refreshGold = 2;
        text_RefreshBtn.text = $"���ΰ�ħ\n{refreshGold} ���";

        // ��ǰ ����
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
            text_NotifyStrongCreatureText.GetComponent<TMP_Text>().text = "������ ������ ������� 1���� ���ҽ��ϴ�.";
        }
        else if (S_GameFlowManager.Instance.CurrentTrial % 3 == 2)
        {
            text_NotifyStrongCreatureText.GetComponent<TMP_Text>().text = "���� �÷ÿ� ������ �������� �����մϴ�! �ε� ������ �غ�...";
        }
        else if (S_GameFlowManager.Instance.CurrentTrial % 3 == 0)
        {
            text_NotifyStrongCreatureText.GetComponent<TMP_Text>().text = "������ ������ ������� 2���� ���ҽ��ϴ�.";
        }

        text_CreatureHealthAndLimitText.GetComponent<TMP_Text>().text = $"ü�� : {S_FoeInfoSystem.Instance.CurrentFoe.MaxHealth}\n�Ѱ� : {S_FoeInfoSystem.Instance.CurrentFoe.OriginLimit}";

        text_CreatureAbilityText.GetComponent<TMP_Text>().text = $"�ɷ� : {S_FoeInfoSystem.Instance.CurrentFoe.FoeInfo.AbilityDescription}";
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

        // ��Ʈ������ ���� �ִϸ��̼� �ֱ�
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearUIByBuyStoreProduct()
    {
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_BuyProductOrRefreshBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_BuyProductOrRefreshBtnBase.SetActive(false));
    }
    public void AppearUIBySelectItem()
    {
        panel_SelectItemBtnBase.SetActive(true);

        // ��Ʈ������ ���� �ִϸ��̼� �ֱ�
        panel_SelectItemBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_SelectItemBtnBase.GetComponent<RectTransform>().DOAnchorPos(btnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearUIBySelectItem()
    {
        sprite_BlackBackgroundByStore.DOKill();
        panel_SelectItemBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
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
            text_SelectItemText.GetComponent<TMP_Text>().text = $"ī�� {selectCount}���� �������ּ���.";
        }
        else
        {
            text_SelectItemText.GetComponent<TMP_Text>().text = $"����ǰ {selectCount}���� �������ּ���.";
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


    // ��ǰ ����
    public void ClickBuy() // ��ǰ ���� ��ư �Լ�
    {
        if (isSelectRequiredProduct)
        {
            // ����ǰ�� ���� á�ٸ� �α� ���� ����
            if (selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Type == S_ProductTypeEnum.Loot &&
                selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Modify == S_ProductModifyEnum.Add &&
                !S_PlayerSkill.Instance.CanAddSkill())
            {
                S_InGameUISystem.Instance.CreateLog("�� �̻� ����ǰ�� ������ �� �����ϴ�!");
                return;
            }

            // �����Ҹ�ŭ ��尡 �ִٸ� ����
            if (S_PlayerStat.Instance.CurrentGold >= selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Price)
            {
                // ��� ���
                S_PlayerStat.Instance.AddOrSubtractGold(-selectedProduct.GetComponent<S_ProductObject>().ThisProduct.Price);

                // ��ǰ �����
                storeProductsBase.SetActive(false);

                // ���� ���� ���� ��ǰ ����
                CurrentBuyingProduct = selectedProduct.GetComponent<S_ProductObject>().ThisProduct;

                // ���õ� ��ǰ�� ���Ÿ� ����
                selectedProduct.GetComponent<S_ProductObject>().ThisProduct.BuyProduct();
          

                // �ش� ������ �ֵ�ƿ� ó���ϱ�
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

                // �ı�
                Destroy(selectedProduct);
                selectedProduct = null;

                // �ʿ��� �� �� �����ߴ��� ���� �ʱ�ȭ
                isSelectRequiredProduct = false;
            }
            else // ��尡 ���ٸ�
            {
                S_InGameUISystem.Instance.CreateLog("��尡 �����մϴ�.");
            }
        }
        else // �ʿ��� �� �� �������� �ʾҴٸ�
        {
            S_InGameUISystem.Instance.CreateLog("��ǰ�� �������ּ���!");
        }
    }
    void GenerateNewProducts() // ��ǰ ����
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
    public void SelectProduct(S_Product product, S_StoreSlotEnum slot) // ProductInfo���� ȣ��
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
    public void CancelSelectedProduct() // ProductInfo���� ȣ��. ��ǰ ���� ���
    {
        Destroy(selectedProduct);
        selectedProduct = null;

        if (selectedProduct == null)
        {
            isSelectRequiredProduct = false;
        }
    }


    // ��ǰ �� �߰��� ī�� ���� ����
    public void StartSelectOptionCards(List<S_Card> cards) // ī�带 �߰��ϴ� ��ǰ �Լ�
    {
        // ��ư ����
        DisappearUIByBuyStoreProduct();
        AppearUIBySelectItem();
        DisappearStoreTextInStore();
        AppearSelectCardOrLootTextInStore();
        AppearBlackBackground();

        // ��ǰ ī�� ����
        foreach (S_Card card in cards)
        {
            GenerateOptionCard(card, optionsBase.transform, itemOptionsList);
        }

        // ����
        AlignmentProductCard(itemOptionsList, true); // true�� �ؾ� optionCards������ ���ĵȴ�.
    }
    public void GenerateOptionCard(S_Card card, Transform parent, List<GameObject> list) // ���� �߰��� ī�� �����̳� ������ ī�� ���� �ÿ��� ��� ����
    {
        // �ɼ� ī�� ����
        GameObject go = Instantiate(optionCardPrefab);
        go.GetComponent<S_OptionCard>().SetCardInfo(card);
        go.transform.SetParent(parent, true);
        go.transform.localPosition = Vector3.zero;

        // ����Ʈ�� �߰�(���� ������)
        list.Add(go);
    }
    void AlignmentProductCard(List<GameObject> cards, bool isOption) // ī�带 �����ϱ�
    {
        List<PRS> originCardPRS = SetCardPos(cards.Count, isOption);

        for (int i = 0; i < cards.Count; i++)
        {
            // ī�� ��ġ ����
            cards[i].GetComponent<S_DeckCard>().OriginPRS = originCardPRS[i];

            // ���ÿ��� ����
            cards[i].GetComponent<S_DeckCard>().OriginOrder = (i + 1) * 10;
            cards[i].GetComponent<S_DeckCard>().SetOrder(cards[i].GetComponent<S_DeckCard>().OriginOrder);

            // ī���� ��ġ ����
            cards[i].GetComponent<Transform>().localPosition = cards[i].GetComponent<S_DeckCard>().OriginPRS.Pos;
            cards[i].GetComponent<Transform>().localScale = cards[i].GetComponent<S_DeckCard>().OriginPRS.Scale;
        }
    }
    List<PRS> SetCardPos(int cardCount, bool isOption) // ī�� ��ġ �����ϴ� �޼���
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
    public void SelectCardByOption(S_Card card) // ���� �߰��� ī�� ���� �� 
    {
        // ������ ī�� �߿� card�� �ִ��� �˻�
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

        // card�� �����ٸ� �� ī�� ������ ���� ó��
        if (!same)
        {
            if (CurrentBuyingProduct.SelectCount > selectedItemList.Count)
            {
                GenerateOptionCard(card, selectedItemBase.transform, selectedItemList);
            }
            else if (CurrentBuyingProduct.SelectCount == selectedItemList.Count) // �����ϴ� ������ ���ٸ� ������ ������ ���� �����ϰ� �߰�
            {
                Destroy(selectedItemList[0]);
                selectedItemList.RemoveAt(0);

                GenerateOptionCard(card, selectedItemBase.transform, selectedItemList);
            }
        }

        // ���õ� �ɼ� ī������ üũ
        foreach (GameObject go in selectedItemList)
        {
            go.GetComponent<S_OptionCard>().IsSelectedOptionCard = true;
        }

        // ����
        AlignmentProductCard(selectedItemList, false);

        // �����ϴ°� �� ������� üũ
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

        // ���� ����� ī�带 selectedCards���� ã��
        foreach (GameObject go in selectedItemList)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo == card)
            {
                exceptedCard = go;
            }
        }

        // ã�� ī�带 ����Ʈ���� ���� ��Ʈ����
        selectedItemList.Remove(exceptedCard);
        Destroy(exceptedCard);

        // ����
        AlignmentProductCard(selectedItemList, false);

        // ������ �� �ִ��� ���� Ȱ��ȭ
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }


    // ��ǰ �� ī�� ���� ����
    public async void StartSelectCardInDeck()
    {
        // �� ���� UI ����
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
            if (CurrentBuyingProduct.SelectCount > selectedItemList.Count) // �����ϴ� ������ ���� ���Ҵٸ� ������ ī�带 �׳� �߰�
            {
                GenerateOptionCard(card, selectedRemoveCardBase.transform, selectedItemList);
            }
            else if (CurrentBuyingProduct.SelectCount == selectedItemList.Count) // �����ϴ� ������ ���ٸ� ������ ������ ���� �����ϰ� �߰�
            {
                Destroy(selectedItemList[0]);
                selectedItemList.RemoveAt(0);

                GenerateOptionCard(card, selectedRemoveCardBase.transform, selectedItemList);
            }
        }

        // ���õ� ī��� ����
        foreach (GameObject go in selectedItemList)
        {
            go.GetComponent<S_DeckCard>().IsSelectedByRemoveCard = true;
        }

        // ����
        AlignmentProductCard(selectedItemList, false);

        // ������ �� �ִ��� ���� Ȱ��ȭ
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }


    // ��ǰ �� ����ǰ ���� ����
    public void StartSelectOptionLoots(List<S_Skill> loots)
    {
        // ��ư ����
        DisappearUIByBuyStoreProduct();
        AppearUIBySelectItem();
        DisappearStoreTextInStore();
        AppearSelectCardOrLootTextInStore();
        AppearBlackBackground();

        // ��ǰ ����ǰ ����
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

        // ����
        AlignmentProductLoot(itemOptionsList, true); // true�� �ؾ� optionCards������ ���ĵȴ�.
    }
    public void GenerateOptionLoot(S_Skill loot, Transform parent, List<GameObject> list) // ���� �߰��� ī�� �����̳� ������ ī�� ���� �ÿ��� ��� ����
    {
        // ī�� ����
        GameObject go = Instantiate(optionLootPrefab);
        go.GetComponent<S_ReadOnlyLoot>().SetLootInfo(loot);
        go.transform.SetParent(parent, true);
        go.transform.localPosition = Vector3.zero;

        // ����Ʈ�� �߰�(���� ������)
        list.Add(go);
    }
    void AlignmentProductLoot(List<GameObject> cards, bool isOption) // ī�带 �����ϱ�
    {
        List<PRS> originCardPRS = SetLootPos(cards.Count, isOption);

        for (int i = 0; i < cards.Count; i++)
        {
            // ī�� ��ġ ����
            cards[i].GetComponent<S_ReadOnlyLoot>().OriginPRS = originCardPRS[i];

            // ī���� ��ġ ����
            cards[i].GetComponent<Transform>().localPosition = cards[i].GetComponent<S_ReadOnlyLoot>().OriginPRS.Pos;
            cards[i].GetComponent<Transform>().localScale = cards[i].GetComponent<S_ReadOnlyLoot>().OriginPRS.Scale;
        }
    }
    List<PRS> SetLootPos(int count, bool isOption) // ī�� ��ġ �����ϴ� �޼���
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
    public void SelectSkillByOption(S_Skill loot) // ���� �߰��� ī�� ���� ��
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
            if (CurrentBuyingProduct.SelectCount > selectedItemList.Count) // �����ϴ� ������ ���� ���Ҵٸ� ������ ī�带 �׳� �߰�
            {
                GenerateOptionLoot(loot, selectedItemBase.transform, selectedItemList);
            }
            else if (CurrentBuyingProduct.SelectCount == selectedItemList.Count) // �����ϴ� ������ ���ٸ� ������ ������ ���� �����ϰ� �߰�
            {
                Destroy(selectedItemList[0]);
                selectedItemList.RemoveAt(0);

                GenerateOptionLoot(loot, selectedItemBase.transform, selectedItemList);
            }
        }

        // ���õ� ����ǰ�� ���õ� ó��
        foreach (GameObject go in selectedItemList)
        {
            go.GetComponent<S_ReadOnlyLoot>().IsSelectedOption = true;
        }

        // ����
        AlignmentProductLoot(selectedItemList, false);

        // ������ �� �ִ��� ���� Ȱ��ȭ
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

        // ���� ����� ����ǰ�� selected���� ã��
        foreach (GameObject go in selectedItemList)
        {
            if (go.GetComponent<S_ReadOnlyLoot>().LootInfo.Equals(loot))
            {
                excepted = go;
            }
        }

        // ã�� ����ǰ�� ����Ʈ���� ���� ��Ʈ����
        selectedItemList.Remove(excepted);
        Destroy(excepted);

        // ����
        AlignmentProductLoot(selectedItemList, false);

        // ������ �� �ִ��� ���� Ȱ��ȭ
        if (CurrentBuyingProduct.SelectCount == selectedItemList.Count)
        {
            isSelectRequiredItems = true;
        }
        else
        {
            isSelectRequiredItems = false;
        }
    }


    // ��ǰ �� ����ǰ ���� ����
    public void StartRemoveLoot()
    {
        // ��ư ����
        DisappearUIByBuyStoreProduct();
        AppearUIBySelectItem();
        DisappearStoreTextInStore();
        AppearSelectCardOrLootTextInStore();
        AppearBlackBackground();

        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.StoreByRemove;
    }


    // ȹ���� ī�� �����ִ� UI
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


    // ��ư �Լ�
    public void ClickRefresh()
    {
        if (S_PlayerStat.Instance.CurrentGold >= refreshGold)
        {
            // ��� ���
            S_PlayerStat.Instance.AddOrSubtractGold(-refreshGold);

            // ���ΰ�ħ ��� ����
            refreshGold += refreshGoldIncreaseValue;
            text_RefreshBtn.text = $"���ΰ�ħ\n{refreshGold} ���";

            // ������ �ִ� ��ǰ ����
            foreach (GameObject go in storeProducts)
            {
                Destroy(go);
            }
            storeProducts.Clear();

            // ���� ��Ȱ��ȭ
            CurrentBuyingProduct = null;
            isSelectRequiredProduct = false;

            // ������ ��ǰ�� �ִٸ� ���ֱ�
            if (selectedProduct != null)
            {
                Destroy(selectedProduct);
                selectedProduct = null;
            }

            // ���ο� ��ǰ ���
            GenerateNewProducts();
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("��尡 �����մϴ�.");
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
        // �ʼ� ������ ��� �ߴٸ�
        if (isSelectRequiredItems)
        {
            if (CurrentBuyingProduct.Modify == S_ProductModifyEnum.Add) // ����ǰ�̳� ī�带 �߰��ϴ� ���
            {
                if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Card) // ī���� ���
                {
                    foreach (GameObject go in selectedItemList)
                    {
                        S_PlayerCard.Instance.AddCard(go.GetComponent<S_DeckCard>().CardInfo);
                        GenerateUICard(go.GetComponent<S_DeckCard>().CardInfo);
                        Destroy(go);
                    }
                }
                else if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Loot) // ����ǰ�� ���
                {
                    foreach (GameObject go in selectedItemList)
                    {
                        S_PlayerSkill.Instance.AddSkill(go.GetComponent<S_ReadOnlyLoot>().LootInfo);
                        GenerateUILoot(go.GetComponent<S_ReadOnlyLoot>().LootInfo);
                        Destroy(go);
                    }

                    S_SkillInfoSystem.Instance.UpdateSkillObject();
                }

                // ������ ��� ����
                selectedItemList.Clear();

                // �����ߴ� ������ �ɼ� ��ϵ� ����
                foreach (GameObject go in itemOptionsList)
                {
                    Destroy(go);
                }
                itemOptionsList.Clear();

                // UI �ǵ�����
                AppearUIByBuyStoreProduct();
                DisappearUIBySelectItem();
                AppearStoreTextInStore();
                DisappearSelectCardOrLootTextInStore();
                DisappearBlackBackground();

                // ��ǰ �ٽ� Ȱ��ȭ
                storeProductsBase.SetActive(true);
            }
            else if (CurrentBuyingProduct.Modify == S_ProductModifyEnum.Remove) // ����ǰ�̳� ī�带 �����ϴ� ���
            {
                if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Card) // ī���� ���
                {
                    // ��ǰ Ű�� ���� �ٸ��� ó���ϱ�
                    switch (CurrentBuyingProduct.Key)
                    {
                        case "P_DreamingDetermination":
                            // ���� ������ ī�� ����
                            S_Card addCard1 = S_CardManager.Instance.GenerateRandomCard(-1, selectedItemList[0].GetComponent<S_DeckCard>().CardInfo.Suit);

                            // ī�� �߰��ϰ� �߰��� ī�� �����ִ� VFX����
                            S_PlayerCard.Instance.AddCard(addCard1);
                            GenerateUICard(addCard1);

                            // selected �ʱ�ȭ
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_RebelliousDetermination":
                            // ���� ������ ī�� ����
                            S_Card addCard2 = S_CardManager.Instance.GenerateRandomCard(selectedItemList[0].GetComponent<S_DeckCard>().CardInfo.Number);

                            // ī�� �߰��ϰ� �߰��� ī�� �����ִ� VFX����
                            S_PlayerCard.Instance.AddCard(addCard2);
                            GenerateUICard(addCard2);

                            // selected �ʱ�ȭ
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_ImmortalDetermination":
                            // ���� ī�� ����
                            S_Card addCard3 = S_CardManager.Instance.GenerateCopyCard(selectedItemList[0].GetComponent<S_DeckCard>().CardInfo);

                            // ī�� �߰��ϰ� �߰��� ī�� �����ִ� VFX����
                            S_PlayerCard.Instance.AddCard(addCard3);
                            GenerateUICard(addCard3);

                            // selected �ʱ�ȭ
                            foreach (GameObject go in selectedItemList)
                            {
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_TwistedDetermination":
                            // ������ ���ڿ� ������ ī�� ����
                            S_Card addCard4 = S_CardManager.Instance.GenerateRandomCard(selectedItemList[0].GetComponent<S_DeckCard>().CardInfo.Number, selectedItemList[1].GetComponent<S_DeckCard>().CardInfo.Suit);

                            // ī�� �߰��ϰ� �߰��� ī�� �����ִ� VFX����
                            S_PlayerCard.Instance.AddCard(addCard4);
                            GenerateUICard(addCard4);

                            // selected �ʱ�ȭ
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_BurningDetermination":
                            // selected �ʱ�ȭ
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                        case "P_CreatureLost":
                            // ���� ������ ī�� ����
                            S_Card addCard5 = S_CardManager.Instance.GenerateRandomCard();

                            // ī�� �߰��ϰ� �߰��� ī�� �����ִ� VFX����
                            S_PlayerCard.Instance.AddCard(addCard5);
                            GenerateUICard(addCard5);

                            // selected �ʱ�ȭ
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

                            // ī�� �߰��ϰ� �߰��� ī�� �����ִ� VFX����
                            S_PlayerCard.Instance.AddCard(addCard6);
                            GenerateUICard(addCard6);

                            // selected �ʱ�ȭ
                            foreach (GameObject go in selectedItemList)
                            {
                                S_PlayerCard.Instance.RemoveCard(go.GetComponent<S_DeckCard>().CardInfo);
                                Destroy(go);
                            }
                            selectedItemList.Clear();
                            break;
                    }

                    // �� ���� UI ����
                    await S_DeckInfoSystem.Instance.ClostDeckInfoByEndSelectRemoveCardByStore();
                }
                else if (CurrentBuyingProduct.Type == S_ProductTypeEnum.Loot) // ����ǰ�� ���
                {
                    foreach (GameObject go in selectedItemList)
                    {
                        S_PlayerSkill.Instance.RemoveSkill(go.GetComponent<S_ReadOnlyLoot>().LootInfo);
                        GenerateUILoot(go.GetComponent<S_ReadOnlyLoot>().LootInfo, true);
                        Destroy(go);
                    }

                    selectedItemList.Clear();

                    // UI �ǵ�����
                    AppearUIByBuyStoreProduct();
                    DisappearUIBySelectItem();
                    AppearStoreTextInStore();
                    DisappearSelectCardOrLootTextInStore();
                    DisappearBlackBackground();
                }
            }

            // ��ǰ �ٽ� Ȱ��ȭ
            storeProductsBase.SetActive(true);

            // ������ �ɼ� ���ÿ� ���� ���� �ʱ�ȭ
            CurrentBuyingProduct = null;
            isSelectRequiredItems = false;
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("ī�� Ȥ�� ����ǰ�� �������ּ���!");
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