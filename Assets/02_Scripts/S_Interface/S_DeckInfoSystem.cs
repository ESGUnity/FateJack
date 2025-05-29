using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_DeckInfoSystem : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject deckCard;

    [Header("�� ������Ʈ")]
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

    [Header("�� ī�� ������Ʈ ����Ʈ")]
    List<GameObject> spadeCards = new();
    List<GameObject> heartCards = new();
    List<GameObject> diamondCards = new();
    List<GameObject> cloverCards = new();
    List<GameObject> deckCardObjects = new();

    [Header("������Ʈ")]
    GameObject panel_DeterminationHitBtnAndCancelBtnBase;
    GameObject panel_CloseDeckBtnBase;
    GameObject panel_ViewDeckInfoBtnBase;

    [Header("�� ���ų� �ݴ� UI")]
    Vector2 deckInfoBtnsHidePos = new Vector2(0, -80);
    Vector2 deckInfoBtnsOriginPos = new Vector2(0, 55);
    Vector2 viewDeckInfoBtnHidePos = new Vector2(-180, -10);
    Vector2 viewDeckInfoBtnOriginPos = new Vector2(10, -10);
    Vector3 deckCameraPos = new Vector3(0, -14, -15);
    Vector3 inGameCameraPos = new Vector3(0, 0, -15);
    Vector3 storeCameraPos = new Vector3(0, 10, -15);

    [Header("ī�� UI")]
    Vector3 startPoint = new Vector3(-3.85f, 0, 0);
    Vector3 endPoint = new Vector3(3.85f, 0, 0);
    float zValue = -0.02f;

    [Header("���� ��Ʈ ����")]
    bool isDeterminationHit;
    bool isSelect;
    S_Card selectedCard;

    // �̱���
    static S_DeckInfoSystem instance;
    public static S_DeckInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        panel_DeterminationHitBtnAndCancelBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_DeterminationHitBtnAndCancelBtnBase")).gameObject;
        panel_CloseDeckBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_CloseDeckBtnBase")).gameObject;
        panel_ViewDeckInfoBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_ViewDeckInfoBtnBase")).gameObject;

        // TMP�� ���ÿ��� ����
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

        // �̱���
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

        // ��Ʈ������ ���� �ִϸ��̼� �ֱ�
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOAnchorPos(viewDeckInfoBtnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearViewDeckInfoBtn()
    {
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_ViewDeckInfoBtnBase.GetComponent<RectTransform>().DOAnchorPos(viewDeckInfoBtnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_ViewDeckInfoBtnBase.SetActive(false));
    }

    public void AddDeck(S_Card card) // ���� ī�� �߰��ϱ�
    {
        GameObject go = Instantiate(deckCard); // �� ī�� ������ ����

        go.GetComponent<S_DeckCard>().SetCardInfo(card); // ī�� ���� ����
        deckCardObjects.Add(go); // ���� ī�� ������Ʈ�� �߰�

        // ���翡 ���� ���̽� �����ֱ�
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
    void AlignmentStackCard(List<GameObject> suitCards) // ī�带 �����ϱ�
    {
        List<PRS> originCardPRS = SetStackCardPos(suitCards.Count);
        List<Task> tweenTask = new List<Task>();

        suitCards.Sort((a, b) => a.GetComponent<S_DeckCard>().CardInfo.Number.CompareTo(b.GetComponent<S_DeckCard>().CardInfo.Number));

        for (int i = 0; i < suitCards.Count; i++)
        {
            // ī�� ��ġ ����
            suitCards[i].GetComponent<S_DeckCard>().OriginPRS = originCardPRS[i];

            // ���ÿ��� ����
            suitCards[i].GetComponent<S_DeckCard>().OriginOrder = (i + 1) * 10;
            suitCards[i].GetComponent<S_DeckCard>().SetOrder(suitCards[i].GetComponent<S_DeckCard>().OriginOrder);

            // ī���� ��ġ ����
            suitCards[i].GetComponent<Transform>().localPosition = suitCards[i].GetComponent<S_DeckCard>().OriginPRS.Pos;
            suitCards[i].GetComponent<Transform>().localScale = suitCards[i].GetComponent<S_DeckCard>().OriginPRS.Scale;
        }

        UpdateDeckCardsState();
    }
    List<PRS> SetStackCardPos(int cardCount) // ī�� ��ġ �����ϴ� �޼���
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
    public void RemoveDeck(S_Card card) // ������ ī�� �����ϱ�
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

    #region ��ư �Լ�
    public async void ClickViewDeckInfoBtn() // Open �� ���� ��ư �Լ�
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

            // Store UI ���� �����
            S_StoreInfoSystem.Instance.DisappearUIBySelectItem();
            S_StoreInfoSystem.Instance.DisappearUIByBuyStoreProduct();
            S_StoreInfoSystem.Instance.DisappearStoreTextInStore();
            S_StoreInfoSystem.Instance.DisappearBlackBackground();

            // �� ����
            await OpenDeckInfoCommonProperty(false);

            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
        }
    }
    public async void ClickDeterminationHitBtn() // Open �� ���� ȭ���� ���� ��Ʈ ��ư �Լ�
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
            S_InGameUISystem.Instance.CreateLog("����Ʈ �ÿ� ���� ��Ʈ�� �� �� ���ٴ� ���̳�~!");
        }
        else if (!S_PlayerStat.Instance.CanUseDetermination())
        {
            S_InGameUISystem.Instance.CreateLog("������ �����ϱ�.");
        }
        else if (S_PlayerCard.Instance.GetPreDeckCards().Count <= 0)
        {
            S_InGameUISystem.Instance.CreateLog("���� ī�尡 ����.");
        }
        else if (S_PlayerCard.Instance.GetPreStackCards().Count > 48)
        {
            S_InGameUISystem.Instance.CreateLog("�� �̻� ���ÿ� ī�带 �� �� �����ϴ�. �ִ� ��� : 48��");
        }
    }
    public async void ClickDecideDeterminationHitCardBtn() // Close. ���� ��Ʈ�� ī�带 �����ϴ� ��ư �Լ�
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.OnDeckInfo && S_PlayerStat.Instance.CanUseDetermination())
        {
            if (isSelect)
            {
                // ��Ʈ �� ���� ����
                S_PlayerStat.Instance.UseDetermination();

                // �������� ���� �߿� �������� ��Ʈ ��ư UI�� �߰� �����ؾ���.
                S_FoeInfoSystem.Instance.AppearCreature();
                S_HitBtnSystem.Instance.AppearHitBtn();
                S_PlayerInfoSystem.Instance.AppearPlayerImage();

                // ī�޶� �̵�
                Camera.main.transform.DOMove(inGameCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

                // ���� ��Ʈ �� ���Ǵ� selectCard ������Ʈ ��Ȱ��ȭ�ϰ� ���� ���ε� ����
                selectCard.SetActive(false);
                isSelect = false;

                // �� ���� �ݱ�
                await CloseDeckInfoCommonProperty();

                // ī�� ����
                await S_GameFlowManager.Instance.EnqueueCardOrderAndUpdateCardsState(selectCard.GetComponent<S_DeckCard>().CardInfo, S_CardOrderTypeEnum.BasicHit);

                // ��Ʈ ī�� ����
                if (S_GameFlowManager.Instance.GetCardOrderQueueCount() <= 1)
                {
                    await S_GameFlowManager.Instance.StartHittingCard();
                }
            }
            else
            {
                S_InGameUISystem.Instance.CreateLog("���� ��Ʈ�� ī�带 ������.");
            }
        }
    }
    public async void ClickCloseDeckInfoBtn() // Close �� ������ ���� ��Ʈ ����� �� ȣ��
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.OnDeckInfo)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // �������� ���� �߿� �������� ��Ʈ ��ư UI�� �߰� �����ؾ���.
            S_FoeInfoSystem.Instance.AppearCreature();
            S_HitBtnSystem.Instance.AppearHitBtn();
            S_PlayerInfoSystem.Instance.AppearPlayerImage();

            // ī�޶� �̵�
            Camera.main.transform.DOMove(inGameCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            // ���� ��Ʈ �� ���Ǵ� selectCard ������Ʈ ��Ȱ��ȭ�ϰ� ���� ���ε� ����
            selectCard.SetActive(false);
            isSelect = false;

            // �� ���� �ݱ�
            await CloseDeckInfoCommonProperty();

            // �ٽ� Hit�� ��ȯ
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Hit;
        }
        else if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store)
        {
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

            // ���� UI ������
            S_StoreInfoSystem.Instance.AppearStoreTextInStore();
            if (S_StoreInfoSystem.Instance.CurrentBuyingProduct != null) // ������ �������� ���� ��
            {
                S_StoreInfoSystem.Instance.AppearUIBySelectItem();
            }
            else // ���� ��
            {
                S_StoreInfoSystem.Instance.AppearUIByBuyStoreProduct();
            }

            // ī�޶� �̵�
            Camera.main.transform.DOMove(storeCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

            // �� ���� �ݱ�
            await CloseDeckInfoCommonProperty();

            // �ٽ� Store�� ��ȯ
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
        }
    }
    public async Task OpenDeckInfoBySelectRemoveCardByStore()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // ���� UI ����
        S_StoreInfoSystem.Instance.DisappearUIByBuyStoreProduct();
        S_StoreInfoSystem.Instance.AppearUIBySelectItem();
        S_StoreInfoSystem.Instance.DisappearStoreTextInStore();
        S_StoreInfoSystem.Instance.AppearSelectCardOrLootTextInStore();
        S_StoreInfoSystem.Instance.DisappearBlackBackground();

        // �� ���� UI ����
        await OpenDeckInfoCommonProperty(false, false);

        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.StoreByRemove;
    }
    public async Task ClostDeckInfoByEndSelectRemoveCardByStore()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // ���� UI ����
        S_StoreInfoSystem.Instance.AppearUIByBuyStoreProduct();
        S_StoreInfoSystem.Instance.DisappearUIBySelectItem();
        S_StoreInfoSystem.Instance.AppearStoreTextInStore();
        S_StoreInfoSystem.Instance.DisappearSelectCardOrLootTextInStore();
        S_StoreInfoSystem.Instance.DisappearBlackBackground();

        // ī�޶� �̵�
        Camera.main.transform.DOMove(storeCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // �� ���� �ݱ�
        await CloseDeckInfoCommonProperty();

        // �ٽ� Store�� ��ȯ
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
    }
    #endregion
    #region ���� ��Ʈ �� ȣ��Ǵ� �Լ�
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
    #region ���� �Լ�
    public void UpdateDeckCardsState() // ����, ���� ���� ����, ī���� ���� ��Ȳ ������Ʈ
    {
        // ���� ���ٸ� �������ϰ� ó���ϰ� ���ֹ��� ī�嵵 üũ
        foreach (GameObject go in deckCardObjects)
        {
            go.GetComponent<S_DeckCard>().UpdateDeckCardState();
        }

        // �ؽ�Ʈ ������Ʈ
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

        text_ExclusionCount.GetComponent<TMP_Text>().text = $"���ܵ� ī�� ���� : {S_PlayerCard.Instance.GetPreExclusionTotalCards().Count}";
        text_StackCount.GetComponent<TMP_Text>().text = $"���ÿ� �ִ� ī�� ���� : {S_PlayerCard.Instance.GetPreStackCards().Count}";
        text_DeckCount.GetComponent<TMP_Text>().text = $"���� �ִ� ī�� ���� : {S_PlayerCard.Instance.GetPreDeckCards().Count}";
    }
    // ����Ǵ� �� ���� �ݴ� ����
    public async Task OpenDeckInfoCommonProperty(bool isDetermination, bool needBtn = true)
    {
        // �ΰ��� �������̽��� �����(������, ����ǰ, ���� �ɷ�ġ, ��Ʈ ��ư, �÷��̾� �̹���, �� ���� ��ư, ���ÿ� �����ϴ� ��ư)
        S_FoeInfoSystem.Instance.DisappearCreature();
        S_SkillInfoSystem.Instance.DisappearSkill();
        S_StatInfoSystem.Instance.DisappearBattleStat();
        S_HitBtnSystem.Instance.DisappearHitBtn();
        S_PlayerInfoSystem.Instance.DisappearPlayerImage();
        S_DeckInfoSystem.Instance.DisappearViewDeckInfoBtn();
        S_StackInfoSystem.Instance.DisappearStackInfoBtn();

        // �� �������� ����� ��ư ����
        isDeterminationHit = isDetermination;
        if (needBtn)
        {
            if (isDetermination)
            {
                panel_DeterminationHitBtnAndCancelBtnBase.SetActive(true);
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                    .OnComplete(() => panel_CloseDeckBtnBase.SetActive(false));
            }
            else
            {
                panel_CloseDeckBtnBase.SetActive(true);
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
                panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
                panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                    .OnComplete(() => panel_DeterminationHitBtnAndCancelBtnBase.SetActive(false));
            }
        }

        // ��¦ ��ο����� ����� ȿ��
        sprite_BlackBackgroundByDeckInfo.DOFade(0.7f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // ī�޶� �̵�
        Camera.main.transform.DOMove(deckCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        // UI �̵� ���
        await Task.Delay(Mathf.RoundToInt(S_GameFlowManager.PANEL_APPEAR_TIME * 1000));
    }
    public async Task CloseDeckInfoCommonProperty()
    {
        // �⺻���� ���� �������̽� ����(����ǰ, ���� �ɷ�ġ, �� ���� ��ư, ���ÿ� �����ϴ� ��ư)
        S_SkillInfoSystem.Instance.AppearSkill();
        S_StatInfoSystem.Instance.AppearBattleStat();
        S_DeckInfoSystem.Instance.AppearViewDeckInfoBtn();
        S_StackInfoSystem.Instance.AppearStackInfoBtn();

        // �� ��ư ����
        if (isDeterminationHit)
        {
            panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
            panel_DeterminationHitBtnAndCancelBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                .OnComplete(() => panel_DeterminationHitBtnAndCancelBtnBase.SetActive(false));
        }
        else
        {
            panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
            panel_CloseDeckBtnBase.GetComponent<RectTransform>().DOAnchorPos(deckInfoBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
                .OnComplete(() => panel_CloseDeckBtnBase.SetActive(false));
        }

        // ��ο����� �ٽ� Ǯ���ֱ�
        sprite_BlackBackgroundByDeckInfo.DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);

        await Task.Delay(Mathf.RoundToInt(S_GameFlowManager.PANEL_APPEAR_TIME * 1000));
    }
    public List<GameObject> GetDeckCardObjects()
    {
        return deckCardObjects.ToList();
    }
    #endregion
}