using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_ReadOnlyUILoot : MonoBehaviour
{
    // ī�� ����
    [HideInInspector] public S_Skill LootInfo;

    // ī�� �ִ� ����
    [HideInInspector] public PRS OriginPRS;
    public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;

    // ī�� ǥ�� ����
    [SerializeField] Image image_LootBase;

    Vector2 addLootPos = new Vector2(1200, -800);
    Vector2 removeLootPos = new Vector2(0, 400);
    const float APPEAR_TIME = 0.5f;

    public void SetLootInfo(S_Skill loot)
    {
        image_LootBase.raycastTarget = false;

        // ī�� ���� ����
        LootInfo = loot;

        if (LootInfo.Equals(null)) return;

        // ī�� ȿ�� ����
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
        // ũ�⸦ �۰�
        transform.localScale = Vector3.zero;  // ó���� ������ 0

        // ������ ó��
        image_LootBase.DOFade(0f, 0f);

        // ī�� ��ġ ����
        Vector2 generateLootPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
        GetComponent<RectTransform>().anchoredPosition = generateLootPos;

        // VFX
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;
        Vector2 end = addLootPos;

        // ��� �־��� ���� (y�δ� ���� ����, x�δ� ��/�� ������ �־���)
        float curveHeight = 200f;
        float curveHorizontal = Random.Range(-350f, 350f); // ���� �Ǵ� ���������� �ְ�

        // �߰� ���� (�־��� ����)
        Vector2 control = new Vector2(
            Mathf.Lerp(start.x, end.x, 0.5f) + curveHorizontal,
            Mathf.Lerp(start.y, end.y, 0.5f) + curveHeight
        );

        // ��� ���� (�������� ���� ����)
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
        // ũ�⸦ �۰�
        transform.localScale = Vector3.zero;  // ó���� ������ 0

        // ������ ó��
        image_LootBase.DOFade(0f, 0f);

        // ī�� ��ġ ����
        Vector2 generateLootPos = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        GetComponent<RectTransform>().anchoredPosition = generateLootPos;

        Sequence seq = DOTween.Sequence();

        RectTransform rt = GetComponent<RectTransform>();

        // ���ÿ� ����: ��ġ �̵� (2��), ������ �� (0.5��), ���� �� (0.5��)
        seq.Append(rt.DOAnchorPos(removeLootPos, 2f));
        seq.Join(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuart));
        seq.Join(image_LootBase.DOFade(1f, 0.5f).SetEase(Ease.OutQuart));

        // ��Ȯ�� 1.5�� �������� ���� �ٿ� ����
        seq.Insert(1.5f, image_LootBase.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));

        // 2�� �� ����
        seq.OnComplete(() => Destroy(gameObject));
    }
}
