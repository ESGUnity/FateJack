using TMPro;
using UnityEngine;

public class BtnAction : MonoBehaviour
{
    // �Ҵ� �ʿ�
    [SerializeField] protected TMP_Text text_BtnText;

    // ������Ʈ
    protected RectTransform rectTransform;

    // �⺻ ����
    protected Vector3 originPos;
    protected Vector3 originScale;
    protected const float REACT_TIME = 0.3f;
    protected Color enterTextColor = Color.white;
    protected Color exitTextColor = new Color(0.7f, 0.7f, 0.7f);
    protected Color enterBtnBaseColor = new Color(0.6f, 0.6f, 0.6f);
    protected Color exitBtnBaseColor = new Color(0.4f, 0.4f, 0.4f);
    

    public virtual void Start()
    {
        // ���� ���� ���� �ʱ�ȭ
        rectTransform = GetComponent<RectTransform>();
        originPos = rectTransform.anchoredPosition;
        originScale = rectTransform.localScale;

        text_BtnText.color = exitTextColor;
    }
}
