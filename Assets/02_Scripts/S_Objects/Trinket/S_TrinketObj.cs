using DG.Tweening;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_TrinketObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("주요 정보")]
    [HideInInspector] public S_Trinket TrinketInfo;

    [Header("컴포넌트")]
    [SerializeField] GameObject obj_Trinket;
    [SerializeField] SpriteRenderer sprite_MeetConditionEffect;
    [SerializeField] protected SpriteRenderer sprite_Trinket;

    [Header("VFX")]
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;
    public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;
    public const float POINTER_ENTER_SCALE_AMOUNT = 1.2f;

    [Header("포인터 연출")]
    bool isEnter = false;
    protected List<S_GameFlowStateEnum> VALID_STATES;

    protected virtual void Awake()
    {
        VALID_STATES = new();
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

    #region 초기화
    public virtual void SetTrinketInfo(S_Trinket trinket)
    {
        TrinketInfo = trinket;

        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Trinket_{trinket.Key}");
        cardEffectOpHandle.Completed += OnTrinketSpriteLoadComplete;
    }
    void OnTrinketSpriteLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_MeetConditionEffect.sprite = opHandle.Result;
            sprite_Trinket.sprite = opHandle.Result;
        }
    }
    public virtual void SetOrder(int order) // 각 요소의 소팅오더 설정
    {
        sprite_MeetConditionEffect.sortingLayerName = "WorldObject";
        sprite_MeetConditionEffect.sortingOrder = order;

        sprite_Trinket.sortingLayerName = "WorldObject";
        sprite_Trinket.sortingOrder = order + 1;
    }
    #endregion
    #region 주요 함수
    public virtual void UpdateTrinketObj() // MeetConditionEffect 업데이트
    {
        // MeetConditionEffect 업데이트
        if (TrinketInfo.IsMeetCondition)
        {
            sprite_MeetConditionEffect.gameObject.SetActive(true);
            sprite_MeetConditionEffect.DOKill();
            sprite_MeetConditionEffect.DOFade(1f, POINTER_ENTER_ANIMATION_TIME);
        }
        else
        {
            sprite_MeetConditionEffect.DOKill();
            sprite_MeetConditionEffect.DOFade(0f, POINTER_ENTER_ANIMATION_TIME)
                .OnComplete(() => sprite_MeetConditionEffect.gameObject.SetActive(false));
        }
    }
    #endregion
    #region 포인터 함수
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            obj_Trinket.transform.DOKill();

            SetOrder(1000);
            obj_Trinket.transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverInfoSystem.Instance.ActivateHoverInfo(TrinketInfo, gameObject);

            isEnter = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ForceExit();
    }
    public virtual void ForceExit()
    {
        if (!isEnter) return;

        obj_Trinket.transform.DOKill();

        SetOrder(OriginOrder);
        obj_Trinket.transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();

        isEnter = false;
    }
    #endregion
    #region VFX 함수
    public virtual void SetAlphaValue(float value, float duration)
    {
        sprite_MeetConditionEffect.DOKill();
        sprite_Trinket.DOKill();

        sprite_MeetConditionEffect.DOFade(value, duration);
        sprite_Trinket.DOFade(value, duration);
    }
    public void BouncingVFX()
    {
        S_TweenHelper.Instance.BouncingVFX(obj_Trinket.transform, OriginPRS.Scale);
    }
    #endregion
}
