using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_DialogManager : MonoBehaviour
{
    [Header("�� ���̾�α� ����Ʈ")]
    [SerializeField] List<GameObject> dialogList = new();
    [SerializeField] List<GameObject> tutorialDialogList = new();

    [Header("�� ������Ʈ")]
    [SerializeField] GameObject sprite_WorldObjectBlockingBackground;
    [SerializeField] GameObject image_UIBlockingBackground;

    [Header("��� ��ư �� ������Ʈ")]
    [SerializeField] Canvas creatureInfoCanvas; // ������ (sortingOrder)
    [SerializeField] Canvas statInfoCanvas; // ���� (sortingOrder)
    [SerializeField] Image image_DeckBtnBase; // �� ���� ��ư (raycastTarget, sortingOrder)
    [SerializeField] Canvas lootInfoCanvas; // ����ǰ (sortingOrder)
    [SerializeField] Image image_BasicHitBtnBase; // ��Ʈ ��ư (raycastTarget, sortingOrder)
    [SerializeField] Image image_DeterminationHitBtnBase; // ���� ��Ʈ ��ư (raycastTarget, sortingOrder)
    [SerializeField] Image image_DeterminationHitBtnBaseByDeckInfo; // �� �������� �ϴ� ���� ��Ʈ ��ư (raycastTarget, sortingOrder)
    [SerializeField] Image image_TwistBtnBase; // ��Ʋ�� ��ư (raycastTarget, sortingOrder)
    [SerializeField] Image image_StandBtnBase; // ���ĵ� ��ư (raycastTarget, sortingOrder)

    // ���̾�α� �� ����
    S_GameFlowStateEnum prevState;
    [HideInInspector] public bool IsCompleteDialog;
    [HideInInspector] public S_DialogSystem CurrentDialogSystem;

    // �̱���
    static S_DialogManager instance;
    public static S_DialogManager Instance { get { return instance; } }
    void Awake()
    {
        sprite_WorldObjectBlockingBackground.SetActive(false);
        sprite_WorldObjectBlockingBackground.GetComponent<SpriteRenderer>().DOFade(0f, 0f);
        image_UIBlockingBackground.SetActive(false);
        image_UIBlockingBackground.GetComponent<SpriteRenderer>().DOFade(0f, 0f);

        SetBtn(S_ActivateUIEnum.OKBtn);

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

    public void StartDialog(int trial, S_DialogStateEnum state, bool isTutorial = false)
    {
        prevState = S_GameFlowManager.Instance.GameFlowState;
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Dialog;

        // ��׶��� ����
        sprite_WorldObjectBlockingBackground.SetActive(true);
        image_UIBlockingBackground.SetActive(true);
        image_UIBlockingBackground.GetComponent<Image>().DOFade(0.95f, 0.8f);

        GameObject dialogGo = null;

        if (isTutorial)
        {
            foreach (GameObject dialog in tutorialDialogList)
            {
                S_DialogSystem dia = dialog.GetComponent<S_DialogSystem>();

                if (dia.Trial == trial && dia.DialogState == state)
                {
                    dialogGo = Instantiate(dialog, transform);
                    CurrentDialogSystem = dialogGo.GetComponent<S_DialogSystem>();
                    CurrentDialogSystem.SetUp();
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject dialog in dialogList)
            {
                S_DialogSystem dia = dialog.GetComponent<S_DialogSystem>();

                if (dia.Trial == trial && dia.DialogState == state)
                {
                    dialogGo = Instantiate(dialog, transform);
                    CurrentDialogSystem = dialogGo.GetComponent<S_DialogSystem>();
                    CurrentDialogSystem.SetUp();
                    break;
                }
            }
        }

        // ��ư �����ϱ�
        SetBtn(CurrentDialogSystem.GetActiveBtn());

        if (dialogGo != null)
        {
            StartCoroutine(DialogCoroutine());
        }
        else
        {
            S_GameFlowManager.Instance.GameFlowState = prevState;
        }
    }
    IEnumerator DialogCoroutine()
    {
        yield return null;

        while (!IsCompleteDialog)
        {
            yield return null;
        }
        IsCompleteDialog = false;

        // �����÷ο� ���¸� Dialog���� ������� ������
        S_GameFlowManager.Instance.GameFlowState = prevState;
        Debug.Log(S_GameFlowManager.Instance.GameFlowState.ToString());

        // ��׶��� ���ֱ�
        sprite_WorldObjectBlockingBackground.SetActive(false);
        image_UIBlockingBackground.SetActive(false);
        image_UIBlockingBackground.GetComponent<Image>().DOFade(0f, 0.8f);

        yield return new WaitForSeconds(0.3f);

        // ��ư �������
        SetBtn(S_ActivateUIEnum.OKBtn);

        Debug.Log(S_GameFlowManager.Instance.GameFlowState.ToString());

        S_TutorialManager.Instance.IsCompleteDialog = true;
    }
    public void SetBtn(S_ActivateUIEnum btn)
    {
        // �� UI�� �⺻�� ����
        creatureInfoCanvas.sortingLayerName = "UI";
        creatureInfoCanvas.sortingOrder = 0;

        statInfoCanvas.sortingLayerName = "UI";
        statInfoCanvas.sortingOrder = 0;

        image_DeckBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_DeckBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        lootInfoCanvas.sortingLayerName = "UI";
        lootInfoCanvas.sortingOrder = 0;

        image_BasicHitBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_BasicHitBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        image_DeterminationHitBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_DeterminationHitBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        image_DeterminationHitBtnBaseByDeckInfo.GetComponent<Canvas>().sortingLayerName = "UI";
        image_DeterminationHitBtnBaseByDeckInfo.GetComponent<Canvas>().sortingOrder = 0;

        image_TwistBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_TwistBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        image_StandBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_StandBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        // Ȱ��ȭ�� ��ư ����
        switch (btn)
        {
            case S_ActivateUIEnum.OKBtn:
                break;
            case S_ActivateUIEnum.CreatureCanvas:
                creatureInfoCanvas.sortingLayerName = "Dialog";
                creatureInfoCanvas.sortingOrder = 1;
                break;
            case S_ActivateUIEnum.StatCanvas:
                statInfoCanvas.sortingLayerName = "Dialog";
                statInfoCanvas.sortingOrder = 1;
                break;
            case S_ActivateUIEnum.DeckInfoBtn:
                image_DeckBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_DeckBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateUIEnum.LootCanvas:
                lootInfoCanvas.sortingLayerName = "Dialog";
                lootInfoCanvas.sortingOrder = 1;
                break;
            case S_ActivateUIEnum.HitBtn:
                image_BasicHitBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_BasicHitBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateUIEnum.DeterminationHitBtn:
                image_DeterminationHitBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_DeterminationHitBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateUIEnum.DeterminationHitBtnByDeckInfo:
                image_DeterminationHitBtnBaseByDeckInfo.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_DeterminationHitBtnBaseByDeckInfo.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateUIEnum.TwistBtn:
                image_TwistBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_TwistBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateUIEnum.StandBtn:
                image_StandBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_StandBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
        }
    }
    public void BlockAllClick()
    {
        sprite_WorldObjectBlockingBackground.SetActive(true);
        image_UIBlockingBackground.SetActive(true);
        image_UIBlockingBackground.GetComponent<Image>().DOFade(0f, 0f);
    }
}
public enum S_DialogStateEnum
{
    None,
    StartTrial,
    ExplainUniverse,
    Creature,
    Stat,
    Deck,
    CardInfo,
    Loot,
    Hit,
    DeterminationHit,
    Stand,
    Twist,
    CardProduct,
    LootProduct,
    StatProduct
}
public enum S_ActivateUIEnum
{
    OKBtn,
    CreatureCanvas,
    StatCanvas,
    DeckInfoBtn,
    LootCanvas,
    HitBtn,
    DeterminationHitBtn,
    DeterminationHitBtnByDeckInfo,
    TwistBtn,
    StandBtn
}