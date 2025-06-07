using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_StoreUISkill : MonoBehaviour
{
    // 카드 정보
    [HideInInspector] public S_Skill LootInfo;

    // 카드 애님 관련
    [HideInInspector] public PRS OriginPRS;
    public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;

    // 카드 표시 관련
    [SerializeField] Image image_LootBase;

    Vector2 addLootPos = new Vector2(1200, -800);
    Vector2 removeLootPos = new Vector2(0, 400);
    const float APPEAR_TIME = 0.5f;

    public void SetLootInfo(S_Skill loot)
    {
        image_LootBase.raycastTarget = false;

        // 카드 정보 설정
        LootInfo = loot;

        if (LootInfo.Equals(null)) return;

        // 카드 효과 설정
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{loot.Key}");
        cardEffectOpHandle.Completed += OnLootBaseLoadComplete;
    }
    void OnLootBaseLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_LootBase.sprite = opHandle.Result;
        }
    }
    public void GetLootVFX()
    {
        // 크기를 작게
        transform.localScale = Vector3.zero;  // 처음에 스케일 0

        // 불투명도 처리
        image_LootBase.DOFade(0f, 0f);

        // 카드 위치 설정
        Vector2 generateLootPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
        GetComponent<RectTransform>().anchoredPosition = generateLootPos;

        // VFX
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;
        Vector2 end = addLootPos;

        // 곡선의 휘어짐 정도 (y로는 위로 띄우고, x로는 좌/우 무작위 휘어짐)
        float curveHeight = 200f;
        float curveHorizontal = Random.Range(-350f, 350f); // 왼쪽 또는 오른쪽으로 휘게

        // 중간 지점 (휘어짐 제어)
        Vector2 control = new Vector2(
            Mathf.Lerp(start.x, end.x, 0.5f) + curveHorizontal,
            Mathf.Lerp(start.y, end.y, 0.5f) + curveHeight
        );

        // 경로 구성 (시작점은 생략 가능)
        Vector3[] path = new Vector3[] { control, end };

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(Vector3.one, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(image_LootBase.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .AppendInterval(0.8f)
            .Append(rt.DOLocalPath(path, APPEAR_TIME, PathType.CatmullRom, PathMode.Ignore).SetEase(Ease.OutQuart))
            .Join(transform.DOScale(Vector3.zero, APPEAR_TIME).SetEase(Ease.OutQuart))
            .OnComplete(() => Destroy(gameObject));
    }
    public void RemoveLootVFX()
    {
        // 크기를 작게
        transform.localScale = Vector3.zero;  // 처음에 스케일 0

        // 불투명도 처리
        image_LootBase.DOFade(0f, 0f);

        // 카드 위치 설정
        Vector2 generateLootPos = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        GetComponent<RectTransform>().anchoredPosition = generateLootPos;

        Sequence seq = DOTween.Sequence();

        RectTransform rt = GetComponent<RectTransform>();

        // 동시에 시작: 위치 이동 (2초), 스케일 업 (0.5초), 알파 업 (0.5초)
        seq.Append(rt.DOAnchorPos(removeLootPos, 2f));
        seq.Join(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuart));
        seq.Join(image_LootBase.DOFade(1f, 0.5f).SetEase(Ease.OutQuart));

        // 정확히 1.5초 지점에서 알파 다운 시작
        seq.Insert(1.5f, image_LootBase.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));

        // 2초 후 제거
        seq.OnComplete(() => Destroy(gameObject));
    }
}
