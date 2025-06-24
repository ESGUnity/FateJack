using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_StackCardObj : S_CardObj
{
    [SerializeField] SpriteRenderer sprite_MoveRightBtn;
    [SerializeField] TMP_Text text_MoveRight;
    [SerializeField] SpriteRenderer sprite_MoveLeftBtn;
    [SerializeField] TMP_Text text_MoveLeft;

    const float LEAN_VALUE = 3.5f;
    Vector3 CARD_ROT;

    protected override void Awake()
    {
        VALID_STATES = new() { S_GameFlowStateEnum.Hit, S_GameFlowStateEnum.HittingCard };

        CARD_ROT = new Vector3(0, 0, Random.Range(-LEAN_VALUE, LEAN_VALUE));

        obj_Card.transform.DOLocalRotate(CARD_ROT, 0);
    }

    public override void SetOrder(int order)
    {
        base.SetOrder(order);

        if (CardInfo.Engraving == S_EngravingEnum.Flexible)
        {
            sprite_MoveRightBtn.gameObject.SetActive(true);
            sprite_MoveLeftBtn.gameObject.SetActive(true);
            sprite_MoveRightBtn.sortingLayerName = "WorldObject";
            sprite_MoveRightBtn.sortingOrder = order + 1;
            text_MoveRight.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
            text_MoveRight.GetComponent<MeshRenderer>().sortingOrder = order + 2;
            sprite_MoveLeftBtn.sortingLayerName = "WorldObject";
            sprite_MoveLeftBtn.sortingOrder = order + 1;
            text_MoveLeft.GetComponent<MeshRenderer>().sortingLayerName = "WorldObject";
            text_MoveLeft.GetComponent<MeshRenderer>().sortingOrder = order + 2;

            EventTrigger rightTrigger = sprite_MoveRightBtn.GetComponent<EventTrigger>();
            EventTrigger.Entry rightClickEntry = rightTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
            rightClickEntry.callback.AddListener((eventData) => { ClickMoveRightBtn((PointerEventData)eventData); });

            EventTrigger leftTrigger = sprite_MoveRightBtn.GetComponent<EventTrigger>();
            EventTrigger.Entry leftClickEntry = leftTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
            leftClickEntry.callback.AddListener((eventData) => { ClickMoveLeftBtn((PointerEventData)eventData); });
        }
        else
        {
            sprite_MoveRightBtn.gameObject.SetActive(false);
            sprite_MoveLeftBtn.gameObject.SetActive(false);
        }
    }
    public override void ForceExit()
    {
        base.ForceExit();

        obj_Card.transform.DOLocalRotate(CARD_ROT, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
    }
    public async void ClickMoveRightBtn(PointerEventData eventData)
    {
        S_PlayerCard.Instance.SwapCardObjIndex(CardInfo, true);
        await S_StackInfoSystem.Instance.SwapCardObjIndex(CardInfo, true);
    }
    public async void ClickMoveLeftBtn(PointerEventData eventData)
    {
        S_PlayerCard.Instance.SwapCardObjIndex(CardInfo, false);
        await S_StackInfoSystem.Instance.SwapCardObjIndex(CardInfo, false);
    }
}
