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
            case "�ܷ�":
                text_AdditiveDescriptionTitle.text = "�ܷ�(@@)";
                text_AdditiveDescription.text = "�� ī�尡 ��Ʈ�� ��, ������ ��Ʈ�� ī��� �� ī�尡 @@��� �ߵ��մϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "�� ī�尡 ��Ʈ�� �� �ߵ��մϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����(@@)";
                text_AdditiveDescription.text = "�� ī�尡 ��Ʈ�� ��, ������ ��Ʈ�� ī��� �� ī�尡 @@��� �ߵ��մϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����(@@, X)";
                text_AdditiveDescription.text = "�� ī�尡 ��Ʈ�� ��, ���ÿ� @@�� ī�尡 X �̻��ִٸ� �ߵ��մϴ�.";
                break;
            case "�޾Ƹ�":
                text_AdditiveDescriptionTitle.text = "�޾Ƹ�(@@)";
                text_AdditiveDescription.text = "@@�� ī�带 ��Ʈ�� ������ �ߵ��մϴ�.";
                break;
            case "������":
                text_AdditiveDescriptionTitle.text = "������";
                text_AdditiveDescription.text = "������ �� ������ ī�尡 ��� ������ ������ �ߵ��մϴ�.";
                break;
            case "��Ż":
                text_AdditiveDescriptionTitle.text = "��Ż";
                text_AdditiveDescription.text = "ī�尡 ���ܵ� ������ �ߵ��մϴ�.";
                break;
            case "���ĵ�":
                text_AdditiveDescriptionTitle.text = "���ĵ�";
                text_AdditiveDescription.text = "���ĵ带 �� ������ �ߵ��մϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����(�ɷ�ġ, X)";
                text_AdditiveDescription.text = "�⺻ �ɷ�ġ +X";
                break;
            case "�߰�":
                text_AdditiveDescriptionTitle.text = "�߰�(�ɷ�ġ, X)";
                text_AdditiveDescription.text = "�߰� �ɷ�ġ +X";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����(�ɷ�ġ, X)";
                text_AdditiveDescription.text = "���� �ɷ�ġ +X";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "���� ���� �ٲߴϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "�Ѱ踦 �ٲߴϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "�������� �������� �ݴϴ�.";
                break;
            case "ȸ��":
                text_AdditiveDescriptionTitle.text = "ȸ��";
                text_AdditiveDescription.text = "ü���� ����ϴ�.";
                break;
            case "�ܷ�":
                text_AdditiveDescriptionTitle.text = "�ܷ�";
                text_AdditiveDescription.text = "������ ����ϴ�.";
                break;
            case "��Ż":
                text_AdditiveDescriptionTitle.text = "��Ż";
                text_AdditiveDescription.text = "��带 ����ϴ�.";
                break;
            case "â��":
                text_AdditiveDescriptionTitle.text = "â��";
                text_AdditiveDescription.text = "ī�带 ���Ӱ� ������ �� ��Ʈ�մϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "�����̳� ������ ī�带 �����Ͽ� ������ �� ��Ʈ�մϴ�.";
                break;
            case "�ε�":
                text_AdditiveDescriptionTitle.text = "�ε�";
                text_AdditiveDescription.text = "������ Ư�� ī�带 ��Ʈ�մϴ�.";
                break;
            case "ȯ��":
                text_AdditiveDescriptionTitle.text = "ȯ��";
                text_AdditiveDescription.text = "�̹� �÷��� ������ �Ҹ�˴ϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "�̹� �÷ÿ��� �� ī�带 ����� �� �����ϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "ī���� ȿ���� �ߵ����� �ʽ��ϴ�.";
                break;
            case "��������":
                text_AdditiveDescriptionTitle.text = "��������";
                text_AdditiveDescription.text = "ī���� ���ָ� �����մϴ�.";
                break;
            case "�켱":
                text_AdditiveDescriptionTitle.text = "�켱";
                text_AdditiveDescription.text = "������ ��Ʈ�� ī�带 Ư���մϴ�.";
                break;
            case "����":
                text_AdditiveDescriptionTitle.text = "����";
                text_AdditiveDescription.text = "������ ��Ʈ�� ī�带 �����մϴ�.";
                break;
            case "�Ҹ�":
                text_AdditiveDescriptionTitle.text = "�Ҹ�";
                text_AdditiveDescription.text = "������ ���ŵ˴ϴ�.";
                break;
            default:
                text_AdditiveDescriptionTitle.text = "-";
                text_AdditiveDescription.text = "-";
                break;
        }
    }
}