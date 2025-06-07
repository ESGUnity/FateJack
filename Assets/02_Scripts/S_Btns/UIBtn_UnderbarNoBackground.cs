using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtn_UnderbarNoBackground : UIBtnAction, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image_RightBar;
    [SerializeField] Image image_LeftBar;
    [SerializeField] bool isExitBtn;


    public override void Start()
    {
        base.Start();

        image_RightBar.fillAmount = 0;
        image_LeftBar.fillAmount = 0;

        if (isExitBtn)
        {
            image_RightBar.fillOrigin = (int)Image.OriginHorizontal.Right;
            image_LeftBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            image_RightBar.fillOrigin = (int)Image.OriginHorizontal.Left;
            image_LeftBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isExitBtn)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(image_RightBar.DOFillAmount(0.85f, REACT_TIME).SetEase(Ease.OutQuart))
                .Join(image_LeftBar.DOFillAmount(0.85f, REACT_TIME).SetEase(Ease.OutQuart))
                .Join(text_BtnText.DOColor(enterTextColor, REACT_TIME).SetEase(Ease.OutQuart));
        }
        else
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(image_RightBar.DOFillAmount(1f, REACT_TIME).SetEase(Ease.OutQuart))
                .Join(image_LeftBar.DOFillAmount(1f, REACT_TIME).SetEase(Ease.OutQuart))
                .Join(text_BtnText.DOColor(enterTextColor, REACT_TIME).SetEase(Ease.OutQuart));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(image_RightBar.DOFillAmount(0f, REACT_TIME).SetEase(Ease.OutQuart))
            .Join(image_LeftBar.DOFillAmount(0f, REACT_TIME).SetEase(Ease.OutQuart))
            .Join(text_BtnText.DOColor(exitTextColor, REACT_TIME).SetEase(Ease.OutQuart));
    }
}
