using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_ProductObject : MonoBehaviour // TODO : CardObj처럼 꼼꼼하게 만들기. 버튼은 클릭하면 나오게 하기
{
    [Header("주요 정보")]
    [HideInInspector] public S_ProductInfoEnum ProductInfo;
    [HideInInspector] public string ProductName;
    [HideInInspector] public string Description;
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
    const float POINTER_ENTER_SCALE_AMOUNT = 1.2f;

    public void SetProductInfo(S_ProductInfoEnum product)
    {
        // 상품 설정
        ProductInfo = product;

        // 텍스트 설정
        switch (ProductInfo)
        {
            case S_ProductInfoEnum.ThanatosBundle:
                ProductName = "타나토스의 보따리";
                Description = "무작위 쓸만한 물건 3개 중 1개를 얻습니다.";
                Price = 7;
                break;
            case S_ProductInfoEnum.OldMold:
                ProductName = "위대한 설계도";
                Description = "덱에서 카드를 1장 선택하여 모든 요소를 무작위로 재설정합니다.";
                Price = 2;
                break;
            case S_ProductInfoEnum.MeltedMold:
                ProductName = "검붉은 붓";
                Description = "덱에서 카드를 1장 선택하여 문양을 무작위로 변경합니다.";
                Price = 3;
                break;
            case S_ProductInfoEnum.SpiritualMold:
                ProductName = "에리스의 주사위";
                Description = "덱에서 카드를 1장 선택하여 숫자를 무작위로 변경합니다.";
                Price = 3;
                break;
            case S_ProductInfoEnum.BrightMold:
                ProductName = "점성술 도구";
                Description = "덱에서 카드를 1장 선택하여 기본 조건, 추가 조건, 디버프를 재설정합니다.";
                Price = 3;
                break;
            case S_ProductInfoEnum.DelicateMold:
                ProductName = "모무스의 손가락";
                Description = "덱에서 카드를 1장 선택하여 기본 효과, 추가 효과를 재설정합니다.";
                Price = 3;
                break;
            case S_ProductInfoEnum.GerasBlueprint:
                ProductName = "낡은 직조기";
                Description = "덱에서 카드를 1장 선택하여 기본 조건을 변경하고 기본 효과, 추가 효과를 재설정합니다.";
                Price = 4;
                break;
            case S_ProductInfoEnum.ErisDice:
                ProductName = "신성한 인장";
                Description = "덱에서 카드를 1장 선택하여 추가 조건을 부여하고 기본 효과, 추가 효과를 재설정합니다.";
                Price = 4;
                break;
            case S_ProductInfoEnum.HypnosBrush:
                ProductName = "모로스의 저주";
                Description = "덱에서 카드를 1장 선택하여 디버프를 부여하고 기본 효과, 추가 효과를 재설정합니다.";
                Price = 4;
                break;
            case S_ProductInfoEnum.OneiroiChisel:
                ProductName = "플루토의 잔불";
                Description = "덱에서 카드를 1장 선택하여 디버프를 없애고 기본 효과, 추가 효과를 재설정합니다.";
                Price = 5;
                break;
            case S_ProductInfoEnum.PlutoChisel:
                ProductName = "낡은 가위";
                Description = "덱에서 카드를 1장 선택하여 추가 효과를 부여하고 기본 조건, 추가 조건, 디버프를 재설정합니다.";
                Price = 4;
                break;
            case S_ProductInfoEnum.OracleBall:
                ProductName = "예지 구슬";
                Description = "다음 피조물의 체력과 능력을 봅니다.";
                Price = 0;
                break;
            case S_ProductInfoEnum.WasteBasket:
                ProductName = "신경마비 광원";
                Description = "내 능력 중 1개를 제거합니다.";
                Price = 0;
                break;
            case S_ProductInfoEnum.ShellGameCup:
                ProductName = "자세 교정기";
                Description = "내 능력 중 1개를 선택하여 맨 앞으로 위치하게 만듭니다.";
                Price = 0;
                break;
        }
        text_Name.text = ProductName;

        // TMP 셋팅
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
    public void PointerEnterProductSprite()
    {
        sprite_Product.transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_StoreInfoSystem.Instance.GenerateMonologByPointerEnter(ProductInfo);
    }
    public void PointerExitProductSprite()
    {
        sprite_Product.transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_DialogInfoSystem.Instance.EndMonolog();
    }
    public void ClickBuyBtn()
    {
        if (ProductInfo == S_ProductInfoEnum.OracleBall)
        {
            S_StoreInfoSystem.Instance.GenerateMonologByBuyOracleBall();
        }
        else if (S_StoreInfoSystem.Instance.BuyProduct(this)) // 구매 완료 시 오브젝트 파괴
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
