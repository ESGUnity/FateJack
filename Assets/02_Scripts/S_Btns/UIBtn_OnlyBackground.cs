using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtn_OnlyBackground : UIBtnAction, IPointerEnterHandler, IPointerExitHandler
{
    // 컴포넌트
    [SerializeField] Image image_BtnBase;

    public override void Start()
    {
        base.Start();

        image_BtnBase.color = exitBtnBaseColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(image_BtnBase.DOColor(enterBtnBaseColor, REACT_TIME).SetEase(Ease.OutQuart))
            .Join(text_BtnText.DOColor(enterTextColor, REACT_TIME).SetEase(Ease.OutQuart));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(image_BtnBase.DOColor(exitBtnBaseColor, REACT_TIME).SetEase(Ease.OutQuart))
            .Join(text_BtnText.DOColor(exitTextColor, REACT_TIME).SetEase(Ease.OutQuart));
    }
    void OnDisable()
    {
        image_BtnBase.DOColor(exitBtnBaseColor, 0).SetEase(Ease.OutQuart);
        text_BtnText.DOColor(exitTextColor, 0).SetEase(Ease.OutQuart);
    }
}