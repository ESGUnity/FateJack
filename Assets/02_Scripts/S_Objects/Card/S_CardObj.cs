using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_CardObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("주요 정보")]
    [HideInInspector] public S_CardBase CardInfo;

    [Header("씬 오브젝트")]
    [SerializeField] protected GameObject obj_Card; // 버튼없는 오직 카드 요소만 있는 오브젝트
    [SerializeField] SpriteRenderer sprite_CardBase; 
    [SerializeField] public SpriteRenderer sprite_CardFrame; // 타입에 따라 바뀜 (테두리가 아래에 Type 적는 ㅜㅂ분도 있음.
    [SerializeField] TMP_Text text_CardType; // 타입에 따라 바뀜
    [SerializeField] TMP_Text text_CardNumber;
    [SerializeField] public SpriteRenderer sprite_CardEffect;
    [SerializeField] public SpriteRenderer sprite_Engraving;
    [SerializeField] SpriteRenderer sprite_CursedEffect;
    [SerializeField] SpriteRenderer sprite_CursedEffect2;

    [Header("프리팹")]
    [SerializeField] protected Material mat_OriginEngraving;

    [Header("연출")]
    [HideInInspector] public PRS OriginPRS;
    [HideInInspector] public int OriginOrder;
    [HideInInspector] public const float POINTER_ENTER_ANIMATION_TIME = 0.15f;
    [HideInInspector] public const float POINTER_ENTER_SCALE_AMOUNT = 1.15f;

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
    public void SetCardInfo(S_CardBase card)
    {
        // 카드 정보 설정
        CardInfo = card;

        if (CardInfo == null) return;

        // 카드 베이스
        var cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_CardBase_Origin");
        cardBaseOpHandle.Completed += OnCardBaseLoadComplete;

        if (card.CardType == S_CardTypeEnum.Foe)
        {
            sprite_CardFrame.gameObject.SetActive(false);
            text_CardType.gameObject.SetActive(false);
            text_CardNumber.gameObject.SetActive(false);
        }
        else
        {
            sprite_CardFrame.gameObject.SetActive(true);
            text_CardType.gameObject.SetActive(true);
            text_CardNumber.gameObject.SetActive(true);

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
                case S_CardTypeEnum.Foe:
                    text_CardType.text = "";
                    break;
            }

            // 카드 숫자 설정
            text_CardNumber.text = card.Weight.ToString();
        }

        // 카드 효과 설정
        string effectKey = card.Key;
        bool isFlipped = false;
        if (effectKey.EndsWith("_Flip")) // _Flip 감지 및 제거
        {
            isFlipped = true;
            effectKey = effectKey.Replace("_Flip", "");
        }
        if (isFlipped) sprite_CardEffect.transform.DOLocalRotate(new Vector3(0, 0, 180), 0); // _Flip이었다면 뒤집기

        sprite_CardEffect.gameObject.SetActive(true);
        var cardEffectOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_CardEffect_{effectKey}"); // 스프라이트 불러오기
        cardEffectOpHandle.Completed += OnCardEffectLoadComplete;

        // 각인 설정
        if (card.Engraving.Count > 0)
        {
            S_EngravingEnum engraving = card.Engraving.First();
            sprite_Engraving.gameObject.SetActive(true);
            var engravingOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_Engraving_{engraving}");
            engravingOpHandle.Completed += OnEngravingLoadComplete;

            switch (engraving) // 분해나 가면의 경우 카드 베이스도 변경해주기
            {
                case S_EngravingEnum.Dismantle:
                    cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_CardBase_Dismantle");
                    cardBaseOpHandle.Completed += OnCardBaseLoadComplete;
                    break;
                case S_EngravingEnum.Mask:
                    cardBaseOpHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_CardBase_Mask");
                    cardBaseOpHandle.Completed += OnCardBaseLoadComplete;
                    break;
            }
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
        text_CardNumber.GetComponent<MeshRenderer>().sortingOrder = order + 6;

        sprite_CardEffect.sortingLayerName = "WorldObject";
        sprite_CardEffect.sortingOrder = order + 2;
        sprite_Engraving.sortingLayerName = "WorldObject";
        sprite_Engraving.sortingOrder = order + 3;
        sprite_CursedEffect.sortingLayerName = "WorldObject";
        sprite_CursedEffect.sortingOrder = order + 7;
        sprite_CursedEffect2.sortingLayerName = "WorldObject";
        sprite_CursedEffect2.sortingOrder = order + 7;
    }
    #endregion
    #region 버튼 함수
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            transform.DOKill();
            obj_Card.transform.DOKill();

            SetOrder(1000);
            transform.DOScale(OriginPRS.Scale * POINTER_ENTER_SCALE_AMOUNT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            transform.DOLocalRotate(Vector3.zero, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
            obj_Card.transform.DOLocalRotate(Vector3.zero, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

            S_HoverInfoSystem.Instance.ActivateHoverInfoByCard(CardInfo, sprite_CardBase.gameObject);

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

        transform.DOKill();
        obj_Card.transform.DOKill();

        SetOrder(OriginOrder);
        transform.DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        transform.DOLocalRotate(OriginPRS.Rot, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);

        S_HoverInfoSystem.Instance.DeactiveHoverInfo();

        isEnter = false;
    }
    #endregion
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
            sprite_CursedEffect2.gameObject.SetActive(true);
        }
        else
        {
            sprite_CursedEffect.gameObject.SetActive(false);
            sprite_CursedEffect2.gameObject.SetActive(false);
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
        sprite_CursedEffect2.DOKill();

        sprite_CardBase.material.DOFloat(value, "_AlphaValue", duration);
        sprite_CardFrame.material.DOFloat(value, "_AlphaValue", duration);
        text_CardType.DOFade(value, duration);
        text_CardNumber.DOFade(value, duration);
        sprite_CardEffect.material.DOFloat(value, "_AlphaValue", duration);
        sprite_Engraving.material.DOFloat(value, "_AlphaValue", duration);
        sprite_CursedEffect.material.DOFloat(value, "_AlphaValue", duration);
        sprite_CursedEffect2.material.DOFloat(value, "_AlphaValue", duration);
    }
    public virtual Sequence SetAlphaValueAsync(float value, float duration)
    {
        sprite_CardBase.DOKill();
        sprite_CardFrame.DOKill();
        text_CardType.DOKill();
        text_CardNumber.DOKill();
        sprite_CardEffect.DOKill();
        sprite_Engraving.DOKill();
        sprite_CursedEffect.DOKill();
        sprite_CursedEffect2.DOKill();

        // Sequence 생성
        Sequence seq = DOTween.Sequence();

        // 모든 트윈을 시퀀스에 추가
        seq.Append(sprite_CardBase.material.DOFloat(value, "_AlphaValue", duration));
        seq.Join(sprite_CardFrame.material.DOFloat(value, "_AlphaValue", duration));
        seq.Join(text_CardType.DOFade(value, duration));
        seq.Join(text_CardNumber.DOFade(value, duration));
        seq.Join(sprite_CardEffect.material.DOFloat(value, "_AlphaValue", duration));
        seq.Join(sprite_Engraving.material.DOFloat(value, "_AlphaValue", duration));
        seq.Join(sprite_CursedEffect.material.DOFloat(value, "_AlphaValue", duration));
        seq.Join(sprite_CursedEffect2.material.DOFloat(value, "_AlphaValue", duration));

        return seq;
    }
    public void BouncingVFX() // 바운싱 VFX
    {
        S_TweenHelper.Instance.BouncingVFX(transform, OriginPRS.Scale, OriginPRS.Rot);
    }
    #endregion
}
