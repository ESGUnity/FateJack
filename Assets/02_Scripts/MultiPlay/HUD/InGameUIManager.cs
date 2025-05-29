using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    // ���� ���� UI ����
    GameObject currentRoundPanel;
    TMP_Text currentRoundText;
    Vector2 currentRoundPanelOriginPos = new Vector3(0, 600);
    Vector2 currentRoundPanelTargetPos = new Vector3(0, 460);
    const float CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME = 0.3f;

    // �α� ����
    [SerializeField] GameObject logPrefab;
    Vector2 logPos = new Vector2(0, -233);

    // �̱���
    static InGameUIManager instance;
    public static InGameUIManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // Ʈ������, �ؽ�Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] tmps = GetComponentsInChildren<TMP_Text>(true);

        // ���� ���� UI ���� ����
        currentRoundPanel = Array.Find(transforms, c => c.gameObject.name.Equals("CurrentRoundPanel")).gameObject;
        currentRoundText = Array.Find(tmps, c => c.gameObject.name.Equals("CurrentRoundText"));
    }

    public void DisplayPhrase(string phrase) // ���� UI ����
    {
        currentRoundPanel.SetActive(true);
        currentRoundPanel.GetComponent<RectTransform>().anchoredPosition = currentRoundPanelOriginPos;
        currentRoundText.text = phrase;

        currentRoundPanel.GetComponent<RectTransform>().DOKill(true);

        currentRoundPanel.GetComponent<RectTransform>()
            .DOAnchorPos(currentRoundPanelTargetPos, CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME)
            .SetEase(Ease.OutQuart);
    }

    public void EndDisplayPhrase() // ���� UI �����
    {
        currentRoundPanel.GetComponent<RectTransform>().DOKill(true);

        currentRoundPanel.GetComponent<RectTransform>()
            .DOAnchorPos(currentRoundPanelOriginPos, CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => currentRoundPanel.SetActive(false));
    }
    public void CreateLog(string log) // ���� ������ �˸��� �α� ������ �޼���
    {
        GameObject go = Instantiate(logPrefab, transform);

        TMP_Text logText = go.GetComponent<TMP_Text>();
        logText.raycastTarget = false;
        logText.text = log;

        logText.GetComponent<RectTransform>().anchoredPosition = logPos;

        Sequence logSequence = DOTween.Sequence();
        logSequence.Append(logText.GetComponent<RectTransform>().DOAnchorPos(logPos + new Vector2(0, 70), 2.5f))
           .Insert(1f, logText.DOFade(0f, 1.5f));
    }
}
