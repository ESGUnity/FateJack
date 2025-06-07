using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnObject_OnlyBackground : BtnObjectAction, IPointerEnterHandler, IPointerExitHandler
{
    [Header("배경있는 버튼 씬 오브젝트")]
    [SerializeField] SpriteRenderer sprite_BtnBase;

    public override void Start()
    {
        base.Start();

        sprite_BtnBase.color = exitBtnBaseColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sprite_BtnBase.DOKill();
        text_BtnText.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(sprite_BtnBase.DOColor(enterBtnBaseColor, REACT_TIME).SetEase(Ease.OutQuart))
            .Join(text_BtnText.DOColor(enterTextColor, REACT_TIME).SetEase(Ease.OutQuart));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sprite_BtnBase.DOKill();
        text_BtnText.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(sprite_BtnBase.DOColor(exitBtnBaseColor, REACT_TIME).SetEase(Ease.OutQuart))
            .Join(text_BtnText.DOColor(exitTextColor, REACT_TIME).SetEase(Ease.OutQuart));
    }
}
