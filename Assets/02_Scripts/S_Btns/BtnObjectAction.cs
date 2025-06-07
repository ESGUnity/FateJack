using TMPro;
using UnityEngine;

public class BtnObjectAction : MonoBehaviour
{
    [Header("공용 씬 오브젝트")]
    [SerializeField] protected TMP_Text text_BtnText;

    [Header("기본 상태")]
    protected Vector3 originPos;
    protected Vector3 originScale;
    protected const float REACT_TIME = 0.3f;
    protected Color enterTextColor = new Color(1f, 1f, 1f, 1f);
    protected Color exitTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    protected Color enterBtnBaseColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    protected Color exitBtnBaseColor = new Color(0.4f, 0.4f, 0.4f, 1f);


    public virtual void Start()
    {
        // 원본 상태 변수 초기화
        originPos = transform.localPosition;
        originScale = transform.localScale;

        text_BtnText.color = exitTextColor;
    }
}
