using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class S_StoreInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_Section;
    [SerializeField] GameObject prefab_ProductObj;
    [SerializeField] GameObject prefab_OptionCardObj; // 현재 옵션으로 카드 고르는 상품이 없어서 안 쓰임.
    [SerializeField] GameObject prefab_OptionTrinketObj;
    [SerializeField] GameObject prefab_ShowingCardObj;
    GameObject instance_Section;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject pos_FreeProductsBase;
    [SerializeField] GameObject pos_PaidProductsBase;
    [SerializeField] GameObject pos_OptionsBase;

    [Header("컴포넌트")]
    GameObject panel_RefreshAndExitBtnsBase;
    TMP_Text text_RefreshBtn;

    [Header("상품")]
    [HideInInspector] public S_ProductInfoEnum BuiedProduct;
    List<GameObject> freeProductObjects = new();
    List<GameObject> paidProductObjects = new();
    Vector3 FREE_PRODUCTS_START_POS = new Vector3(-3f, 0, 0);
    Vector3 FREE_PRODUCTS_END_POS = new Vector3(3f, 0, 0);
    Vector3 PAID_PRODUCTS_START_POS = new Vector3(-5f, 0, 0);
    Vector3 PAID_PRODUCTS_END_POS = new Vector3(5f, 0, 0);
    Vector3 PRODUCTS_SCALE_VALUE = new Vector3(0.9f, 0.9f, 0.9f);
    int refreshGold;
    int refreshGoldIncreaseValue;
    const int ORIGIN_REFRESH_GOLD = 2;
    const int ORIGIN_REFRESH_GOLD_INCREASE_VALUE = 1;

    [Header("쓸만한 물건 상품")]
    List<GameObject> optionObjs = new();
    Vector3 OPTION_TRINKET_START_POS = new Vector3(-5f, 0, 0);
    Vector3 OPTION_TRINKET_END_POS = new Vector3(5f, 0, 0);
    Vector3 MY_TRINKET_START_POS = new Vector3(-6.5f, 0, 0);
    Vector3 MY_TRINKET_END_POS = new Vector3(6.5f, 0, 0);

    [Header("카드 상품")]
    Vector3 OPTION_CARD_START_POS = new Vector3(-1.25f, 0, 0);
    Vector3 OPTION_CARD_END_POS = new Vector3(1.25f, 0, 0);
    Vector3 MY_CARD_START_POS = new Vector3(-1.25f, 0, 0);
    Vector3 MY_CARD_END_POS = new Vector3(1.25f, 0, 0);
    const float STACK_Z_VALUE = -0.02f;

    [Header("UI")]
    Vector2 refreshAndExitBtnsHidePos = new Vector2(0, -80);
    Vector2 refreshAndExitBtnsOriginPos = new Vector2(0, 85);

    Vector3 FREE_PRODUCTS_BASE_HIDE_POS = new Vector3(-15f, -1.6f, 0);
    Vector3 FREE_PRODUCTS_BASE_ORIGIN_POS = new Vector3(-5f, -1.6f, 0);
    Vector3 PAID_PRODUCTS_BASE_HIDE_POS = new Vector3(15f, -1.6f, 0);
    Vector3 PAID_PRODUCTS_BASE_ORIGIN_POS = new Vector3(5f, -1.6f, 0);

    [Header("캐싱")]
    SpriteRenderer sprite_Character;
    string merchantName;

    // 싱글턴
    static S_StoreInfoSystem instance;
    public static S_StoreInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        panel_RefreshAndExitBtnsBase = System.Array.Find(transforms, c => c.gameObject.name.Equals("Panel_RefreshAndExitBtnsBase")).gameObject;
        text_RefreshBtn = System.Array.Find(texts, c => c.gameObject.name.Equals("Text_RefreshBtn"));

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

    public async Task StartStore()
    {
        // 상품 생성
        GenerateNewProducts();

        // 새로고침 골드 초기화
        refreshGold = ORIGIN_REFRESH_GOLD;
        refreshGoldIncreaseValue = ORIGIN_REFRESH_GOLD_INCREASE_VALUE;
        text_RefreshBtn.text = $"새로고침\n{refreshGold} 골드";

        // UI 관리
        AppearRefreshAndExitBtn();
        instance_Section = Instantiate(prefab_Section);
        instance_Section.GetComponent<S_SectionObj>().SpawnSection("Character_Store");
        await S_GameFlowManager.WaitPanelAppearTimeAsync(); // 캔버스 등장 동안 대기

        // 상점에 들어오면 상품 등장
        AppearProductsBase();

        sprite_Character = instance_Section.GetComponent<S_SectionObj>().sprite_Character.GetComponent<SpriteRenderer>();
        merchantName = S_DialogMetaData.GetMerchantName(S_GameFlowManager.Instance.CurrentTrial);

        // 상점 조우 대사
        S_DialogInfoSystem.Instance.StartMonolog(sprite_Character, merchantName, S_DialogMetaData.GetStoreMonologData("Store_Intro"), 8);
    }
    public async Task StartStoreByTutorial()
    {
        // 상품 생성
        GenerateNewProductsByTutorial();

        // 새로고침 골드 초기화
        refreshGold = ORIGIN_REFRESH_GOLD;
        refreshGoldIncreaseValue = ORIGIN_REFRESH_GOLD_INCREASE_VALUE;
        text_RefreshBtn.text = $"새로고침\n{refreshGold} 골드";

        // UI 관리
        AppearRefreshAndExitBtn();
        instance_Section = Instantiate(prefab_Section);
        instance_Section.GetComponent<S_SectionObj>().SpawnSection("Daedalus");
        await S_GameFlowManager.WaitPanelAppearTimeAsync(); // 캔버스 등장 동안 대기

        // 상점에 들어오면 상품 등장
        AppearProductsBase();
    }
    #region UI
    public void AppearProductsBase()
    {
        pos_FreeProductsBase.SetActive(true);
        pos_PaidProductsBase.SetActive(true);

        pos_FreeProductsBase.transform.DOKill();
        pos_PaidProductsBase.transform.DOKill();

        pos_FreeProductsBase.transform.DOLocalMove(FREE_PRODUCTS_BASE_ORIGIN_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        pos_PaidProductsBase.transform.DOLocalMove(PAID_PRODUCTS_BASE_ORIGIN_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearProductsBase()
    {
        pos_FreeProductsBase.transform.DOKill();
        pos_PaidProductsBase.transform.DOKill();

        pos_FreeProductsBase.transform.DOLocalMove(FREE_PRODUCTS_BASE_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => pos_FreeProductsBase.SetActive(false));
        pos_PaidProductsBase.transform.DOLocalMove(PAID_PRODUCTS_BASE_HIDE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => pos_PaidProductsBase.SetActive(false));
    }
    public void AppearRefreshAndExitBtn()
    {
        panel_RefreshAndExitBtnsBase.SetActive(true);

        // 두트윈으로 등장 애니메이션 주기
        panel_RefreshAndExitBtnsBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_RefreshAndExitBtnsBase.GetComponent<RectTransform>().DOAnchorPos(refreshAndExitBtnsOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearRefreshAndExitBtn()
    {
        panel_RefreshAndExitBtnsBase.GetComponent<RectTransform>().DOKill(); // 두트윈 전 트윈 초기화
        panel_RefreshAndExitBtnsBase.GetComponent<RectTransform>().DOAnchorPos(refreshAndExitBtnsHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_RefreshAndExitBtnsBase.SetActive(false));
    }
    public void AppearMonlogByBuiedProduct()
    {
        S_DialogInfoSystem.Instance.StartMonolog(sprite_Character, merchantName, S_ProductMetaData.GetBuiedProductDescription(BuiedProduct), 9999);
    }
    public void DisappearMonologByBuiedProduct()
    {
        S_DialogInfoSystem.Instance.EndMonolog();
    }
    #endregion
    #region 상품 관련
    void GenerateNewProducts() // 상품 생성
    {
        // 무료 상품
        var freeProducts = new List<S_ProductInfoEnum>() { S_ProductInfoEnum.OracleBall, S_ProductInfoEnum.WasteBasket, S_ProductInfoEnum.ShellGameCup };
        // 상품 생성
        foreach (S_ProductInfoEnum product in freeProducts)
        {
            GameObject go = Instantiate(prefab_ProductObj, pos_FreeProductsBase.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<S_ProductObj>().SetProductInfo(product);
            freeProductObjects.Add(go);
        }
        // 상품 정렬
        AlignmentFreeProducts();

        // 유료 상품
        var allProducts = System.Enum.GetValues(typeof(S_ProductInfoEnum))
            .Cast<S_ProductInfoEnum>()
            .Where(x => x != S_ProductInfoEnum.None && x != S_ProductInfoEnum.OracleBall && x != S_ProductInfoEnum.WasteBasket && x != S_ProductInfoEnum.ShellGameCup)
            .ToList();
        // 셔플
        for (int i = allProducts.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (allProducts[i], allProducts[j]) = (allProducts[j], allProducts[i]); // Fisher–Yates 셔플
        }
        // 상품 뽑기
        var products = allProducts.Take(GetProductCount()).ToList();
        // 상품 생성
        foreach (S_ProductInfoEnum product in products)
        {
            GameObject go = Instantiate(prefab_ProductObj, pos_PaidProductsBase.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<S_ProductObj>().SetProductInfo(product);
            paidProductObjects.Add(go);
        }
        // 상품 정렬
        AlignmentPaidProducts();
    }
    void GenerateNewProductsByTutorial() // 상품 생성
    {
        // 무료 상품
        var freeProducts = new List<S_ProductInfoEnum>() { S_ProductInfoEnum.OracleBall, S_ProductInfoEnum.WasteBasket, S_ProductInfoEnum.ShellGameCup };
        // 상품 생성
        foreach (S_ProductInfoEnum product in freeProducts)
        {
            GameObject go = Instantiate(prefab_ProductObj, pos_FreeProductsBase.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<S_ProductObj>().SetProductInfo(product);
            freeProductObjects.Add(go);
        }
        // 상품 정렬
        AlignmentFreeProducts();

        // 유료 상품
        // 상품 생성
        bool isFree = true; // 상품 1개만 무료로 만들기 위함.
        for (int i = 0; i < GetProductCount(); i++)
        {
            GameObject go = Instantiate(prefab_ProductObj, pos_PaidProductsBase.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<S_ProductObj>().SetProductInfo(S_ProductInfoEnum.OneiroiChisel, isFree);
            paidProductObjects.Add(go);
            isFree = false;
        }
  
        // 상품 정렬
        AlignmentPaidProducts();
    }
    int GetProductCount() // 상품 개수 정하기
    {
        if (S_PlayerTrinket.Instance.IsPlayerHavePassive(S_TrinketPassiveEnum.AddProductCount, out S_Trinket tri))
        {
            return 5;
        }

        return 3;
    }
    public void BuyProduct(S_ProductObj product) // S_ProductObject의 구매 버튼에서 호출
    {
        // 쓸만한 물건이 가득 찼다면 리턴
        if (product.ProductInfo == S_ProductInfoEnum.ThanatosBundle && S_PlayerTrinket.Instance.IsFullTrinket())
        {
            S_DialogInfoSystem.Instance.StartMonolog(sprite_Character, merchantName, S_DialogMetaData.GetStoreMonologData("Store_FullTrinket"), 8);
            return;
        }
        // 제거할 물건이 없어도 리턴
        if (product.ProductInfo == S_ProductInfoEnum.WasteBasket && S_PlayerTrinket.Instance.GetPlayerOwnedTrinkets().Count <= 0)
        {
            S_DialogInfoSystem.Instance.StartMonolog(sprite_Character, merchantName, S_DialogMetaData.GetStoreMonologData("Store_EmptyTrinket"), 8);
            return;
        }

        // 구매할만큼 골드가 있다면 구매
        if (S_PlayerStat.Instance.CurrentGold >= product.Price)
        {
            // 골드 사용
            S_PlayerStat.Instance.AddOrSubtractGold(-product.Price);

            // 현재 구매 중인 상품 설정
            BuiedProduct = product.ProductInfo;
            S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.StoreBuying;

            switch (BuiedProduct)
            {
                case S_ProductInfoEnum.ThanatosBundle: StartSelectOptionTrinkets(3); break;
                case S_ProductInfoEnum.OldMold: StartSelectOptionCards(5, S_CardTypeEnum.None); break;
                case S_ProductInfoEnum.MeltedMold: StartSelectOptionCards(3, S_CardTypeEnum.Str); break;
                case S_ProductInfoEnum.SpiritualMold: StartSelectOptionCards(3, S_CardTypeEnum.Mind); break;
                case S_ProductInfoEnum.BrightMold: StartSelectOptionCards(3, S_CardTypeEnum.Luck); break;
                case S_ProductInfoEnum.DelicateMold: StartSelectOptionCards(3, S_CardTypeEnum.Common); break;
                case S_ProductInfoEnum.GerasBlueprint: StartSelectMyCards(10, S_CardTypeEnum.None); break;
                case S_ProductInfoEnum.ErisDice: StartSelectMyCards(10, S_CardTypeEnum.None); break;
                case S_ProductInfoEnum.HypnosBrush: StartSelectMyCards(10, S_CardTypeEnum.None); break;
                case S_ProductInfoEnum.OneiroiChisel: StartSelectMyCards(10, S_CardTypeEnum.None); break;
                case S_ProductInfoEnum.PlutoChisel: StartSelectFilpEngravingCards(); break;

                case S_ProductInfoEnum.OracleBall: break;
                case S_ProductInfoEnum.WasteBasket: StartSelectMyTrinkets(); break;
                case S_ProductInfoEnum.ShellGameCup: StartSelectMyTrinkets(); break;
            }

            if (BuiedProduct != S_ProductInfoEnum.OracleBall)
            {
                DisappearProductsBase();
                DisappearRefreshAndExitBtn();
            }

            AppearMonlogByBuiedProduct(); 

            // 예지구슬이면 구매가 안 되니까 None으로
            if (BuiedProduct == S_ProductInfoEnum.OracleBall)
            {
                BuiedProduct = S_ProductInfoEnum.None;
            }
        }
        else // 골드가 없다면
        {
            S_DialogInfoSystem.Instance.StartMonolog(sprite_Character, merchantName, S_DialogMetaData.GetStoreMonologData("Store_NotEnoughGold"), 8);
        }
    }
    #endregion
    #region 카드 선택 상품
    public void StartSelectOptionCards(int count, S_CardTypeEnum cardType)
    {
        for (int i = 0; i < count; i++)
        {
            GenerateOptionCardObj(S_CardManager.Instance.GenerateRandomCard(cardType));
        }

        AlignmentOptionCards();
    }
    public void StartSelectMyCards(int count, S_CardTypeEnum cardType)
    {
        List<S_Card> cards = S_PlayerCard.Instance.GetDeckCards();

        if (cards.Count > count)
        {
            cards = cards.OrderBy(x => Random.value).Take(count).ToList(); 
        }
        
        foreach (S_Card card in cards)
        {
            GenerateOptionCardObj(card);
        }

        AlignmentMyCards();
    }
    public void StartSelectFilpEngravingCards()
    {
        foreach (S_Card card in S_PlayerCard.Instance.GetDeckCards())
        {
            if (S_CardEffectMetadata.CanFlipEngraving.Contains(card.Engraving))
            {
                GenerateOptionCardObj(card);
            }
        }

        AlignmentMyCards();
    }
    public void GenerateOptionCardObj(S_Card card) // 옵션 카드 생성
    {
        // 카드 생성
        GameObject go = Instantiate(prefab_OptionCardObj, pos_OptionsBase.transform);
        go.GetComponent<S_OptionCardObj>().SetCardInfo(card);
        go.transform.localPosition = Vector3.zero;

        // 리스트에 추가
        optionObjs.Add(go);
    }
    public async void DecideSelectCard(S_Card card) // 옵션 선택 버튼에서 호출
    {
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
        showingObj.transform.SetParent(pos_OptionsBase.transform);
        showingObj.transform.localPosition = obj.transform.localPosition;
        showingObj.transform.eulerAngles = obj.transform.eulerAngles;
        showingObj.transform.localScale = obj.transform.localScale;
        // 옵션 제거
        DestroyOptionObjs();
        // 가운데로 이동
        await showingObj.GetComponent<S_ShowingCardObj>().MoveShowingPos();
  

        // 단순 획득이면 바로얻기
        if (BuiedProduct == S_ProductInfoEnum.OldMold || BuiedProduct == S_ProductInfoEnum.MeltedMold || BuiedProduct == S_ProductInfoEnum.SpiritualMold ||
            BuiedProduct == S_ProductInfoEnum.BrightMold || BuiedProduct == S_ProductInfoEnum.DelicateMold)
        {
            S_PlayerCard.Instance.AddCard(card);
            await showingObj.GetComponent<S_ShowingCardObj>().MoveDeckPos();
        }
        else // 효과나 각인 변경이면 추가 효과 후 얻기
        {
            switch (BuiedProduct)
            {
                case S_ProductInfoEnum.ErisDice:
                    S_CardManager.Instance.ChangeCardEffect(card);
                    await showingObj.GetComponent<S_ShowingCardObj>().ChangeCardEffect();
                    break;
                case S_ProductInfoEnum.GerasBlueprint:
                    S_CardManager.Instance.ChangeSameTypeCardEffect(card);
                    await showingObj.GetComponent<S_ShowingCardObj>().ChangeCardEffect();
                    break;
                case S_ProductInfoEnum.HypnosBrush:
                    S_CardManager.Instance.DeleteEngraving(card);
                    await showingObj.GetComponent<S_ShowingCardObj>().ChangeEngraving();
                    break;
                case S_ProductInfoEnum.OneiroiChisel:
                    S_CardManager.Instance.GrantEngraving(card);
                    await showingObj.GetComponent<S_ShowingCardObj>().ChangeEngraving();
                    break;
                case S_ProductInfoEnum.PlutoChisel:
                    S_CardManager.Instance.FlipEngraving(card);
                    await showingObj.GetComponent<S_ShowingCardObj>().ChangeEngraving();
                    break;
            }

            await showingObj.GetComponent<S_ShowingCardObj>().MoveDeckPos();
        }
        S_DeckInfoSystem.Instance.SetDeckCardInfo();

        // 각종 UI를 구매 전으로, BuiedProduct 초기화, GameFlowState 되돌리기
        EndDecide(); 
    }
    #endregion
    #region 쓸만한 물건 선택 상품
    public void StartSelectOptionTrinkets(int count) // 옵션 능력 선택용
    {
        List<S_Trinket> tris = S_TrinketList.PickRandomSkills(count);

        // 상품 전리품 생성
        foreach (S_Trinket tri in tris)
        {
            GenerateOptionTrinketObj(tri);
        }

        // 정렬
        AlignmentOptionTrinkets();
    }
    public void StartSelectMyTrinkets() // 내 능력 선택용
    {
        List<S_Trinket> tris = S_PlayerTrinket.Instance.GetPlayerOwnedTrinkets();

        // 상품 전리품 생성
        for (int i = 0; i < tris.Count; i++)
        {
            GenerateOptionTrinketObj(tris[i]);
        }

        // 정렬
        AlignmentMyTrinkets();
    }
    public void GenerateOptionTrinketObj(S_Trinket tri) // 옵션 쓸만한 물건 생성
    {
        // 카드 생성
        GameObject go = Instantiate(prefab_OptionTrinketObj, pos_OptionsBase.transform);
        go.GetComponent<S_OptionTrinketObj>().SetTrinketInfo(tri);
        go.transform.localPosition = Vector3.zero;

        // 리스트에 추가
        optionObjs.Add(go);
    }
    public void DecideTrinketOption(S_Trinket tri) // 옵션 선택 버튼에서 호출
    {
        GameObject obj = null;
        foreach (GameObject go in optionObjs)
        {
            if (go.GetComponent<S_OptionTrinketObj>().TrinketInfo == tri)
            {
                obj = go;
                break;
            }
        }

        switch (BuiedProduct) // pos 넘길 때 worldPos로
        {
            case S_ProductInfoEnum.ThanatosBundle:
                S_PlayerTrinket.Instance.AddTrinket(tri, obj.transform.position);
                break;
            case S_ProductInfoEnum.WasteBasket:
                S_PlayerTrinket.Instance.RemoveTrinket(tri);
                break;
            case S_ProductInfoEnum.ShellGameCup:
                S_PlayerTrinket.Instance.SwapLeftTrinketIndex(tri);
                S_TrinketInfoSystem.Instance.SwapLeftTrinketObjIndex(tri);
                break;
        }
        S_TrinketInfoSystem.Instance.UpdateTrinketObjState();

        DestroyOptionObjs();
        EndDecide();
    }
    #endregion
    #region 버튼 함수
    public void ClickRefresh()
    {
        if (S_PlayerStat.Instance.CurrentGold >= refreshGold)
        {
            // 골드 사용
            S_PlayerStat.Instance.AddOrSubtractGold(-refreshGold);

            // 새로고침 비용 증가
            refreshGold += refreshGoldIncreaseValue;
            text_RefreshBtn.text = $"새로고침 : {refreshGold} 골드";

            // 기존에 있던 상품 제거
            foreach (GameObject go in freeProductObjects)
            {
                Destroy(go);
            }
            freeProductObjects.Clear();
            foreach (GameObject go in paidProductObjects)
            {
                Destroy(go);
            }
            paidProductObjects.Clear();

            // 구매한 상품 비활성화
            BuiedProduct = S_ProductInfoEnum.None;

            // 새로운 상품 등록
            GenerateNewProducts();
        }
        else
        {
            S_DialogInfoSystem.Instance.StartMonolog(instance_Section.GetComponent<S_SectionObj>().sprite_Character.GetComponent<SpriteRenderer>(), "데달로스", S_DialogMetaData.GetStoreMonologData("Daedalus_NotEnoughGold"), 8);
        }
    }
    public async void ClickExit()
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.None;

        // UI 연출 및 캐릭터 들어가기
        DisappearProductsBase();
        DisappearRefreshAndExitBtn();
        DisappearMonologByBuiedProduct();
        await instance_Section.GetComponent<S_SectionObj>().ExitCharacter();

        // 기존에 있던 상품 제거
        foreach (GameObject go in freeProductObjects)
        {
            Destroy(go);
        }
        freeProductObjects.Clear();
        foreach (GameObject go in paidProductObjects)
        {
            Destroy(go);
        }
        paidProductObjects.Clear();
        BuiedProduct = S_ProductInfoEnum.None;

        // 상점 섹션 퇴장과 동시에 적 섹션 등장
        instance_Section.GetComponent<S_SectionObj>().ExitSection();
        S_GameFlowManager.Instance.StartTrial(); // 적 섹션 등장 포함.
    }
    #endregion
    #region 보조 함수
    void DestroyOptionObjs() // 옵션 모두 제거
    {
        foreach (GameObject go in optionObjs)
        {
            Sequence seq = DOTween.Sequence();

            seq.AppendCallback(() =>
            {
                if (go.TryGetComponent<S_TrinketObj>(out var trinket))
                {
                    trinket.SetAlphaValue(0, S_GameFlowManager.PANEL_APPEAR_TIME);
                }
                else if (go.TryGetComponent<S_DeckCardObj>(out var card))
                {
                    card.SetAlphaValue(0, S_GameFlowManager.PANEL_APPEAR_TIME);
                }
            })
                .AppendInterval(S_GameFlowManager.PANEL_APPEAR_TIME)
                .OnComplete(() => Destroy(go));
        }
        optionObjs.Clear();
    }
    void EndDecide() // 각종 UI를 구매 전으로, BuiedProduct 초기화, GameFlowState 되돌리기
    {
        AppearProductsBase();
        AppearRefreshAndExitBtn();
        DisappearMonologByBuiedProduct();

        BuiedProduct = S_ProductInfoEnum.None;
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Store;
    }  
    int GetSelectCount(S_ProductInfoEnum product) // 나중에 2개 이상 선택할 게 나오면 ㄱㄱ
    {
        return 1;
    }
    public void GenerateMonologByHoverProduct(S_ProductInfoEnum product) // 포인터 엔터 시 독백 출력
    {
        StringBuilder sb = new();

        // 상품 설명 추가
        sb.Append(S_ProductMetaData.GetProductDescription(product));

        // 예지 구슬이라면
        if (product == S_ProductInfoEnum.OracleBall)
        {
            (S_Foe, int) nextFoe = S_FoeManager.Instance.PeekNextFoe();

            int health = nextFoe.Item2;

            if (S_GameFlowManager.Instance.CurrentTrial % 9 == 8)
            {
                sb.Append($"\n다음은 세 여신 중 한 분이신 {nextFoe.Item1.Name}이십니다. 체력은 {health}며 {S_DialogMetaData.GetStoreMonologData($"Store_{nextFoe.Item1.Name}")} 부디 여신의 시련도 그대의 강한 의지로 버텨낼 수 있기를...");
            }
            else
            {
                sb.Append($"\n다음 적은 {nextFoe.Item1.Name}입니다. 체력은 {health}며 {S_DialogMetaData.GetStoreMonologData($"Store_{nextFoe.Item1.Name}")} 그리고 다음 여신까지 {8 - (S_GameFlowManager.Instance.CurrentTrial % 9)}개의 시련이 남아있습니다.");
            }
        }

        S_DialogInfoSystem.Instance.StartMonolog(sprite_Character, merchantName, sb.ToString(), 9999);
    }

    void AlignmentFreeProducts() // 상품 정렬
    {
        // 무료 상품 정렬
        List<PRS> originCardPRS1 = SetObjectsPos(freeProductObjects.Count, FREE_PRODUCTS_START_POS, FREE_PRODUCTS_END_POS);

        for (int i = 0; i < freeProductObjects.Count; i++)
        {
            // 위치 설정
            freeProductObjects[i].GetComponent<S_ProductObj>().OriginPRS = originCardPRS1[i];

            // 소팅오더 설정
            freeProductObjects[i].GetComponent<S_ProductObj>().OriginOrder = (i + 1) * 10;
            freeProductObjects[i].GetComponent<S_ProductObj>().SetOrder(freeProductObjects[i].GetComponent<S_ProductObj>().OriginOrder);

            freeProductObjects[i].transform.DOLocalMove(freeProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Pos, 0);
            freeProductObjects[i].transform.DOScale(freeProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Scale, 0);
        }
    }
    void AlignmentPaidProducts() // 상품 정렬
    {
        // 유료 상품 정렬
        List<PRS> originCardPRS2 = SetObjectsPos(paidProductObjects.Count, PAID_PRODUCTS_START_POS, PAID_PRODUCTS_END_POS);

        for (int i = 0; i < paidProductObjects.Count; i++)
        {
            // 위치 설정
            paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS = originCardPRS2[i];

            // 소팅오더 설정
            paidProductObjects[i].GetComponent<S_ProductObj>().OriginOrder = (i + 1) * 10;
            paidProductObjects[i].GetComponent<S_ProductObj>().SetOrder(paidProductObjects[i].GetComponent<S_ProductObj>().OriginOrder);

            paidProductObjects[i].transform.DOLocalMove(paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Pos, 0);
            paidProductObjects[i].transform.DOScale(paidProductObjects[i].GetComponent<S_ProductObj>().OriginPRS.Scale, 0);
        }
    }
    void AlignmentOptionTrinkets() // 쓸만한 물건 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(optionObjs.Count, OPTION_TRINKET_START_POS, OPTION_TRINKET_END_POS);

        for (int i = 0; i < optionObjs.Count; i++)
        {
            // 카드 위치 설정
            optionObjs[i].GetComponent<S_TrinketObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            optionObjs[i].GetComponent<S_TrinketObj>().OriginOrder = (i + 1) * 10;
            optionObjs[i].GetComponent<S_TrinketObj>().SetOrder(optionObjs[i].GetComponent<S_TrinketObj>().OriginOrder);

            // 위치 설정
            optionObjs[i].transform.DOLocalMove(optionObjs[i].GetComponent<S_TrinketObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f).SetEase(Ease.OutQuart);
            optionObjs[i].transform.DOScale(optionObjs[i].GetComponent<S_TrinketObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f).SetEase(Ease.OutQuart);
        }
    }
    void AlignmentMyTrinkets() // 쓸만한 물건 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(optionObjs.Count, MY_TRINKET_START_POS, MY_TRINKET_END_POS);

        for (int i = 0; i < optionObjs.Count; i++)
        {
            // 카드 위치 설정
            optionObjs[i].GetComponent<S_TrinketObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            optionObjs[i].GetComponent<S_TrinketObj>().OriginOrder = (i + 1) * 10;
            optionObjs[i].GetComponent<S_TrinketObj>().SetOrder(optionObjs[i].GetComponent<S_TrinketObj>().OriginOrder);

            // 위치 설정
            optionObjs[i].transform.DOLocalMove(optionObjs[i].GetComponent<S_TrinketObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f).SetEase(Ease.OutQuart);
            optionObjs[i].transform.DOScale(optionObjs[i].GetComponent<S_TrinketObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f).SetEase(Ease.OutQuart);
        }
    }
    void AlignmentOptionCards() // 카드 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(optionObjs.Count, OPTION_CARD_START_POS, OPTION_CARD_END_POS);

        for (int i = 0; i < optionObjs.Count; i++)
        {
            // 카드 위치 설정
            optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS = originCardPRS[i];
            optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Scale = new Vector3(1.35f, 1.35f, 1.35f);

            // 소팅오더 설정
            optionObjs[i].GetComponent<S_DeckCardObj>().OriginOrder = (i + 1) * 10;
            optionObjs[i].GetComponent<S_DeckCardObj>().SetOrder(optionObjs[i].GetComponent<S_DeckCardObj>().OriginOrder);

            // 카드의 위치 설정
            optionObjs[i].transform.DOLocalMove(optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.5f).SetEase(Ease.OutQuart);
            optionObjs[i].transform.DOScale(optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.5f).SetEase(Ease.OutQuart);
        }
    }
    void AlignmentMyCards() // 카드 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(optionObjs.Count, MY_CARD_START_POS, MY_CARD_END_POS);

        for (int i = 0; i < optionObjs.Count; i++)
        {
            // 카드 위치 설정
            optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS = originCardPRS[i];
            optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Scale = new Vector3(1.35f, 1.35f, 1.35f);

            // 소팅오더 설정
            optionObjs[i].GetComponent<S_DeckCardObj>().OriginOrder = (i + 1) * 10;
            optionObjs[i].GetComponent<S_DeckCardObj>().SetOrder(optionObjs[i].GetComponent<S_DeckCardObj>().OriginOrder);

            // 카드의 위치 설정
            optionObjs[i].transform.DOLocalMove(optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.5f).SetEase(Ease.OutQuart);
            optionObjs[i].transform.DOScale(optionObjs[i].GetComponent<S_DeckCardObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.5f).SetEase(Ease.OutQuart);
        }
    }
    List<PRS> SetObjectsPos(int count, Vector3 startPos, Vector3 endPos) // 위치 설정하는 메서드
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
            lerps[0] = 0.35f;
            lerps[1] = 0.65f;
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
            Vector3 scale = PRODUCTS_SCALE_VALUE;
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    #endregion
}

public enum S_StoreSlotEnum
{
    Free,
    Card1,
    Card2,
    Loot,
}
public enum S_ProductInfoEnum
{
    None,
    ThanatosBundle,
    OldMold,
    MeltedMold,
    SpiritualMold,
    BrightMold,
    DelicateMold,
    ErisDice,
    GerasBlueprint,
    HypnosBrush,
    OneiroiChisel,
    PlutoChisel,

    // 무료
    OracleBall,
    WasteBasket,
    ShellGameCup,
}