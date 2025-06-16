using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_CardObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("주요 정보")]
    [HideInInspector] public S_Card CardInfo;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject obj_Card; // 버튼없는 오직 카드 요소만 있는 오브젝트
    [SerializeField] SpriteRenderer sprite_CardBase; // 그냥 카드와 생성된 카드에 따라
    [SerializeField] SpriteRenderer sprite_CardFrame; // 타입에 따라 바뀜 (테두리가 아래에 Type 적는 ㅜㅂ분도 있음.
    [SerializeField] TMP_Text text_CardType; // 타입에 따라 바뀜
    [SerializeField] TMP_Text text_CardNumber;
    [SerializeField] protected SpriteRenderer sprite_CardEffect;
    [SerializeField] protected SpriteRenderer sprite_Engraving;
    [SerializeField] SpriteRenderer sprite_CursedEffect;

    [Header("프리팹")]
    [SerializeField] Material m_EngravingOrigin;
    [SerializeField] Material m_EngravingGlow;

    [Header("연출")]
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;
    [HideInInspector] public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;
    [HideInInspector] public const float POINTER_ENTER_SCALE_AMOUNT = 1.15f;
    [HideInInspector] public Vector3 POINTER_ENTER_POS = new Vector3(0, 0.05f, -0.1f); // TODO 포즈를 높이면 그림자는 안지지만 다음 카드 호버링이 메그럽지 못할 수도 있다. 머티리얼을바꾸는 쪽으로 생각해보자 TODO

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
    public void SetCardInfo(S_Card card)
    {
        // 카드 정보 설정
        CardInfo = card;

        if (CardInfo == null) return;

        // 카드 베이스 설정(덱에서 낸 카드와 생성된 카드 구분)
        string cardBaseAddress = "";
        if (card.IsGenerated) cardBaseAddress = "Sprite_CardBase_Gen";
        else cardBaseAddress = "Sprite_CardBase_Origin";
        var cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>(cardBaseAddress);
        cardBaseOpHandle.Completed += OnCardBaseLoadComplete;

        // 카드 프레임 설정
        var cardFrameOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_CardFrame_{card.CardType}");
        cardFrameOpHandle.Completed += OnCardFrameLoadComplete;

        // 카드 타입 설정
        switch (card.CardType)
        {
            case S_CardTypeEnum.Str:
                text_CardType.text = "힘";
                break;
            case S_CardTypeEnum.Mind:
                text_CardType.text = "정신력";
                break;
            case S_CardTypeEnum.Luck:
                text_CardType.text = "행운";
                break;
            case S_CardTypeEnum.Common:
                text_CardType.text = "공용";
                break;
        }

        // 카드 숫자 설정
        text_CardNumber.text = card.Num.ToString();

        // 카드 효과 설정
        if (card.CardEffect != S_CardEffectEnum.None)
        {
            sprite_CardEffect.gameObject.SetActive(true);
            var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_CardEffect_{card.CardEffect}");
            cardEffectOpHandle.Completed += OnCardEffectLoadComplete;
        }
        else
        {
            sprite_CardEffect.gameObject.SetActive(false);
        }

        // 각인 설정
        if (card.Engraving != S_EngravingEnum.None)
        {
            sprite_Engraving.gameObject.SetActive(true);
            var engravingOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Engraving_{card.Engraving}");
            engravingOpHandle.Completed += OnEngravingLoadComplete;
        }
        else
        {
            sprite_Engraving.gameObject.SetActive(false);
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
    void OnCardEffectLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_CardEffect.sprite = opHandle.Result;
        }
    }
    void OnEngravingLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_Engraving.sprite = opHandle.Result;
        }
    }
    void OnCardFrameLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_CardFrame.sprite = opHandle.Result;
        }
    }
    public virtual void SetOrder(int order) // 카드 요소의 소팅 오더를 정렬
    {
        // 카드 베이스
        sprite_CardBase.sortingLayerName = "WorldObject";
        sprite_CardBase.sortingOrder = order + 1;
        sprite_CardFrame.sortingLayerName = "WorldObject";
        sprite_CardFrame.sortingOrder = order + 5;
        text_CardType.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_CardType.GetComponent<MeshRenderer>().sortingOrder = order + 6;
        text_CardNumber.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_CardNumber.GetComponent<MeshRenderer>().sortingOrder = order + 4;

        sprite_CardEffect.sortingLayerName = "WorldObject";
        sprite_CardEffect.sortingOrder = order + 2;
        sprite_Engraving.sortingLayerName = "WorldObject";
        sprite_Engraving.sortingOrder = order + 3;
        sprite_CursedEffect.sortingLayerName = "WorldObject";
        sprite_CursedEffect.sortingOrder = order + 7;


    }
    #endregion
    #region 버튼 함수
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            obj_Card.transform.DOKill();

            SetOrder(1000);
            obj_Card.transform.DOLocalMove(OriginPRS.Pos + POINTER_ENTER_POS, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            obj_Card.transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            obj_Card.transform.DOLocalRotate(Vector3.zero, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverInfoSystem.Instance.ActivateHoverInfo(CardInfo, gameObject);

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

        obj_Card.transform.DOKill();

        SetOrder(OriginOrder);
        obj_Card.transform.DOLocalMove(OriginPRS.Pos, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        obj_Card.transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        obj_Card.transform.DOLocalRotate(OriginPRS.Rot, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();

        isEnter = false;
    }
    #endregion
    #region 카드의 상태에 따른 효과
    public virtual void UpdateCardState()
    {
        OnCursedEffect();
        OnEngravingMeetConditionEffect();
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
    public void OnEngravingMeetConditionEffect()
    {
        if (CardInfo.IsEngravingActiaved)
        {
            sprite_Engraving.material = m_EngravingGlow;
        }
        else
        {
            sprite_Engraving.material = m_EngravingOrigin;
        }
    }
    public virtual void SetAlphaValue(float value, float duration)
    {
        sprite_CardBase.DOKill();
        sprite_CardFrame.DOKill();
        text_CardType.DOKill();
        text_CardNumber.DOKill();
        sprite_CardEffect.DOKill();
        sprite_Engraving.DOKill();
        sprite_CursedEffect.DOKill();

        sprite_CardBase.DOFade(value, duration);
        sprite_CardFrame.DOFade(value, duration);
        text_CardType.DOFade(value, duration);
        text_CardNumber.DOFade(value, duration);
        sprite_CardEffect.DOFade(value, duration);
        sprite_Engraving.DOFade(value, duration);
        sprite_CursedEffect.DOFade(value, duration);
    }
    public void BouncingVFX() // 바운싱 VFX
    {
        S_TweenHelper.Instance.BouncingVFX(obj_Card.transform, OriginPRS.Scale, OriginPRS.Rot);
    }
    #endregion
}
