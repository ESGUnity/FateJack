using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class S_RewardInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_ProductObj;
    [SerializeField] GameObject prefab_OptionCardObj; 
    [SerializeField] GameObject prefab_ShowingCardObj;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject pos_PaidProductsBase;
    [SerializeField] GameObject pos_OptionsBase;

    [Header("컴포넌트")]
    GameObject panel_ExitBtnsBase;

    [Header("상품 연출")]
    [HideInInspector] public S_ProductEnum BuiedProduct;
    List<GameObject> paidProductObjects = new();
    Vector3 PRODUCTS_START_POS = new Vector3(-5.8f, 0, 0);
    Vector3 PRODUCTS_END_POS = new Vector3(5.8f, 0, 0);
    Vector3 PRODUCTS_SCALE_VALUE = new Vector3(1.5f, 1.5f, 1.5f);

    [Header("옵션")]
    List<GameObject> optionObjs = new();
    Vector3 OPTION_CARD_START_POS = new Vector3(-5.8f, 0, 0);
    Vector3 OPTION_CARD_END_POS = new Vector3(5.8f, 0, 0);
    Vector3 MY_CARD_START_POS = new Vector3(-7.5f, 0, 0);
    Vector3 MY_CARD_END_POS = new Vector3(7.5f, 0, 0);
    Vector3 CARDS_SCALE_VALUE = new Vector3(1.7f, 1.7f, 1.7f);
    const float STACK_Z_VALUE = -0.02f;

    [Header("UI")]
    Vector2 REWARD_BTNS_HIDE_POS = new Vector2(0, -80);
    Vector2 REWARD_BTNS_ORIGIN_POS = new Vector2(0, 85);

    // 싱글턴
    static S_RewardInfoSystem instance;
    public static S_RewardInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        panel_ExitBtnsBase = System.Array.Find(transforms, c => c.gameObject.name.Equals("Panel_ExitBtnsBase")).gameObject;

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

    public async Task StartReward()
    {
        // 상품 생성
        GenerateNewProducts();
        AppearExitBtn();
        await S_GameFlowManager.WaitPanelAppearTimeAsync();

        // 상품 등장
        AlignmentProducts();

        //상점 조우 대사
        DialogData dialog = S_DialogMetaData.GetMonologs("Reward_Start");
        S_DialogInfoSystem.Instance.StartMonolog(dialog.Name, dialog.Dialog, 8);
    }
    public async Task StartRewardByTutorial()
    {
        // 상품 생성
        GenerateNewProducts();
        AppearExitBtn();
        await S_GameFlowManager.WaitPanelAppearTimeAsync();

        // 상품 등장
        AlignmentProducts();
    }
    #region UI
    public void AppearExitBtn()
    {
        panel_ExitBtnsBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_ExitBtnsBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_ExitBtnsBase.GetComponent<RectTransform>().DOAnchorPos(REWARD_BTNS_ORIGIN_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearExitBtn()
    {
        panel_ExitBtnsBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_ExitBtnsBase.GetComponent<RectTransform>().DOAnchorPos(REWARD_BTNS_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_ExitBtnsBase.SetActive(false));
    }
    public void AppearMonlogByBuiedProduct() // 구매한 상품이 뭘 선택해야하는지 알려주는 메서드
    {
        DialogData dialog = S_DialogMetaData.GetMonologs($"Reward_{BuiedProduct}_Buied");
        S_DialogInfoSystem.Instance.StartMonolog(dialog.Name, dialog.Dialog, 9999);
    }
    public void DisappearMonologByBuiedProduct()
    {
        S_DialogInfoSystem.Instance.EndMonolog();
    }
    #endregion
    #region 상품 관련
    void GenerateNewProducts() // 상품 생성
    {
        // 유료 상품
        S_FoeStruct foe = S_FoeList.FOES.First(x => x.Trial == S_GameFlowManager.Instance.CurrentTrial);
        var allProducts = foe.Rewards.OrderBy(x => Random.value).Take(foe.RewardCount);

        // 상품 생성
        foreach (S_ProductEnum product in allProducts)
        {
            GameObject go = Instantiate(prefab_ProductObj, pos_PaidProductsBase.transform);
            go.transform.localPosition = new Vector3(0, 0, -11);
            go.GetComponent<S_ProductObj>().SetProductInfo(product);
            paidProductObjects.Add(go);
        }

        // 상품 정렬
        AlignmentPaidProductsPrev();
    }
    public async void BuyProduct(S_ProductObj product) // S_ProductObject에서 호출
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 현재 구매 중인 상품 설정
        BuiedProduct = product.ProductInfo;

        // 구매 로직
        switch (BuiedProduct)
        {
            case S_ProductEnum.OldMold: StartSelectOptionCards(5, S_CardTypeEnum.None); break;
            case S_ProductEnum.MeltedMold: StartSelectOptionCards(3, S_CardTypeEnum.Str); break;
            case S_ProductEnum.SpiritualMold: StartSelectOptionCards(3, S_CardTypeEnum.Mind); break;
            case S_ProductEnum.BrightMold: StartSelectOptionCards(3, S_CardTypeEnum.Luck); break;
            case S_ProductEnum.DelicateMold: StartSelectOptionCards(3, S_CardTypeEnum.Common); break;
        }

        // 나가기 버튼 사라짐
        DisappearExitBtn();

        // 구매한 옵션에 대한 모놀로그 띄우기
        AppearMonlogByBuiedProduct();

        // 구매한 옵션은 제거
        for (int i = 0; i < paidProductObjects.Count; i++)
        {
            GameObject productObj = paidProductObjects[i];
            if (productObj.GetComponent<S_ProductObj>().ProductInfo == BuiedProduct)
            {
                paidProductObjects.RemoveAt(i);
                Destroy(productObj);
                break;
            }
        }

        await S_EffectActivator.Instance.WaitEffectLifeTimeAsync();
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.StoreBuying;
    }
    #endregion
    #region 카드 선택 상품
    public void StartSelectOptionCards(int count, S_CardTypeEnum cardType)
    {
        List<S_CardBase> cards = S_CardList.PickRandomCards(count, cardType);

        foreach (S_CardBase card in cards)
        {
            GenerateOptionCardObj(card);
        }

        AlignmentOptionCards();
    }
    public void StartSelectMyCards(int count, S_CardTypeEnum cardType)
    {
        List<S_CardBase> cards = S_PlayerCard.Instance.GetOriginPlayerDeckCards();

        if (cards.Count > count)
        {
            cards = cards.OrderBy(x => Random.value).Take(count).ToList(); 
        }
        
        foreach (S_CardBase card in cards)
        {
            GenerateOptionCardObj(card);
        }

        AlignmentMyCards();
    }
    public void GenerateOptionCardObj(S_CardBase card) // 옵션 카드 생성
    {
        // 카드 생성
        GameObject go = Instantiate(prefab_OptionCardObj, pos_OptionsBase.transform);
        go.GetComponent<S_OptionCardObj>().SetCardInfo(card);
        go.transform.localPosition = Vector3.zero;

        // 리스트에 추가
        optionObjs.Add(go);
    }
    public async Task DecideSelectCard(S_CardBase card) // 옵션 선택 버튼에서 호출
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 선택한 카드를 가운데로 이동시키기
        GameObject obj = null;
        foreach (GameObject go in optionObjs)
        {
            if (go.GetComponent<S_OptionCardObj>().CardInfo == card)
            {
                obj = go;
                break;
            }
        }
        GameObject showingObj = Instantiate(prefab_ShowingCardObj);
        showingObj.GetComponent<S_ShowingCardObj>().SetCardInfo(card);
        showingObj.transform.SetParent(pos_OptionsBase.transform);
        showingObj.transform.localPosition = obj.transform.localPosition;
        showingObj.transform.eulerAngles = obj.transform.eulerAngles;
        showingObj.transform.localScale = obj.transform.localScale;
        showingObj.GetComponent<S_ShowingCardObj>().SetOrder(100);
        // 옵션 제거
        DestroyOptionObjs();
        // 가운데로 이동
        await showingObj.GetComponent<S_ShowingCardObj>().MoveShowingPos();

        // 카드 획득
        if (BuiedProduct == S_ProductEnum.OldMold || BuiedProduct == S_ProductEnum.MeltedMold || BuiedProduct == S_ProductEnum.SpiritualMold ||
            BuiedProduct == S_ProductEnum.BrightMold || BuiedProduct == S_ProductEnum.DelicateMold)
        {
            S_PlayerCard.Instance.AddCard(card);
            await showingObj.GetComponent<S_ShowingCardObj>().MoveDeckPos();
        }
        else // 각인 부여
        {
            List<S_EngravingEnum> prev;
            prev = card.Engraving;
            card.Engraving.Clear();

            switch (BuiedProduct)
            {
                case S_ProductEnum.Immunity: card.Engraving.Add(S_EngravingEnum.Immunity); break;
                case S_ProductEnum.QuickAction: card.Engraving.Add(S_EngravingEnum.QuickAction); break;
                case S_ProductEnum.Rebound: card.Engraving.Add(S_EngravingEnum.Rebound); break;
                case S_ProductEnum.Fix: card.Engraving.Add(S_EngravingEnum.Fix); break;
                case S_ProductEnum.Flexible: card.Engraving.Add(S_EngravingEnum.Flexible); break;
                case S_ProductEnum.Leap: card.Engraving.Add(S_EngravingEnum.Leap); break;
                case S_ProductEnum.Dismantle: card.Engraving.Add(S_EngravingEnum.Dismantle); break;
                case S_ProductEnum.Mask: card.Engraving.Add(S_EngravingEnum.Mask); break;
            }

            await showingObj.GetComponent<S_ShowingCardObj>().ChangeEngraving(prev);
            await showingObj.GetComponent<S_ShowingCardObj>().MoveDeckPos();
        }

        S_DeckInfoSystem.Instance.SetDeckCardInfo();

        // 각종 UI를 구매 전으로, BuiedProduct 초기화, GameFlowState 되돌리기
        await EndDecide(); 
    }
    #endregion
    #region 버튼 함수
    public void ClickExit()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // 버튼 제거, 모놀로그 제거
        DisappearExitBtn();
        DisappearMonologByBuiedProduct();

        // 남아있는 상품 제거
        foreach (GameObject go in paidProductObjects)
        {
            Destroy(go);
        }
        paidProductObjects.Clear();
        BuiedProduct = S_ProductEnum.None;

        // 시련 다시 시작
        S_GameFlowManager.Instance.StartTrial();
    }
    #endregion
    #region 보조 함수
    void DestroyOptionObjs() // 옵션 모두 제거
    {
        foreach (GameObject go in optionObjs)
        {
            Destroy(go);
        }

        optionObjs.Clear();
    }
    async Task EndDecide() // 각종 UI를 구매 전으로, BuiedProduct 초기화, GameFlowState 되돌리기
    {
        AppearExitBtn();
        DisappearMonologByBuiedProduct();

        await S_GameFlowManager.WaitPanelAppearTimeAsync();

        BuiedProduct = S_ProductEnum.None;
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
    }  
    void AlignmentPaidProductsPrev() // 상품 정렬(공중)
    {
        // 유료 상품 정렬
        List<PRS> originCardPRS2 = SetObjectsPos(paidProductObjects.Count, PRODUCTS_START_POS, PRODUCTS_END_POS, PRODUCTS_SCALE_VALUE);

        for (int i = 0; i < paidProductObjects.Count; i++)
        {
            // 위치 설정
            paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS = originCardPRS2[i];

            // 소팅오더 설정
            paidProductObjects[i].GetComponent<S_ProductObj>().OriginOrder = i;
            paidProductObjects[i].GetComponent<S_ProductObj>().SetOrder(paidProductObjects[i].GetComponent<S_ProductObj>().OriginOrder);

            paidProductObjects[i].transform.DOLocalMove(paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Pos + new Vector3(0, 0, -10f), 0);
            paidProductObjects[i].transform.DOScale(paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Scale, 0);
        }
    }
    void AlignmentProducts() // 상품 정렬
    {
        // 유료 상품 정렬
        List<PRS> originCardPRS2 = SetObjectsPos(paidProductObjects.Count, PRODUCTS_START_POS, PRODUCTS_END_POS, PRODUCTS_SCALE_VALUE);

        for (int i = 0; i < paidProductObjects.Count; i++)
        {
            // 위치 설정
            paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS = originCardPRS2[i];

            // 소팅오더 설정
            paidProductObjects[i].GetComponent<S_ProductObj>().OriginOrder = i;
            paidProductObjects[i].GetComponent<S_ProductObj>().SetOrder(paidProductObjects[i].GetComponent<S_ProductObj>().OriginOrder);

            paidProductObjects[i].transform.DOLocalMove(paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            paidProductObjects[i].transform.DOScale(paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }
    }
    void AlignmentOptionCards() // 카드 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(optionObjs.Count, OPTION_CARD_START_POS, OPTION_CARD_END_POS, CARDS_SCALE_VALUE);

        for (int i = 0; i < optionObjs.Count; i++)
        {
            // 카드 위치 설정
            optionObjs[i].GetComponent<S_OptionCardObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            optionObjs[i].GetComponent<S_OptionCardObj>().OriginOrder = (i + 1) * 10;
            optionObjs[i].GetComponent<S_OptionCardObj>().SetOrder(optionObjs[i].GetComponent<S_OptionCardObj>().OriginOrder);

            // 카드의 위치 설정
            optionObjs[i].transform.DOLocalMove(optionObjs[i].GetComponent<S_OptionCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            optionObjs[i].transform.DOScale(optionObjs[i].GetComponent<S_OptionCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }
    }
    void AlignmentMyCards() // 카드 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(optionObjs.Count, OPTION_CARD_START_POS, OPTION_CARD_END_POS, CARDS_SCALE_VALUE);

        for (int i = 0; i < optionObjs.Count; i++)
        {
            // 카드 위치 설정
            optionObjs[i].GetComponent<S_OptionCardObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            optionObjs[i].GetComponent<S_OptionCardObj>().OriginOrder = (i + 1) * 10;
            optionObjs[i].GetComponent<S_OptionCardObj>().SetOrder(optionObjs[i].GetComponent<S_OptionCardObj>().OriginOrder);

            // 카드의 위치 설정
            optionObjs[i].transform.DOLocalMove(optionObjs[i].GetComponent<S_OptionCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            optionObjs[i].transform.DOScale(optionObjs[i].GetComponent<S_OptionCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }
    }
    List<PRS> SetObjectsPos(int count, Vector3 startPos, Vector3 endPos, Vector3 scaleValue) // 위치 설정하는 메서드
    {
        if (count <= 0) return null;

        float[] lerps = new float[count];
        List<PRS> results = new List<PRS>(count);

        if (count == 1)
        {
            lerps[0] = 0.5f;
        }
        else if (count == 2)
        {
            lerps[0] = 0.3f;
            lerps[1] = 0.7f;
        }
        else
        {
            float interval = 1f / (count - 1);
            for (int i = 0; i < count; i++)
            {
                lerps[i] = interval * i;
            }
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Vector3.Lerp(startPos, endPos, lerps[i]);
            pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);
            Vector3 rot = Vector3.zero;
            Vector3 scale = scaleValue;
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    #endregion
}

public enum S_ProductEnum
{
    None,
    // 카드
    OldMold, MeltedMold, SpiritualMold, BrightMold, DelicateMold,
    // 각인
    Immunity, QuickAction, Rebound, Fix, Flexible, Leap, Dismantle, Mask,
    // 카드 제거
    WasteBasket, 
}