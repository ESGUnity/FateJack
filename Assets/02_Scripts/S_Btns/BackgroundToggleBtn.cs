using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundToggleBtn : BtnAction
{
    [SerializeField] Image btn3Base;
    Color btn3EnterColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public bool IsBtn3Toggle = false;

    public override void Start()
    {
        base.Start();
    }

    public void Btn3Enter()
    {
        if (IsBtn3Toggle) return;

        btn3Base.DOColor(btn3EnterColor, REACT_TIME).SetEase(Ease.OutQuart);
        text_BtnText.DOColor(btn3EnterColor, REACT_TIME).SetEase(Ease.OutQuart);
    }
    public void Btn3Exit()
    {
        if (IsBtn3Toggle) return;

        btn3Base.DOColor(Color.gray, REACT_TIME).SetEase(Ease.OutQuart);
        text_BtnText.DOColor(Color.gray, REACT_TIME).SetEase(Ease.OutQuart);
    }
    public void Btn3Click()
    {
        if (IsBtn3Toggle)
        {
            IsBtn3Toggle = false;
            btn3Base.DOColor(Color.gray, REACT_TIME).SetEase(Ease.OutQuart);
            text_BtnText.DOColor(Color.gray, REACT_TIME).SetEase(Ease.OutQuart);
        }
        else
        {
            IsBtn3Toggle = true;
            btn3Base.DOColor(Color.white, REACT_TIME).SetEase(Ease.OutQuart);
            text_BtnText.DOColor(Color.white, REACT_TIME).SetEase(Ease.OutQuart);
        }
    }
}
