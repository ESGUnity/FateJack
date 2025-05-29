using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_ProductObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // ��ǰ ����
    [HideInInspector] public S_Product ThisProduct;
    [HideInInspector] public S_StoreSlotEnum SlotInfo;

    // ������Ʈ
    [SerializeField] SpriteRenderer sprite_Product;
    [SerializeField] TMP_Text text_Description;
    [SerializeField] TMP_Text text_Price;

    // �ִ� ����
    const float POINTER_ENTER_ANIMATION_TIME = 0.2f;
    const float POINTER_ENTER_SCALE_AMOUNT = 1.2f;

    // ���� ����
    bool isSelectedProduct;

    // ���� ����
    bool isSoldOut;

    public void SetProductInfo(S_Product product, bool isSelectedProduct, S_StoreSlotEnum slot)
    {
        // ��ǰ �� ���� ����
        ThisProduct = product;
        SlotInfo = slot;

        // ��ǰ ��������Ʈ
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{ThisProduct.Key}");
        cardEffectOpHandle.Completed += OnProductLoadComplete;

        // ��� ����
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
                text_Price.text = $"����!";
            }
            else
            {
                text_Price.text = $"{ThisProduct.Price} ���";
            }
        }

        // ���ÿ��� ����
        SetOrder();
    }
    public void SetSoldOut()
    {
        // ��ǰ ��������Ʈ
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Product_SoldOut");
        cardEffectOpHandle.Completed += OnProductLoadComplete;

        // ��� ����
        text_Description.text = "������ �κ��� �� �̻� ��ǰ�� ������ �ʽ��ϴ�.";
        text_Price.text = "����";

        // ���ÿ��� ����
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
    void SetOrder() // ��ǰ�� ���� ����
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

            // TODO : ȣ���� �� ���� �ߴ� ȿ�� �ֱ�

        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        sprite_Product.GetComponent<Transform>().DOScale(transform.localScale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSoldOut) return;

        if (isSelectedProduct) // ���õ� ��ǰ�� ��� Ŭ���ϸ� ���
        {
            S_StoreInfoSystem.Instance.CancelSelectedProduct();
        }
        else // ��ǰ ��Ͽ� �ִ� ��� Ŭ���ϸ� ����
        {
            S_StoreInfoSystem.Instance.SelectProduct(ThisProduct, SlotInfo);
        }
    }
}
