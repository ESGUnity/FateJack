using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_InGameUISystem : MonoBehaviour
{
    [Header("컴포넌트")]
    GameObject panel_CurrentTurnBase;
    Image image_CurrentTurnBase;
    TMP_Text text_CurrentTurn;

    [Header("프리팹")]
    [SerializeField] GameObject logPrefab;

    [Header("로그")]
    Vector2 logPos = new Vector2(0, -233);

    [Header("현재 턴 UI")]
    Vector2 currentTurnUIHidePos = new Vector2(0, -120);
    Vector2 currentTurnUIOriginPos = new Vector2(0, 0);
    const float CURRENT_TURN_UI_APPEAR_TIME = 0.5f;
    const float CURRENT_TURN_UI_WAIT_TIME = 0.2f;

    // 싱글턴
    static S_InGameUISystem instance;
    public static S_InGameUISystem Instance { get { return instance; } }
    void Awake()
    {
        // 자식 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        Image[] images = GetComponentsInChildren<Image>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // 컴포넌트 할당
        panel_CurrentTurnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_CurrentTurnBase")).gameObject;
        image_CurrentTurnBase = Array.Find(images, c => c.gameObject.name.Equals("Image_CurrentTurnBase"));
        text_CurrentTurn = Array.Find(texts, c => c.gameObject.name.Equals("Text_CurrentTurn"));

        // 턴 표시 UI의 레이캐스트 막기
        image_CurrentTurnBase.raycastTarget = false;
        text_CurrentTurn.raycastTarget = false;

        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitPos();
    }
    public void InitPos()
    {
        panel_CurrentTurnBase.GetComponent<RectTransform>().anchoredPosition = currentTurnUIHidePos;
    }
    public void AppearCurrentTurnUI()
    {
        panel_CurrentTurnBase.GetComponent<RectTransform>().anchoredPosition = currentTurnUIHidePos;
        image_CurrentTurnBase.DOFade(0f, 0f);
        text_CurrentTurn.DOFade(0f, 0f);
        text_CurrentTurn.text = $"{S_GameFlowManager.Instance.CurrentTurn} 턴";

        panel_CurrentTurnBase.SetActive(true);
        panel_CurrentTurnBase.GetComponent<RectTransform>().DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(panel_CurrentTurnBase.GetComponent<RectTransform>().DOAnchorPos(currentTurnUIOriginPos, CURRENT_TURN_UI_APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(image_CurrentTurnBase.DOFade(0.8f, CURRENT_TURN_UI_APPEAR_TIME).SetEase(Ease.OutQuart))
            .Join(text_CurrentTurn.DOFade(1f, CURRENT_TURN_UI_APPEAR_TIME).SetEase(Ease.OutQuart))
            .AppendInterval(CURRENT_TURN_UI_WAIT_TIME)
            .Append(panel_CurrentTurnBase.GetComponent<RectTransform>().DOAnchorPos(currentTurnUIHidePos, CURRENT_TURN_UI_APPEAR_TIME).SetEase(Ease.InQuart))
            .Join(image_CurrentTurnBase.DOFade(0f, CURRENT_TURN_UI_APPEAR_TIME).SetEase(Ease.InQuart))
            .Join(text_CurrentTurn.DOFade(0f, CURRENT_TURN_UI_APPEAR_TIME).SetEase(Ease.InQuart))
            .OnComplete(() => panel_CurrentTurnBase.SetActive(false));
    }
    public void CreateLog(string log) // 로그 생성
    {
        // 로그 인스턴스 생성
        GameObject go = Instantiate(logPrefab, transform);

        // 로그 텍스트 설정
        TMP_Text logText = go.GetComponent<TMP_Text>();
        logText.raycastTarget = false;
        logText.text = log;
        logText.GetComponent<RectTransform>().anchoredPosition = logPos;

        // 로그 애니메이션 설정
        Sequence logSequence = DOTween.Sequence();
        logSequence.Append(logText.GetComponent<RectTransform>().DOAnchorPos(logPos + new Vector2(0, 70), 2.5f))
           .Insert(1f, logText.DOFade(0f, 1.5f));
    }
}
