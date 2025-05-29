using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    // 현재 라운드 UI 관련
    GameObject currentRoundPanel;
    TMP_Text currentRoundText;
    Vector2 currentRoundPanelOriginPos = new Vector3(0, 600);
    Vector2 currentRoundPanelTargetPos = new Vector3(0, 460);
    const float CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME = 0.3f;

    // 로그 관련
    [SerializeField] GameObject logPrefab;
    Vector2 logPos = new Vector2(0, -233);

    // 싱글턴
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
        // 트랜스폼, 텍스트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] tmps = GetComponentsInChildren<TMP_Text>(true);

        // 현재 라운드 UI 관련 변수
        currentRoundPanel = Array.Find(transforms, c => c.gameObject.name.Equals("CurrentRoundPanel")).gameObject;
        currentRoundText = Array.Find(tmps, c => c.gameObject.name.Equals("CurrentRoundText"));
    }

    public void DisplayPhrase(string phrase) // 문구 UI 띄우기
    {
        currentRoundPanel.SetActive(true);
        currentRoundPanel.GetComponent<RectTransform>().anchoredPosition = currentRoundPanelOriginPos;
        currentRoundText.text = phrase;

        currentRoundPanel.GetComponent<RectTransform>().DOKill(true);

        currentRoundPanel.GetComponent<RectTransform>()
            .DOAnchorPos(currentRoundPanelTargetPos, CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME)
            .SetEase(Ease.OutQuart);
    }

    public void EndDisplayPhrase() // 문구 UI 숨기기
    {
        currentRoundPanel.GetComponent<RectTransform>().DOKill(true);

        currentRoundPanel.GetComponent<RectTransform>()
            .DOAnchorPos(currentRoundPanelOriginPos, CURRENT_ROUND_UI_APPEAR_DISAPPEAR_TIME)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => currentRoundPanel.SetActive(false));
    }
    public void CreateLog(string log) // 게임 정보를 알리는 로그 날리는 메서드
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
