using DG.Tweening;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_ProductObj : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public S_ProductEnum ProductInfo;

    [Header("컴포넌트")]
    [SerializeField] SpriteRenderer sprite_Product;

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
        spritePointerEnterEntry.callback.AddListener((eventData) => { PointerEnterProductSprite((PointerEventData)eventData); });
        spritePointerExitEntry.callback.AddListener((eventData) => { PointerExitProductSprite((PointerEventData)eventData); });
        spritePointerClickEntry.callback.AddListener((eventData) => { PointerClickProductSprite((PointerEventData)eventData); });
    }
    void Update()
    {
        // Enter 상태인데 상태가 유효하지 않게 바뀌면 강제로 Exit
        if (isEnter && !S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            ForceExit();

            isEnter = false;
        }
    }
    void OnDisable()
    {
        ForceExit();
    }

    public void SetProductInfo(S_ProductEnum product, bool isTutorial = false)
    {
        // 상품 설정
        ProductInfo = product;

        // 상품 스프라이트
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Product_{ProductInfo}");
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
    }
    #region 포인터 함수
    public void PointerEnterProductSprite(BaseEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            transform.DOKill();

            transform.DOScale(OriginPRS.Scale + POINTER_ENTER_SCALE_VALUE, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            // 호버링 시 뜨는 모놀로그
            DialogData dialog = S_DialogMetaData.GetMonologs($"Reward_{ProductInfo}");
            S_DialogInfoSystem.Instance.StartMonolog(dialog.Name, dialog.Dialog, 9999);

            // 사운드
            S_AudioManager.Instance.PlaySFX(SFXEnum.CardHovering);

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

        transform.DOKill();

        transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_DialogInfoSystem.Instance.EndMonolog();

        isEnter = false;
    }
    public void PointerClickProductSprite(BaseEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            ForceExit();

            S_RewardInfoSystem.Instance.BuyProduct(this);
        }
    }
    #endregion
}
