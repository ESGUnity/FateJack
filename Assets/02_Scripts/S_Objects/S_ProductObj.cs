using DG.Tweening;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_ProductObj : MonoBehaviour // TODO : CardObj처럼 꼼꼼하게 만들기. 버튼은 클릭하면 나오게 하기
{
    [Header("주요 정보")]
    [HideInInspector] public S_ProductInfoEnum ProductInfo;
    [HideInInspector] public int Price;

    [Header("컴포넌트")]
    [SerializeField] SpriteRenderer sprite_Product;
    [SerializeField] TMP_Text text_Name;
    [SerializeField] TMP_Text text_Price;

    [SerializeField] SpriteRenderer sprite_BuyBtn;
    [SerializeField] TMP_Text text_Buy;

    [Header("VFX")]
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;
    const float POINTER_ENTER_ANIMATION_TIME = 0.15f;
    Vector3 POINTER_ENTER_SCALE_VALUE = new Vector3(0.3f, 0.3f, 0.3f);
    Vector3 CLICK_SCALE_VALUE = new Vector3(0.3f, 0.3f, 0.3f);

    [Header("포인터 연출")]
    bool isEnter = false;
    List<S_GameFlowStateEnum> VALID_STATES;

    void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Store };
    }
    void Start()
    {
        // 스프라이트 바인딩
        EventTrigger spriteTrigger = sprite_Product.GetComponent<EventTrigger>();
        EventTrigger.Entry spritePointerEnterEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);
        EventTrigger.Entry spritePointerExitEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerExit);
        EventTrigger.Entry spritePointerClickEntry = spriteTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
        // 함수 바인딩 추가
        spritePointerEnterEntry.callback.AddListener((eventData) => { PointerEnterProductSprite((PointerEventData)eventData); });
        spritePointerExitEntry.callback.AddListener((eventData) => { PointerExitProductSprite((PointerEventData)eventData); });
        spritePointerClickEntry.callback.AddListener((eventData) => { PointerClickProductSprite((PointerEventData)eventData); });

        // 버튼 바인딩
        EventTrigger btnTrigger = sprite_BuyBtn.GetComponent<EventTrigger>();
        EventTrigger.Entry btnPointerClickEntry = btnTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
        // 함수 바인딩 추가
        btnPointerClickEntry.callback.AddListener((eventData) => { PointerClickBuyBtn((PointerEventData)eventData); });
    }
    void Update()
    {
        // Enter 상태인데 상태가 유효하지 않게 바뀌면 강제로 Exit
        if (isEnter && !S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            ForceExit();
        }
    }
    void OnDisable()
    {
        ForceExit();
    }

    public void SetProductInfo(S_ProductInfoEnum product, bool isTutorial = false)
    {
        // 상품 설정
        ProductInfo = product;

        // 이름 설정
        text_Name.text = S_ProductMetaData.GetName(product);

        // 가격 설정
        Price = S_ProductMetaData.GetPrice(product);
        if (Price == 0)
        {
            if (ProductInfo == S_ProductInfoEnum.OracleBall)
            {
                text_Price.text = "비매품!";
            }
            else
            {
                text_Price.text = "무료!";
            }
        }
        else
        {
            text_Price.text = $"{Price} 골드";
        }

        if (isTutorial)
        {
            Price = 0;
            text_Price.text = "무료!";
        }

        // 상품 스프라이트
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{ProductInfo}");
        cardEffectOpHandle.Completed += OnProductLoadComplete;

        // 소팅오더 설정
        SetOrder(1);
    }
    void OnProductLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_Product.sprite = opHandle.Result;
        }
    }
    public void SetOrder(int order) // 상품의 오더 설정
    {
        sprite_Product.sortingLayerName = "WorldObject";
        sprite_Product.sortingOrder = order;

        text_Name.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_Name.GetComponent<MeshRenderer>().sortingOrder = order + 1;

        text_Price.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_Price.GetComponent<MeshRenderer>().sortingOrder = order + 1;

        sprite_BuyBtn.sortingLayerName = "WorldObject";
        sprite_BuyBtn.sortingOrder = order + 1;

        text_Buy.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_Buy.GetComponent<MeshRenderer>().sortingOrder = order + 2;
    }
    #region 포인터 함수
    public void PointerEnterProductSprite(BaseEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            sprite_Product.transform.DOKill();

            sprite_Product.transform.DOScale(OriginPRS.Scale + POINTER_ENTER_SCALE_VALUE, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_StoreInfoSystem.Instance.GenerateMonologByHoverProduct(ProductInfo);

            isEnter = true;
        }
    }
    public void PointerExitProductSprite(BaseEventData eventData)
    {
        ForceExit();
    }
    public void ForceExit()
    {
        if (!isEnter) return;

        sprite_Product.transform.DOKill();

        sprite_Product.transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_DialogInfoSystem.Instance.EndMonolog();

        isEnter = false;
    }
    public void PointerClickProductSprite(BaseEventData eventData) // 클릭 시 버튼 생김
    {
        if (sprite_BuyBtn.gameObject.activeInHierarchy)
        {
            sprite_Product.transform.DOKill();
            sprite_Product.transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            sprite_BuyBtn.gameObject.SetActive(false);
        }
        else
        {
            sprite_Product.transform.DOKill();
            sprite_Product.transform.DOScale(OriginPRS.Scale + CLICK_SCALE_VALUE, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            sprite_BuyBtn.gameObject.SetActive(true);
        }
    }
    public void PointerClickBuyBtn(BaseEventData eventData)
    {
        S_StoreInfoSystem.Instance.BuyProduct(this);
    }
    #endregion
}
