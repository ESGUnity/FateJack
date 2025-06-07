using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_StoreInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_ProductObject;
    [SerializeField] GameObject prefab_DeckCard;
    [SerializeField] GameObject prefab_StoreCard; // 현재 옵션으로 카드 고르는 상품이 없어서 안 쓰임.
    [SerializeField] GameObject prefab_UICard;
    [SerializeField] GameObject prefab_SkillObject;
    //[SerializeField] GameObject prefab_UISkill; // 이건 실제 Skill인데 ByStore로 효과용으로 만들어도될듯

    [Header("씬 오브젝트")]
    [SerializeField] GameObject sprite_Character_Store;
    [SerializeField] GameObject pos_FreeProductsBase;
    [SerializeField] GameObject pos_ProductsBase;
    [SerializeField] GameObject pos_OptionsBase;
    [SerializeField] GameObject pos_SelectedCardsInDeckBase;
    [SerializeField] SpriteRenderer sprite_BlackBackgroundByStore;

    [Header("컴포넌트")]
    GameObject panel_RefreshAndExitBtnsBase;
    TMP_Text text_SelectCardOrSkill;
    TMP_Text text_RefreshBtn;

    [Header("상품")]
    [HideInInspector] public S_ProductInfoEnum BuiedProduct;
    List<GameObject> freeProductObjects = new();
    List<GameObject> productObjects = new();
    Vector3 productsStartPos = new Vector3(-3f, 0, 0);
    Vector3 productsEndPos = new Vector3(3f, 0, 0);
    int refreshGold;
    int refreshGoldIncreaseValue;
    const int ORIGIN_REFRESH_GOLD = 2;
    const int ORIGIN_REFRESH_GOLD_INCREASE_VALUE = 1;

    [Header("능력관련 상품")]
    List<GameObject> skillOptionObjects = new();
    Vector3 skillOptionStartPos = new Vector3(-5.5f, 0, 0);
    Vector3 skillOptionEndPos = new Vector3(5.5f, 0, 0);

    [Header("카드관련 상품")]
    List<GameObject> selectedCardInDeckObjects = new();
    Vector3 selectedCardInDeckStartPos = new Vector3(-1.25f, 0, 0);
    Vector3 selectedCardInDeckEndPos = new Vector3(1.25f, 0, 0);
    public const float STACK_Z_VALUE = -0.02f;

    Vector2 inStoreTextPos = new Vector2(0, 225);
    Vector2 inDeckTextPos = new Vector2(0, -105);
    Vector2 uICardPos = new Vector2(0, 50);

    [Header("UI")]
    Vector2 refreshAndExitBtnsHidePos = new Vector2(0, -150);
    Vector2 refreshAndExitBtnsOriginPos = new Vector2(0, 85);

    Vector3 freeProductsBaseHidePos = new Vector3(-15f, -1.6f, 0);
    Vector3 freeProductsBaseOriginPos = new Vector3(-5f, -1.6f, 0);
    Vector3 productsBaseHidePos = new Vector3(15f, -1.6f, 0);
    Vector3 productsBaseOriginPos = new Vector3(5f, -1.6f, 0);

    // 싱글턴
    static S_StoreInfoSystem instance;
    public static S_StoreInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        panel_RefreshAndExitBtnsBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_RefreshAndExitBtnsBase")).gameObject;
        text_SelectCardOrSkill = Array.Find(texts, c => c.gameObject.name.Equals("Text_SelectCardOrSkill"));
        text_SelectCardOrSkill.raycastTarget = false;
        text_RefreshBtn = Array.Find(texts, c => c.gameObject.name.Equals("Text_RefreshBtn"));

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

    public async Task StartStore()
    {
        // 상품 생성
        GenerateNewProducts();

        // 새로고침 골드 초기화
        refreshGold = ORIGIN_REFRESH_GOLD;
        refreshGoldIncreaseValue = ORIGIN_REFRESH_GOLD_INCREASE_VALUE;
        text_RefreshBtn.text = $"새로고침\n{refreshGold} 골드";

        // UI 관리
        AppearDaedalusImage();
        AppearProductsBase();
        AppearRefreshAndExitBtn();
        await Task.Delay(Mathf.RoundToInt(S_GameFlowManager.PANEL_APPEAR_TIME * 1000)); // 캔버스 등장 동안 대기

        // 데달로스 상점 조우 대사
        S_DialogInfoSystem.Instance.StartMonologByStore(S_DialogMetaData.GetMonologData("Daedalus_Intro"));
    }
    #region UI
    void InitPos()
    {
        DisappearDaedalusImage();
        DisappearProductsBase();
        DisappearRefreshAndExitBtn();
        DisappearSelectCardOrSkillText();
        DisappearBlackBackground();
    }
    public void AppearDaedalusImage()
    {
        sprite_Character_Store.SetActive(true);

        sprite_Character_Store.GetComponent<SpriteRenderer>().DOKill();
        sprite_Character_Store.GetComponent<SpriteRenderer>().DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME);
    }
    public void DisappearDaedalusImage()
    {
        sprite_Character_Store.GetComponent<SpriteRenderer>().DOKill();
        sprite_Character_Store.GetComponent<SpriteRenderer>().DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME)
            .OnComplete(() => sprite_Character_Store.SetActive(false));
    }
    public void AppearProductsBase()
    {
        pos_FreeProductsBase.SetActive(true);
        pos_ProductsBase.SetActive(true);

        pos_FreeProductsBase.transform.DOKill();
        pos_ProductsBase.transform.DOKill();

        pos_FreeProductsBase.transform.DOLocalMove(freeProductsBaseOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        pos_ProductsBase.transform.DOLocalMove(productsBaseOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearProductsBase()
    {
        pos_FreeProductsBase.transform.DOKill();
        pos_ProductsBase.transform.DOKill();

        pos_FreeProductsBase.transform.DOLocalMove(freeProductsBaseHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => pos_FreeProductsBase.SetActive(false));
        pos_ProductsBase.transform.DOLocalMove(productsBaseHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => pos_ProductsBase.SetActive(false));
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
    public void AppearSelectCardOrSkillText(bool isCardInDeck)
    {
        int selectCount = GetSelectCount(BuiedProduct);

        if (isCardInDeck)
        {
            text_SelectCardOrSkill.text = $"카드를 {selectCount}장 선택해주세요.";
            text_SelectCardOrSkill.GetComponent<RectTransform>().anchoredPosition = inDeckTextPos;
        }
        else
        {
            text_SelectCardOrSkill.text = $"스킬을 {selectCount}개 선택해주세요.";
            text_SelectCardOrSkill.GetComponent<RectTransform>().anchoredPosition = inStoreTextPos;
        }

        text_SelectCardOrSkill.DOKill();
        text_SelectCardOrSkill.DOFade(1f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearSelectCardOrSkillText()
    {
        text_SelectCardOrSkill.DOKill();
        text_SelectCardOrSkill.DOFade(0f, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
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
    #endregion
    #region 상품 관련
    void GenerateNewProducts() // 상품 생성
    {
        // 무료 상품
        var freeProducts = new List<S_ProductInfoEnum>() { S_ProductInfoEnum.OracleBall, S_ProductInfoEnum.ParallizeLight, S_ProductInfoEnum.PostureCorrector };

        // 상품 생성
        foreach (S_ProductInfoEnum product in freeProducts)
        {
            GameObject go = Instantiate(prefab_ProductObject, pos_FreeProductsBase.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<S_ProductObject>().SetProductInfo(product);
            freeProductObjects.Add(go);
        }

        // 유료 상품
        var allProducts = Enum.GetValues(typeof(S_ProductInfoEnum))
            .Cast<S_ProductInfoEnum>()
            .Where(x => x != S_ProductInfoEnum.None && x != S_ProductInfoEnum.OracleBall && x != S_ProductInfoEnum.ParallizeLight && x != S_ProductInfoEnum.PostureCorrector)
            .ToList();

        // 셔플
        for (int i = allProducts.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (allProducts[i], allProducts[j]) = (allProducts[j], allProducts[i]); // Fisher–Yates 셔플
        }

        // 상품 뽑기
        var products = allProducts.Take(GetAppearProductCount()).ToList();

        // 상품 생성
        foreach (S_ProductInfoEnum product in products)
        {
            GameObject go = Instantiate(prefab_ProductObject, pos_ProductsBase.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<S_ProductObject>().SetProductInfo(product);
            productObjects.Add(go);
        }

        // 상품 정렬
        AlignmentProducts();
    }
    int GetAppearProductCount() // 추후 상품 개수 늘리는 능력 생기면 ㄱㄱ
    {
        foreach (var s in S_PlayerSkill.Instance.GetPlayerOwnedSkills())
        {
            if (s.Passive == S_SkillPassiveEnum.AddProductCount)
            {
                return 5;
            }
        }

        return 3;
    }
    int GetSelectCount(S_ProductInfoEnum product) // 나중에 2개 이상 선택할 게 나오면 ㄱㄱ
    {
        return 1;
    }
    public bool BuyProduct(S_ProductObject product) // S_ProductObject의 구매 버튼에서 호출
    {
        // 능력이 가득 찼다면 리턴
        if (product.ProductInfo == S_ProductInfoEnum.StoneOfInsight && !S_PlayerSkill.Instance.CanAddSkill())
        {
            S_InGameUISystem.Instance.CreateLog("능력이 가득 찼습니다!");
            return false;
        }
        if (product.ProductInfo == S_ProductInfoEnum.ParallizeLight && S_PlayerSkill.Instance.GetPlayerOwnedSkills().Count <= 0)
        {
            S_InGameUISystem.Instance.CreateLog("제거할 능력이 없습니다!");
            return false;
        }

        // 구매할만큼 골드가 있다면 구매
        if (S_PlayerStat.Instance.CurrentGold >= product.Price)
        {
            // 골드 사용
            S_PlayerStat.Instance.AddOrSubtractGold(-product.Price);

            // 현재 구매 중인 상품 설정
            BuiedProduct = product.ProductInfo;

            switch (BuiedProduct)
            {
                case S_ProductInfoEnum.StoneOfInsight: StartSelectSkillOptions(3); break;
                case S_ProductInfoEnum.GrandBlueprint: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.DarkRedBrush: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.DiceOfEris: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.AstroTool: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.FingerOfMomus: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.OldLoom: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.SacredSeal: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.CurseOfMoros: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.EmberOfPluto: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.OldNeedle: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.OldScissors: StartSelectCardInDeck(); break;
                case S_ProductInfoEnum.ParallizeLight: StartSelectMySkill(); break;
                case S_ProductInfoEnum.PostureCorrector: StartSelectMySkill(); break;
                default:
                    S_InGameUISystem.Instance.CreateLog("S_StoreInfoSystem Send : Error At BuyProduct");
                    return false;
            }
            return true;
        }
        else // 골드가 없다면
        {
            S_InGameUISystem.Instance.CreateLog("골드가 부족합니다.");
            return false;
        }
    }
    #endregion
    #region 옵션 카드를 선택하는 상품
    //public void StartSelectOptionCards(List<S_Card> cards) // 카드를 추가하는 상품 함수
    //{
    //    // 버튼 변경
    //    DisappearUIByBuyStoreProduct();
    //    AppearUIBySelectItem();
    //    DisappearStoreTextInStore();
    //    AppearSelectCardOrLootTextInStore();
    //    AppearBlackBackground();

    //    // 상품 카드 생성
    //    foreach (S_Card card in cards)
    //    {
    //        GenerateOptionCard(card, optionsBase.transform, itemOptionsList);
    //    }

    //    // 정렬
    //    AlignmentOptions(itemOptionsList);
    //}
    //public void SelectCardByOption(S_Card card) // 옵션 카드 선택 시 호출 
    //{
    //    // 선택한 카드 중에 card가 있는지 검사
    //    bool same = false;
    //    foreach (GameObject go in selectedItemList)
    //    {
    //        if (go.GetComponent<S_StoreCard>().CardInfo == card)
    //        {
    //            same = true;
    //            break;
    //        }
    //        else
    //        {
    //            same = false;
    //        }
    //    }

    //    // card가 없었다면 고른 카드 개수에 따른 처리
    //    if (!same)
    //    {
    //        if (BuiedProduct.SelectCount > selectedItemList.Count)
    //        {
    //            GenerateOptionCard(card, selectedItemBase.transform, selectedItemList);
    //        }
    //        else if (BuiedProduct.SelectCount == selectedItemList.Count) // 골라야하는 개수가 같다면 이전에 선택한 것을 제거하고 추가
    //        {
    //            Destroy(selectedItemList[0]);
    //            selectedItemList.RemoveAt(0);

    //            GenerateOptionCard(card, selectedItemBase.transform, selectedItemList);
    //        }
    //    }

    //    // 선택된 옵션 카드임을 체크
    //    foreach (GameObject go in selectedItemList)
    //    {
    //        go.GetComponent<S_StoreCard>().IsSelectedOptionCard = true;
    //    }

    //    // 정렬
    //    AlignmentOptions(selectedItemList);

    //    // 골라야하는걸 다 골랐는지 체크
    //    if (BuiedProduct.SelectCount == selectedItemList.Count)
    //    {
    //        isSelectRequiredItems = true;
    //    }
    //    else
    //    {
    //        isSelectRequiredItems = false;
    //    }
    //}
    //public void GenerateOptionCard(S_Card card, Transform parent, List<GameObject> list) // 덱에 추가할 카드 선택이나 제외할 카드 선택 시에도 사용 예정
    //{
    //    // 옵션 카드 생성
    //    GameObject go = Instantiate(prefab_StoreCard);
    //    go.GetComponent<S_StoreCard>().SetCardInfo(card);
    //    go.transform.SetParent(parent, true);
    //    go.transform.localPosition = Vector3.zero;

    //    // 리스트에 추가(추후 관리용)
    //    list.Add(go);
    //}
    #endregion
    #region 덱에서 카드 선택하는 상품(S_StoreCard 대신 S_DeckCard 씀)
    public async void StartSelectCardInDeck()
    {
        // 덱 정보 UI 설정
        await S_DeckInfoSystem.Instance.OpenDeckInfoByStore();
    }
    public void SelectCardInDeck(S_Card card, Vector3 cardPos) // S_DeckCard가 호출
    {
        int selectCount = GetSelectCount(BuiedProduct);

        bool same = false;
        foreach (GameObject go in selectedCardInDeckObjects)
        {
            if (go.GetComponent<S_DeckCard>().CardInfo == card)
            {
                same = true;
                CancelSelectCardInDeck(go, cardPos); // 만약 같은 카드를 클릭했다면 선택 취소
                break;
            }
            else
            {
                same = false;
            }
        }

        if (!same)
        {
            if (selectCount > selectedCardInDeckObjects.Count) // 골라야하는 개수가 아직 남았다면 선택한 카드를 그냥 추가
            {
                GenerateSelectCardInDeck(card, cardPos);
            }
            else if (selectCount == selectedCardInDeckObjects.Count) // 골라야하는 개수가 같다면 이전에 선택한 것을 제거하고 추가
            {
                Destroy(selectedCardInDeckObjects[0]);
                selectedCardInDeckObjects.RemoveAt(0);

                GenerateSelectCardInDeck(card, cardPos);
            }
        }

        // 정렬
        AlignmentSelectedCardsInDeck();
    }
    public void GenerateSelectCardInDeck(S_Card card, Vector3 cardPos)
    {
        // 카드 생성
        GameObject go = Instantiate(prefab_DeckCard, pos_SelectedCardsInDeckBase.transform);
        go.GetComponent<S_DeckCard>().SetCardInfo(card);
        go.transform.position = cardPos;

        // 리스트에 추가
        selectedCardInDeckObjects.Add(go);
    }
    public void CancelSelectCardInDeck(GameObject exceptedCard, Vector3 cardPos)
    {
        selectedCardInDeckObjects.Remove(exceptedCard);
        Destroy(exceptedCard);
    }
    public async void DecideSelectCard() // S_DeckInfoSystem의 선택 버튼에서 호출
    {
        int selectCount = GetSelectCount(BuiedProduct);

        if (selectCount == selectedCardInDeckObjects.Count)
        {
            // 덱 정보 UI 설정
            await S_DeckInfoSystem.Instance.ClosetDeckInfoByStore();

            S_Card card = selectedCardInDeckObjects[0].GetComponent<S_DeckCard>().CardInfo;

            GameObject go = Instantiate(prefab_UICard, transform);
            go.GetComponent<S_UICard>().SetCardInfo(card);

            switch (BuiedProduct)
            {
                case S_ProductInfoEnum.GrandBlueprint:
                    S_CardManager.Instance.ChangeAllProperty(card);
                    break;
                case S_ProductInfoEnum.DarkRedBrush:
                    S_CardManager.Instance.ChangeAnotherSuit(card);
                    break;
                case S_ProductInfoEnum.DiceOfEris:
                    S_CardManager.Instance.ChangeAnotherNumber(card);
                    break;
                case S_ProductInfoEnum.AstroTool:
                    S_CardManager.Instance.ChangeAllCondition(card);
                    break;
                case S_ProductInfoEnum.FingerOfMomus:
                    S_CardManager.Instance.ChangeAllEffect(card);
                    break;
                case S_ProductInfoEnum.OldLoom:
                    S_CardManager.Instance.ChangeBasicCondition(card);
                    S_CardManager.Instance.ChangedDebuffCondition(card);
                    S_CardManager.Instance.ChangeAllEffect(card);
                    break;
                case S_ProductInfoEnum.SacredSeal:
                    S_CardManager.Instance.ChangeAdditiveCondition(card);
                    S_CardManager.Instance.ChangedDebuffCondition(card);
                    S_CardManager.Instance.ChangeAllEffect(card);
                    break;
                case S_ProductInfoEnum.CurseOfMoros:
                    S_CardManager.Instance.ChangeBasicCondition(card);
                    S_CardManager.Instance.ChangeAdditiveCondition(card);
                    S_CardManager.Instance.AddDebuffCondition(card);
                    S_CardManager.Instance.ChangeAllEffect(card);
                    break;
                case S_ProductInfoEnum.EmberOfPluto:
                    S_CardManager.Instance.ChangeBasicCondition(card);
                    S_CardManager.Instance.ChangeAdditiveCondition(card);
                    S_CardManager.Instance.DeleteDebuffCondition(card);
                    S_CardManager.Instance.ChangeAllEffect(card);
                    break;
                case S_ProductInfoEnum.OldNeedle:
                    S_CardManager.Instance.ChangeBasicEffect(card);
                    S_CardManager.Instance.ChangeAllCondition(card);
                    break;
                case S_ProductInfoEnum.OldScissors:
                    S_CardManager.Instance.ChangeAdditiveEffect(card);
                    S_CardManager.Instance.ChangeAllCondition(card);
                    break;
            }

            go.GetComponent<S_UICard>().ChangeCardVFXByStore(card);

            // 선택 카드 지우기
            foreach (GameObject go1 in selectedCardInDeckObjects)
            {
                Sequence seq = DOTween.Sequence();

                seq.AppendCallback(() => go1.GetComponent<S_DeckCard>().SetAlphaValue(0, S_GameFlowManager.PANEL_APPEAR_TIME))
                    .AppendInterval(S_GameFlowManager.PANEL_APPEAR_TIME)
                    .OnComplete(() => Destroy(go1));
            }
            selectedCardInDeckObjects.Clear();

            BuiedProduct = S_ProductInfoEnum.None;

            S_DeckInfoSystem.Instance.SetDeckCardInfo();
        }
        else
        {
            S_InGameUISystem.Instance.CreateLog("먼저 카드를 선택해주세요!");
        }
    }
    #endregion
    #region 옵션 능력을 선택하는 상품
    public void StartSelectSkillOptions(int count) // 옵션 능력 선택용
    {
        // 상품 숨기고 새로고침도 숨기고 검은 화면 등장
        DisappearDaedalusImage();
        DisappearProductsBase();
        DisappearRefreshAndExitBtn();
        AppearSelectCardOrSkillText(false);
        AppearBlackBackground();

        List<S_Skill> skills = S_SkillList.Instance.PickRandomSkills(count);

        // 상품 전리품 생성
        foreach (S_Skill skill in skills)
        {
            GenerateSkillOption(skill);
        }

        // 정렬
        AlignmentSkillOptions();
    }
    public void StartSelectMySkill() // 내 능력 선택용
    {
        // 상품 숨기고 새로고침도 숨기고 검은 화면 등장
        DisappearDaedalusImage();
        DisappearProductsBase();
        DisappearRefreshAndExitBtn();
        AppearSelectCardOrSkillText(false);
        AppearBlackBackground();

        List<S_Skill> skills = S_PlayerSkill.Instance.GetPlayerOwnedSkills();

        // 상품 전리품 생성
        for (int i = 0; i < skills.Count; i++)
        {
            GenerateSkillOption(skills[i]);
        }

        // 정렬
        AlignmentSkillOptions();
    }
    public void GenerateSkillOption(S_Skill skill) // 능력 옵션 생성
    {
        // 카드 생성
        GameObject go = Instantiate(prefab_SkillObject, pos_OptionsBase.transform);
        go.GetComponent<S_SkillObject>().SetSkillInfo(skill);
        go.transform.localPosition = Vector3.zero;

        // 리스트에 추가
        skillOptionObjects.Add(go);
    }
    public void DecideSkillOption(S_Skill skill) // S_SKillObject의 선택 버튼에서 호출
    {
        switch(BuiedProduct)
        {
            case S_ProductInfoEnum.ParallizeLight:
                S_PlayerSkill.Instance.RemoveSkill(skill);
                break;
            case S_ProductInfoEnum.PostureCorrector:
                S_PlayerSkill.Instance.ChangeSkillIndexToFirst(skill);
                break;
            case S_ProductInfoEnum.StoneOfInsight:
                S_PlayerSkill.Instance.AddSkill(skill);
                break;
        }
        S_SkillInfoSystem.Instance.UpdateSkillObject();

        // 옵션 비우기
        foreach (GameObject go in skillOptionObjects)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => go.GetComponent<S_SkillObject>().SetAlphaValue(0, S_GameFlowManager.PANEL_APPEAR_TIME))
                .AppendInterval(S_GameFlowManager.PANEL_APPEAR_TIME)
                .OnComplete(() => Destroy(go));
        }
        skillOptionObjects.Clear();

        AppearDaedalusImage();
        AppearProductsBase();
        AppearRefreshAndExitBtn();
        DisappearSelectCardOrSkillText();
        DisappearBlackBackground();

        BuiedProduct = S_ProductInfoEnum.None;
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
            text_RefreshBtn.text = $"새로고침\n{refreshGold} 골드";

            // 기존에 있던 상품 제거
            foreach (GameObject go in freeProductObjects)
            {
                Destroy(go);
            }
            freeProductObjects.Clear();
            foreach (GameObject go in productObjects)
            {
                Destroy(go);
            }
            productObjects.Clear();

            // 구매한 상품 비활성화
            BuiedProduct = S_ProductInfoEnum.None;

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
        DisappearDaedalusImage();
        DisappearProductsBase();
        DisappearRefreshAndExitBtn();
        DisappearSelectCardOrSkillText();
        DisappearBlackBackground();

        // 기존에 있던 상품 제거
        foreach (GameObject go in freeProductObjects)
        {
            Destroy(go);
        }
        freeProductObjects.Clear();
        foreach (GameObject go in productObjects)
        {
            Destroy(go);
        }
        productObjects.Clear();

        BuiedProduct = S_ProductInfoEnum.None;

        S_GameFlowManager.Instance.StartTrial();
    }
    #endregion
    #region 보조 함수
    void AlignmentProducts() // 상품 정렬
    {
        // 무료 상품 정렬
        List<PRS> originCardPRS1 = SetObjectsPos(freeProductObjects.Count, productsStartPos, productsEndPos);

        for (int i = 0; i < freeProductObjects.Count; i++)
        {
            // 위치 설정
            freeProductObjects[i].GetComponent<S_ProductObject>().OriginPRS = originCardPRS1[i];

            // 소팅오더 설정
            freeProductObjects[i].GetComponent<S_ProductObject>().OriginOrder = (i + 1) * 10;
            freeProductObjects[i].GetComponent<S_ProductObject>().SetOrder(freeProductObjects[i].GetComponent<S_ProductObject>().OriginOrder);

            freeProductObjects[i].transform.DOLocalMove(freeProductObjects[i].GetComponent<S_ProductObject>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f);
            freeProductObjects[i].transform.DOScale(freeProductObjects[i].GetComponent<S_ProductObject>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f);
        }

        // 유료 상품 정렬
        List<PRS> originCardPRS2 = SetObjectsPos(productObjects.Count, productsStartPos, productsEndPos);

        for (int i = 0; i < productObjects.Count; i++)
        {
            // 위치 설정
            productObjects[i].GetComponent<S_ProductObject>().OriginPRS = originCardPRS2[i];

            // 소팅오더 설정
            productObjects[i].GetComponent<S_ProductObject>().OriginOrder = (i + 1) * 10;
            productObjects[i].GetComponent<S_ProductObject>().SetOrder(productObjects[i].GetComponent<S_ProductObject>().OriginOrder);

            productObjects[i].transform.DOLocalMove(productObjects[i].GetComponent<S_ProductObject>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f);
            productObjects[i].transform.DOScale(productObjects[i].GetComponent<S_ProductObject>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f);
        }
    }
    void AlignmentSkillOptions() // 각종 옵션(카드나 기술) 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(skillOptionObjects.Count, skillOptionStartPos, skillOptionEndPos);

        for (int i = 0; i < skillOptionObjects.Count; i++)
        {
            // 카드 위치 설정
            skillOptionObjects[i].GetComponent<S_SkillObject>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            skillOptionObjects[i].GetComponent<S_SkillObject>().OriginOrder = (i + 1) * 10;
            skillOptionObjects[i].GetComponent<S_SkillObject>().SetOrder(skillOptionObjects[i].GetComponent<S_SkillObject>().OriginOrder);

            // 위치 설정
            skillOptionObjects[i].transform.DOLocalMove(skillOptionObjects[i].GetComponent<S_SkillObject>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f).SetEase(Ease.OutQuart);
            skillOptionObjects[i].transform.DOScale(skillOptionObjects[i].GetComponent<S_SkillObject>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.2f).SetEase(Ease.OutQuart);
        }
    }
    void AlignmentSelectedCardsInDeck() // 덱에서 고른 카드 정렬
    {
        List<PRS> originCardPRS = SetObjectsPos(selectedCardInDeckObjects.Count, selectedCardInDeckStartPos, selectedCardInDeckEndPos);

        for (int i = 0; i < selectedCardInDeckObjects.Count; i++)
        {
            // 카드 위치 설정
            selectedCardInDeckObjects[i].GetComponent<S_DeckCard>().OriginPRS = originCardPRS[i];
            selectedCardInDeckObjects[i].GetComponent<S_DeckCard>().OriginPRS.Scale = new Vector3(1.35f, 1.35f, 1.35f);

            // 소팅오더 설정
            selectedCardInDeckObjects[i].GetComponent<S_DeckCard>().OriginOrder = (i + 1) * 10;
            selectedCardInDeckObjects[i].GetComponent<S_DeckCard>().SetOrder(selectedCardInDeckObjects[i].GetComponent<S_DeckCard>().OriginOrder);

            // 카드의 위치 설정
            selectedCardInDeckObjects[i].transform.DOLocalMove(selectedCardInDeckObjects[i].GetComponent<S_DeckCard>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.5f).SetEase(Ease.OutQuart);
            selectedCardInDeckObjects[i].transform.DOScale(selectedCardInDeckObjects[i].GetComponent<S_DeckCard>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime() * 0.5f).SetEase(Ease.OutQuart);
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
            Vector3 scale = new Vector3(1, 1, 1);
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void GenerateMonologByPointerEnter(S_ProductInfoEnum product)
    {
        string monolog = S_DialogMetaData.GetMonologData($"Daedalus_{product}");

        if (product == S_ProductInfoEnum.OracleBall)
        {
            (S_Foe, int) nextFoe = S_FoeManager.Instance.PeekNextFoe();

            string des = nextFoe.Item1.AbilityDescription;
            int health = nextFoe.Item2;

            if (S_GameFlowManager.Instance.CurrentTrial % 9 == 8)
            {
                monolog = $"{monolog}\n다음은 {nextFoe.Item1.Name}입니다.\n{health}의 체력에\n{des}의 권능을 부리십니다...\n부디 여신의 시련도 그대의 강한 의지로 버텨낼 수 있기를...";
            }
            else
            {
                monolog = $"{monolog}\n다음 적은 {nextFoe.Item1.Name}.\n체력은 {health}며\n능력은 {des} 군요..\n그리고 여신을 알현하기까지 {8 - (S_GameFlowManager.Instance.CurrentTrial % 9)}개의 시련이 남아있습니다.";
            }
        }

        S_DialogInfoSystem.Instance.StartMonologByStore(monolog, 9999);
    }
    public void GenerateMonologByBuyOracleBall()
    {
        string monolog = "그건 팔지 않습니다!\n사실 굉장히 위험한 물건이거든요..\n죄송합니다. 다른 물건은 얼마든지 가져가세요!\n대신 돈은 주시구요!";

        S_DialogInfoSystem.Instance.StartMonologByStore(monolog, 9999);
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
    StoneOfInsight,
    GrandBlueprint,
    DarkRedBrush,
    DiceOfEris,
    AstroTool,
    FingerOfMomus,
    OldLoom,
    SacredSeal,
    CurseOfMoros,
    EmberOfPluto,
    OldNeedle,
    OldScissors,

    // 무료
    OracleBall,
    ParallizeLight,
    PostureCorrector,
}