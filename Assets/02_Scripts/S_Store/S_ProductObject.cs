using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_ProductObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // 상품 정보
    [HideInInspector] public S_Product ThisProduct;
    [HideInInspector] public S_StoreSlotEnum SlotInfo;

    // 컴포넌트
    [SerializeField] SpriteRenderer sprite_Product;
    [SerializeField] TMP_Text text_Description;
    [SerializeField] TMP_Text text_Price;

    // 애님 관련
    const float POINTER_ENTER_ANIMATION_TIME = 0.2f;
    const float POINTER_ENTER_SCALE_AMOUNT = 1.2f;

    // 선택 관련
    bool isSelectedProduct;

    // 매진 관련
    bool isSoldOut;

    public void SetProductInfo(S_Product product, bool isSelectedProduct, S_StoreSlotEnum slot)
    {
        // 상품 및 슬롯 설정
        ThisProduct = product;
        SlotInfo = slot;

        // 상품 스프라이트
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{ThisProduct.Key}");
        cardEffectOpHandle.Completed += OnProductLoadComplete;

        // 골드 설정
        if (isSelectedProduct)
        {
            text_Price.gameObject.SetActive(false);
            text_Description.gameObject.SetActive(false);
        }
        else
        {
            text_Description.text = ThisProduct.Description;

            if (ThisProduct.Price == 0)
            {
                text_Price.text = $"무료!";
            }
            else
            {
                text_Price.text = $"{ThisProduct.Price} 골드";
            }
        }

        // 소팅오더 설정
        SetOrder();
    }
    public void SetSoldOut()
    {
        // 상품 스프라이트
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Product_SoldOut");
        cardEffectOpHandle.Completed += OnProductLoadComplete;

        // 골드 설정
        text_Description.text = "매진된 부분은 더 이상 상품이 나오지 않습니다.";
        text_Price.text = "매진";

        // 소팅오더 설정
        SetOrder();

        isSoldOut = true;
    }
    void OnProductLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_Product.sprite = opHandle.Result;
        }
    }
    void SetOrder() // 상품의 오더 설정
    {
        sprite_Product.sortingLayerName = "WorldObject";
        sprite_Product.sortingOrder = 1;

        text_Description.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_Description.GetComponent<MeshRenderer>().sortingOrder = 2;

        text_Price.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_Price.GetComponent<MeshRenderer>().sortingOrder = 2;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSoldOut) return;

        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Store)
        {
            sprite_Product.GetComponent<Transform>().DOScale(transform.localScale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            // TODO : 호버링 시 정보 뜨는 효과 넣기

        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        sprite_Product.GetComponent<Transform>().DOScale(transform.localScale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSoldOut) return;

        if (isSelectedProduct) // 선택된 상품인 경우 클릭하면 취소
        {
            S_StoreInfoSystem.Instance.CancelSelectedProduct();
        }
        else // 상품 목록에 있는 경우 클릭하면 선택
        {
            S_StoreInfoSystem.Instance.SelectProduct(ThisProduct, SlotInfo);
        }
    }
}
