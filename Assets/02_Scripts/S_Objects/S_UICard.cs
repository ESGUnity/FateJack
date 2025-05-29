using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class S_UICard : MonoBehaviour
{
    // ī�� ����
    [HideInInspector] public S_Card CardInfo;

    // ī�� ǥ�� ����
    [SerializeField] protected Image image_CardBase;
    [SerializeField] protected TMP_Text text_CardNumber;
    [SerializeField] protected Image image_CardSuit;
    [SerializeField] protected Image image_CardEffect;
    [SerializeField] protected Image image_CursedEffect;
    [SerializeField] protected Image image_CardFrame;

    Vector2 addCardPos = new Vector2(0, -800);
    Vector2 exclusionCardPos = new Vector2(0, 300);
    const float APPEAR_TIME = 0.5f;

    void Awake()
    {
        image_CardBase.raycastTarget = false;
        text_CardNumber.raycastTarget = false;
        image_CardSuit.raycastTarget = false;
        image_CardEffect.raycastTarget = false;
        image_CursedEffect.raycastTarget = false;
        image_CardFrame.raycastTarget = false;
    }
    public void SetCardInfo(S_Card card)
    {
        // ī�� ���� ����
        CardInfo = card;

        if (CardInfo == null) return;

        // ī�� ���̽� ����
        string cardBaseAddress = "";
        switch (card.CardEffect.Grade)
        {
            case S_CardEffectGradeEnum.Normal:
                cardBaseAddress = "Sprite_NormalCardBase";
                break;
            case S_CardEffectGradeEnum.Superior:
                cardBaseAddress = "Sprite_SuperiorCardBase";
                break;
            case S_CardEffectGradeEnum.Rare:
                cardBaseAddress = "Sprite_RareCardBase";
                break;
            case S_CardEffectGradeEnum.Mythic:
                cardBaseAddress = "Sprite_MythicCardBase";
                break;
        }
        var cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>(cardBaseAddress);
        cardBaseOpHandle.Completed += OnCardBaseLoadComplete;

        // ī�� ���� ����
        text_CardNumber.text = card.Number.ToString();

        // ī�� ���� ����
        string cardSuitAddress = "";
        switch (card.Suit)
        {
            case S_CardSuitEnum.Spade:
                cardSuitAddress = "Sprite_SpadeSuit";
                break;
            case S_CardSuitEnum.Heart:
                cardSuitAddress = "Sprite_HeartSuit";
                break;
            case S_CardSuitEnum.Diamond:
                cardSuitAddress = "Sprite_DiamondSuit";
                break;
            case S_CardSuitEnum.Clover:
                cardSuitAddress = "Sprite_CloverSuit";
                break;
        }
        var cardSuitOpHandle = Addressables.LoadAssetAsync<Sprite>(cardSuitAddress);
        cardSuitOpHandle.Completed += OnCardSuitLoadComplete;

        // ī�� ȿ�� ����
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{card.CardEffect.Key}");
        cardEffectOpHandle.Completed += OnCardEffectLoadComplete;
    }
    void OnCardBaseLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_CardBase.sprite = opHandle.Result;
        }
    }
    void OnCardSuitLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_CardSuit.sprite = opHandle.Result;
        }
    }
    void OnCardEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image_CardEffect.sprite = opHandle.Result;
        }
    }

    public void GetCardVFX()
    {
        // ī�� ũ�⸦ �۰�
        transform.localScale = Vector3.zero;  // ó���� ������ 0

        // ������ ó��
        image_CardBase.DOFade(0f, 0f);
        text_CardNumber.DOFade(0f, 0f);
        image_CardSuit.DOFade(0f, 0f);
        image_CardEffect.DOFade(0f, 0f);

        // ī�� ��ġ ����
        Vector2 generateCardPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
        GetComponent<RectTransform>().anchoredPosition = generateCardPos;

        // VFX
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;
        Vector2 end = addCardPos;

        // ��� �־��� ���� (y�δ� ���� ����, x�δ� ��/�� ������ �־���)
        float curveHeight = 250f;
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
            .Join(image_CardBase.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(1f, APPEAR_TIME).SetEase(Ease.OutQuart))
            .AppendInterval(0.8f)
            .Append(rt.DOLocalPath(path, APPEAR_TIME, PathType.CatmullRom, PathMode.Ignore).SetEase(Ease.OutQuart))
            .Join(transform.DOScale(Vector3.zero, APPEAR_TIME).SetEase(Ease.OutQuart))
            .OnComplete(() => Destroy(gameObject));
    }
    public async Task ExclusionCardVFX()
    {
        // ī�� ũ�⸦ �۰�
        transform.localScale = Vector3.zero;  // ó���� ������ 0
        GetComponent<RectTransform>().DOAnchorPos(addCardPos, 0f); // ī�� ��ġ�� ������

        // ������ ó��
        SetAlphaValue(0, 0);

        // ī�� ��ġ ����
        Vector2 generateCardPos = new Vector2(Random.Range(-600f, 600f), Random.Range(-300f, 300f));
        GetComponent<RectTransform>().anchoredPosition = generateCardPos;

        Sequence seq = DOTween.Sequence();
        seq.Append(GetComponent<RectTransform>().DOAnchorPos(generateCardPos, 1.6f))
            .Join(transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardBase.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(0.8f, 0.8f).SetEase(Ease.OutQuart))
            .Append(GetComponent<RectTransform>().DOAnchorPos(exclusionCardPos, 1f))
            .AppendInterval(0.2f)
            .Join(image_CardBase.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(0f, 0.8f).SetEase(Ease.OutQuart))
            .OnComplete(() => Destroy(gameObject));

        await Task.Delay(1200);
    }
    public void SetAlphaValue(float value, float duration)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(image_CardBase.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(text_CardNumber.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_CardSuit.DOFade(value, duration).SetEase(Ease.OutQuart))
            .Join(image_CardEffect.DOFade(value, duration).SetEase(Ease.OutQuart));
    }
    public void SetCursedEffect(bool isCursed)
    {
        image_CursedEffect.gameObject.SetActive(isCursed);
    }
}
