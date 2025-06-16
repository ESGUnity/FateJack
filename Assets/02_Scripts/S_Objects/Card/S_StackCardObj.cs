using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_StackCardObj : S_CardObj, IPointerEnterHandler
{
    [SerializeField] SpriteRenderer sprite_MoveRightBtn;
    [SerializeField] TMP_Text text_MoveRight;
    [SerializeField] SpriteRenderer sprite_MoveLeftBtn;
    [SerializeField] TMP_Text text_MoveLeft;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Hit, S_GameFlowStateEnum.HittingCard };
    }

    public override void SetOrder(int order)
    {
        base.SetOrder(order);

        sprite_MoveRightBtn.sortingLayerName = "WorldObject";
        sprite_MoveRightBtn.sortingOrder = order + 1;
        text_MoveRight.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_MoveRight.GetComponent<MeshRenderer>().sortingOrder = order + 2;
        sprite_MoveLeftBtn.sortingLayerName = "WorldObject";
        sprite_MoveLeftBtn.sortingOrder = order + 1;
        text_MoveLeft.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
        text_MoveLeft.GetComponent<MeshRenderer>().sortingOrder = order + 2;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        // 유연 각인 카드는 이동 버튼이 뜨고 이동시킬 수 있다.
        if (S_GameFlowManager.Instance.IsInState(VALID_STATES))
        {
            if (CardInfo.Engraving == S_EngravingEnum.Flexible)
            {
                SetAlphaValueBtn(1f, POINTER_ENTER_ANIMATION_TIME);
            }
        }
    }
    public override void ForceExit()
    {
        base.ForceExit();

        SetAlphaValueBtn(0f, POINTER_ENTER_ANIMATION_TIME);
    }

    public void SetAlphaValueBtn(float value, float duration)
    {
        sprite_MoveRightBtn.DOKill();
        text_MoveRight.DOKill();
        sprite_MoveLeftBtn.DOKill();
        text_MoveLeft.DOKill();

        sprite_MoveRightBtn.DOFade(value, duration);
        text_MoveRight.DOFade(value, duration);
        sprite_MoveLeftBtn.DOFade(value, duration);
        text_MoveLeft.DOFade(value, duration);
    }

    public async void ClickMoveRightBtn()
    {
        S_PlayerCard.Instance.SwapCardObjIndex(CardInfo, true);
        await S_StackInfoSystem.Instance.SwapCardObjIndex(CardInfo, true);
    }
    public async void ClickMoveLeftBtn()
    {
        S_PlayerCard.Instance.SwapCardObjIndex(CardInfo, false);
        await S_StackInfoSystem.Instance.SwapCardObjIndex(CardInfo, false);
    }
}
