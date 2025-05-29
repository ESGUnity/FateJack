using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class S_TutorialManager : MonoBehaviour
{
    // ������
    [SerializeField] List<S_TutorialBase> tutorialList;

    // ���̾�α� �� ����
    public bool IsCompleteDialog;

    // �̱���
    static S_TutorialManager instance;
    public static S_TutorialManager Instance { get { return instance; } }
    void Awake()
    {
        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTutorial()
    {
        StartCoroutine(TutorialCoroutine());
    }

    IEnumerator TutorialCoroutine()
    {
        ////////////////////////////////////////////////////////////////////////////////// StartTrial

        // ����� ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.ExplainUniverse, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ������ ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Creature, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ���� ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Stat, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // �� ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Deck, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // �� �Ѿ �ð����� ���
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 1.5f);

        // ī�� ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.CardInfo, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ���� �� �ѷ����� ���� �� ������ ���
        while (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {
            yield return null;
        }

        // ����ǰ ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Loot, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ��Ʈ ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Hit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ��Ʈ �� ������ ���� ������ֱ�
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 4f);

        // ��Ʈ ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(2, S_DialogStateEnum.Hit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ���� ��Ʈ ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.DeterminationHit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ������ �Ѿ ������ ���� ������ֱ�
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 1f);

        // ���� ��Ʈ ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(2, S_DialogStateEnum.DeterminationHit, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ���� ��Ʈ�� �ϰų� ����� ������ ���
        while (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.Hit)
        {
            yield return null;
        }

        // ��Ʋ�� ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Twist, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ��Ʋ�� ���� ���� ������ ���
        S_DialogManager.Instance.BlockAllClick();
        yield return new WaitForSeconds(S_GameFlowManager.PANEL_APPEAR_TIME * 1f);

        // ��Ʋ�� ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(2, S_DialogStateEnum.Twist, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // ���ĵ� ���� ���̾�α� ����
        S_DialogManager.Instance.StartDialog(1, S_DialogStateEnum.Stand, true);
        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // Ʃ�丮�� �Ϸ�
        CompletedAllTutorial();
    }

    public void CompletedAllTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1); // Ʃ�丮�� �ϼ������� �˸��� ����
        PlayerPrefs.Save();
    }
}
