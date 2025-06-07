using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_CardObject : MonoBehaviour
{
    [Header("주요 정보")]
    [HideInInspector] public S_Card CardInfo;

    [Header("연출")]
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;
    [HideInInspector] public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;
    [HideInInspector] public const float POINTER_ENTER_SCALE_AMOUNT = 1.2f;
    [HideInInspector] public Vector3 POINTER_ENTER_POS = new Vector3(0, 0.05f, 0);

    [Header("씬 오브젝트")]
    [SerializeField] protected SpriteRenderer sprite_MeetConditionEffect;
    [SerializeField] SpriteRenderer sprite_CardBase;
    [SerializeField] SpriteRenderer sprite_CardSuit;
    [SerializeField] TMP_Text text_CardNumber;

    [SerializeField] SpriteRenderer sprite_BasicCondition;
    [SerializeField] SpriteRenderer sprite_BasicEffect;
    [SerializeField] SpriteRenderer sprite_AdditiveCondition;
    [SerializeField] SpriteRenderer sprite_Debuff;
    [SerializeField] SpriteRenderer sprite_AdditiveEffect;

    [SerializeField] protected SpriteRenderer sprite_CursedEffect;
    [SerializeField] protected SpriteRenderer sprite_CurrentTurnHitEffect;

    [SerializeField] protected SpriteRenderer sprite_CardFrame;

    public void SetCardInfo(S_Card card)
    {
        // 카드 정보 설정
        CardInfo = card;

        if (CardInfo == null) return;

        // 카드 베이스 설정
        string cardBaseAddress = "";
        if (card.IsIllusion) cardBaseAddress = "Sprite_IllusionCardBase";
        else cardBaseAddress = "Sprite_OriginCardBase";
        var cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>(cardBaseAddress);
        cardBaseOpHandle.Completed += OnCardBaseLoadComplete;

        // 카드 문양 설정
        var cardSuitOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Card_{card.Suit}Suit");
        cardSuitOpHandle.Completed += OnCardSuitLoadComplete;

        // 카드 숫자 설정
        text_CardNumber.text = card.Number.ToString();

        // 카드 효과 설정
        if (card.BasicCondition != S_CardBasicConditionEnum.None)
        {
            sprite_BasicCondition.gameObject.SetActive(true);
            var basicConditionOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_BasicCondition_{card.BasicCondition}");
            basicConditionOpHandle.Completed += OnBasicConditionLoadComplete;
        }
        else
        {
            sprite_BasicCondition.gameObject.SetActive(false);
        }

        if (card.BasicEffect != S_CardBasicEffectEnum.None)
        {
            sprite_BasicEffect.gameObject.SetActive(true);
            var basicEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_BasicEffect_{card.BasicEffect}");
            basicEffectOpHandle.Completed += OnBasicEffectLoadComplete;
        }
        else
        {
            sprite_BasicEffect.gameObject.SetActive(false);
        }

        if (card.AdditiveCondition != S_CardAdditiveConditionEnum.None)
        {
            sprite_AdditiveCondition.gameObject.SetActive(true);
            var additiveConditionOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_AdditiveCondition_{card.AdditiveCondition}");
            additiveConditionOpHandle.Completed += OnAdditiveConditionLoadComplete;
        }
        else
        {
            sprite_AdditiveCondition.gameObject.SetActive(false);
        }

        if (card.Debuff != S_CardDebuffConditionEnum.None)
        {
            sprite_Debuff.gameObject.SetActive(true);
            var debuffOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Debuff_{card.Debuff}");
            debuffOpHandle.Completed += OnDebuffLoadComplete;
        }
        else
        {
            sprite_Debuff.gameObject.SetActive(false);
        }

        if (card.AdditiveEffect != S_CardAdditiveEffectEnum.None)
        {
            sprite_AdditiveEffect.gameObject.SetActive(true);
            var additiveEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_AdditiveEffect_{card.AdditiveEffect}");
            additiveEffectOpHandle.Completed += OnAdditiveEffectLoadComplete;
        }
        else
        {
            sprite_AdditiveEffect.gameObject.SetActive(false);
        }

        UpdateCardState();
    }
    void OnCardBaseLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_CardBase.sprite = opHandle.Result;
        }
    }
    void OnCardSuitLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_CardSuit.sprite = opHandle.Result;
        }
    }
    void OnBasicConditionLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_BasicCondition.sprite = opHandle.Result;
        }
    }
    void OnBasicEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_BasicEffect.sprite = opHandle.Result;
        }
    }
    void OnAdditiveConditionLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_AdditiveCondition.sprite = opHandle.Result;
        }
    }
    void OnDebuffLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_Debuff.sprite = opHandle.Result;
        }
    }
    void OnAdditiveEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_AdditiveEffect.sprite = opHandle.Result;
        }
    }
    public void SetOrder(int order) // 카드 요소의 소팅 오더를 정렬
    {
        // 카드 베이스
        sprite_MeetConditionEffect.sortingLayerName = "WorldObject";
        sprite_MeetConditionEffect.sortingOrder = order;
        sprite_CardBase.sortingLayerName = "WorldObject";
        sprite_CardBase.sortingOrder = order + 1;
        sprite_CardSuit.sortingLayerName = "WorldObject";
        sprite_CardSuit.sortingOrder = order + 3;
        text_CardNumber.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_CardNumber.GetComponent<MeshRenderer>().sortingOrder = order + 4;


        sprite_BasicCondition.sortingLayerName = "WorldObject";
        sprite_BasicCondition.sortingOrder = order + 2;
        sprite_BasicEffect.sortingLayerName = "WorldObject";
        sprite_BasicEffect.sortingOrder = order + 2;
        sprite_AdditiveCondition.sortingLayerName = "WorldObject";
        sprite_AdditiveCondition.sortingOrder = order + 3;
        sprite_Debuff.sortingLayerName = "WorldObject";
        sprite_Debuff.sortingOrder = order + 3;
        sprite_AdditiveEffect.sortingLayerName = "WorldObject";
        sprite_AdditiveEffect.sortingOrder = order + 3;

        sprite_CursedEffect.sortingLayerName = "WorldObject";
        sprite_CursedEffect.sortingOrder = order + 5;
        sprite_CurrentTurnHitEffect.sortingLayerName = "WorldObject";
        sprite_CurrentTurnHitEffect.sortingOrder = order + 6;

        sprite_CardFrame.sortingLayerName = "WorldObject";
        sprite_CardFrame.sortingOrder = order + 7;
    }
    #region 카드의 상태에 따른 효과
    public virtual void UpdateCardState()
    {
        OnCursedEffect();
    }
    public void OnCursedEffect()
    {
        if (CardInfo.IsCursed)
        {
            sprite_CursedEffect.gameObject.SetActive(true);
        }
        else
        {
            sprite_CursedEffect.gameObject.SetActive(false);
        }
    }
    public void SetAlphaValue(float value, float duration)
    {
        sprite_MeetConditionEffect.DOKill();
        sprite_CardBase.DOKill();
        sprite_CardSuit.DOKill();
        text_CardNumber.DOKill();

        sprite_BasicCondition.DOKill();
        sprite_BasicEffect.DOKill();
        sprite_AdditiveCondition.DOKill();
        sprite_Debuff.DOKill();
        sprite_AdditiveEffect.DOKill();

        sprite_CursedEffect.DOKill();
        sprite_CurrentTurnHitEffect.DOKill();

        sprite_CardFrame.DOKill();

        sprite_MeetConditionEffect.DOFade(value, duration);
        sprite_CardBase.DOFade(value, duration);
        sprite_CardSuit.DOFade(value, duration);
        text_CardNumber.DOFade(value, duration);

        sprite_BasicCondition.DOFade(value, duration);
        sprite_BasicEffect.DOFade(value, duration);
        sprite_AdditiveCondition.DOFade(value, duration);
        sprite_Debuff.DOFade(value, duration);
        sprite_AdditiveEffect.DOFade(value, duration);

        sprite_CursedEffect.DOFade(value, duration);
        sprite_CurrentTurnHitEffect.DOFade(value, duration);

        sprite_CardFrame.DOFade(value, duration);
    }
    #endregion
}
