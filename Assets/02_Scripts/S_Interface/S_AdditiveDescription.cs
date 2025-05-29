using TMPro;
using UnityEngine;

public class S_AdditiveDescription : MonoBehaviour
{
    [SerializeField] TMP_Text text_AdditiveDescriptionTitle;
    [SerializeField] TMP_Text text_AdditiveDescription;

    public void SetAdditiveDescriptionText(string text)
    {
        switch (text)
        {
            case "잔류":
                text_AdditiveDescriptionTitle.text = "잔류(@@)";
                text_AdditiveDescription.text = "이 카드가 히트될 때, 직전에 히트한 카드와 이 카드가 @@라면 발동합니다.";
                break;
            case "발현":
                text_AdditiveDescriptionTitle.text = "발현";
                text_AdditiveDescription.text = "이 카드가 히트될 때 발동합니다.";
                break;
            case "예견":
                text_AdditiveDescriptionTitle.text = "예견(@@)";
                text_AdditiveDescription.text = "이 카드가 히트될 때, 다음에 히트한 카드와 이 카드가 @@라면 발동합니다.";
                break;
            case "범람":
                text_AdditiveDescriptionTitle.text = "범람(@@, X)";
                text_AdditiveDescription.text = "이 카드가 히트될 때, 스택에 @@인 카드가 X 이상있다면 발동합니다.";
                break;
            case "메아리":
                text_AdditiveDescriptionTitle.text = "메아리(@@)";
                text_AdditiveDescription.text = "@@인 카드를 히트할 때마다 발동합니다.";
                break;
            case "대미장식":
                text_AdditiveDescriptionTitle.text = "대미장식";
                text_AdditiveDescription.text = "덱에서 한 문양의 카드가 모두 없어질 때마다 발동합니다.";
                break;
            case "이탈":
                text_AdditiveDescriptionTitle.text = "이탈";
                text_AdditiveDescription.text = "카드가 제외될 때마다 발동합니다.";
                break;
            case "스탠드":
                text_AdditiveDescriptionTitle.text = "스탠드";
                text_AdditiveDescription.text = "스탠드를 할 때마다 발동합니다.";
                break;
            case "영원":
                text_AdditiveDescriptionTitle.text = "영원(능력치, X)";
                text_AdditiveDescription.text = "기본 능력치 +X";
                break;
            case "추가":
                text_AdditiveDescriptionTitle.text = "추가(능력치, X)";
                text_AdditiveDescription.text = "추가 능력치 +X";
                break;
            case "배율":
                text_AdditiveDescriptionTitle.text = "배율(능력치, X)";
                text_AdditiveDescription.text = "배율 능력치 +X";
                break;
            case "조작":
                text_AdditiveDescriptionTitle.text = "조작";
                text_AdditiveDescription.text = "숫자 합을 바꿉니다.";
                break;
            case "저항":
                text_AdditiveDescriptionTitle.text = "저항";
                text_AdditiveDescription.text = "한계를 바꿉니다.";
                break;
            case "피해":
                text_AdditiveDescriptionTitle.text = "피해";
                text_AdditiveDescription.text = "피조물에 데미지를 줍니다.";
                break;
            case "회복":
                text_AdditiveDescriptionTitle.text = "회복";
                text_AdditiveDescription.text = "체력을 얻습니다.";
                break;
            case "단련":
                text_AdditiveDescriptionTitle.text = "단련";
                text_AdditiveDescription.text = "의지를 얻습니다.";
                break;
            case "약탈":
                text_AdditiveDescriptionTitle.text = "약탈";
                text_AdditiveDescription.text = "골드를 얻습니다.";
                break;
            case "창조":
                text_AdditiveDescriptionTitle.text = "창조";
                text_AdditiveDescription.text = "카드를 새롭게 생성한 후 히트합니다.";
                break;
            case "역류":
                text_AdditiveDescriptionTitle.text = "역류";
                text_AdditiveDescription.text = "스택이나 덱에서 카드를 복사하여 생성한 후 히트합니다.";
                break;
            case "인도":
                text_AdditiveDescriptionTitle.text = "인도";
                text_AdditiveDescription.text = "덱에서 특정 카드를 히트합니다.";
                break;
            case "환상":
                text_AdditiveDescriptionTitle.text = "환상";
                text_AdditiveDescription.text = "이번 시련이 끝나면 소멸됩니다.";
                break;
            case "제외":
                text_AdditiveDescriptionTitle.text = "제외";
                text_AdditiveDescription.text = "이번 시련에서 이 카드를 사용할 수 없습니다.";
                break;
            case "저주":
                text_AdditiveDescriptionTitle.text = "저주";
                text_AdditiveDescription.text = "카드의 효과가 발동하지 않습니다.";
                break;
            case "저주해제":
                text_AdditiveDescriptionTitle.text = "저주해제";
                text_AdditiveDescription.text = "카드의 저주를 해제합니다.";
                break;
            case "우선":
                text_AdditiveDescriptionTitle.text = "우선";
                text_AdditiveDescription.text = "다음에 히트할 카드를 특정합니다.";
                break;
            case "망상":
                text_AdditiveDescriptionTitle.text = "망상";
                text_AdditiveDescription.text = "다음에 히트할 카드를 저주합니다.";
                break;
            case "소멸":
                text_AdditiveDescriptionTitle.text = "소멸";
                text_AdditiveDescription.text = "덱에서 제거됩니다.";
                break;
            default:
                text_AdditiveDescriptionTitle.text = "-";
                text_AdditiveDescription.text = "-";
                break;
        }
    }
}