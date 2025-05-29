using TMPro;
using UnityEngine;

public class S_ResultBase : MonoBehaviour
{
    [SerializeField] TMP_Text text_ResultTitle;
    [SerializeField] TMP_Text text_ResultValue;

    public void SetResultBase(string title, string value)
    {
        text_ResultTitle.text = title;  
        text_ResultValue.text = value;
    }
}
