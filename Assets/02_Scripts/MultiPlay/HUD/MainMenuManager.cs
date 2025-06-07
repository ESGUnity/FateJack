using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LobbyTypeEnum
{
    None = 0,
    Public = 1,
    Private = 2
}

public class MainMenuManager : MonoBehaviour
{
    //LobbyTypeEnum _lobbyType;
    float transitionDuration = 0.2f;

    GameObject mainMenuBase;

    GameObject multiGameBase;
    GameObject hostBase;
    GameObject lobbyPublicBtn;
    GameObject lobbyPrivateBtn;
    TMP_InputField lobbyNameInputField;
    TMP_InputField joinCodeInputField;
    GameObject joinCodeBase;

    GameObject transitionPanel;

    GameObject loadingPanel;
    TMP_Text loadingText;

    GameObject tutorialBtnBase;
    TMP_Text text_HighTrial;

    // 싱글턴
    static MainMenuManager instance;
    public static MainMenuManager Instance { get { return instance; } }
    void Awake()
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
        //_lobbyType = LobbyTypeEnum.None;

        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        mainMenuBase = Array.Find(transforms, c => c.gameObject.name.Equals("MainMenuBase")).gameObject;

        multiGameBase = Array.Find(transforms, c => c.gameObject.name.Equals("MultiGameBase")).gameObject;
        hostBase = Array.Find(transforms, c => c.gameObject.name.Equals("HostBase")).gameObject;
        lobbyPublicBtn = Array.Find(transforms, c => c.gameObject.name.Equals("LobbyPublicBtn")).gameObject;
        lobbyPrivateBtn = Array.Find(transforms, c => c.gameObject.name.Equals("LobbyPrivateBtn")).gameObject;
        lobbyNameInputField = Array.Find(transforms, c => c.gameObject.name.Equals("LobbyNameInputField")).gameObject.GetComponent<TMP_InputField>();
        joinCodeInputField = Array.Find(transforms, c => c.gameObject.name.Equals("JoinCodeInputField")).gameObject.GetComponent<TMP_InputField>();
        joinCodeBase = Array.Find(transforms, c => c.gameObject.name.Equals("JoinCodeBase")).gameObject;
        multiGameBase.SetActive(false);

        transitionPanel = Array.Find(transforms, c => c.gameObject.name.Equals("TransitionPanel")).gameObject;
        transitionPanel.SetActive(false);

        loadingPanel = Array.Find(transforms, c => c.gameObject.name.Equals("LoadingPanel")).gameObject;
        loadingText = Array.Find(transforms, c => c.gameObject.name.Equals("LoadingText")).gameObject.GetComponent<TMP_Text>();
        loadingPanel.SetActive(false);

        tutorialBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("TutorialBtnBase")).gameObject;
        text_HighTrial = Array.Find(transforms, c => c.gameObject.name.Equals("Text_HighTrial")).gameObject.GetComponent<TMP_Text>();


        // 튜토리얼 여부
        if (PlayerPrefs.HasKey("TutorialCompleted"))
        {
            tutorialBtnBase.SetActive(true);
        }
        else
        {
            tutorialBtnBase.SetActive(false);
        }

        // 최고 시련
        text_HighTrial.text = $"등반한 최고 시련 : {PlayerPrefs.GetInt("HighTrial"), 0}";
    }
    void Update()
    {
        
    }
    void DoTransition(GameObject prev, GameObject next)
    {
        transitionPanel.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append
            (
                transitionPanel.GetComponent<Image>()
                .DOColor(new Color(0f, 0f, 0f, 1f), transitionDuration).SetEase(Ease.OutQuart))
                .AppendCallback(() => prev.SetActive(false))
                .AppendInterval(0.1f)
                .AppendCallback(() => next.SetActive(true))
                .Append(transitionPanel.GetComponent<Image>()
                .DOColor(new Color(0f, 0f, 0f, 0f), transitionDuration).SetEase(Ease.OutQuart)
                .OnComplete(() => transitionPanel.SetActive(false))
            );
    }
    public void PressTutorialBtn()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("SingleGameScene");
    }
    public void PressSingleGameBtn()
    {
        if (PlayerPrefs.HasKey("TutorialCompleted"))
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("SingleGameScene");
    }
    public void PressOptionBtn()
    {
        Debug.Log("옵션");
    }
    public void PressExitBtn()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    // 멀티 코드
    //public void PressMultiGameBtn()
    //{
    //    DoTransition(mainMenuBase, multiGameBase);
    //}
    //public void PressLobbyTypeBtn(int lobbyType)
    //{
    //    if (_lobbyType == (LobbyTypeEnum)lobbyType)
    //    {
    //        _lobbyType = LobbyTypeEnum.None;
    //    }
    //    else
    //    {
    //        _lobbyType = (LobbyTypeEnum)lobbyType;
    //    }
    //}
    //public void PressHostBtn()
    //{
    //    if (hostBase.activeInHierarchy)
    //    {
    //        hostBase.SetActive(false);
    //    }
    //    else
    //    {
    //        hostBase.SetActive(true);
    //    }

    //    joinCodeBase.SetActive(false);
    //}
    //public async void PressHostStart()
    //{
    //    if (_lobbyType == LobbyTypeEnum.None)
    //    {
    //        Debug.Log("로비타입 설정하시오");
    //    }
    //    else if (string.IsNullOrEmpty(lobbyNameInputField.text))
    //    {
    //        Debug.Log("방제를 입력하시오");
    //    }
    //    else
    //    {
    //        loadingPanel.SetActive(true);
    //        loadingText.text = "방 생성 중...";
    //        await HostSingleton.Instance.StartHostAsync(_lobbyType, lobbyNameInputField.text);
    //        loadingPanel.SetActive(false);
    //    }
    //}
    //public void PressJoinCodeBtn()
    //{
    //    if (joinCodeBase.activeInHierarchy)
    //    {
    //        joinCodeBase.SetActive(false);
    //    }
    //    else
    //    {
    //        joinCodeBase.SetActive(true);
    //    }

    //    hostBase.SetActive(false);
    //}
    //public async void PressClientStart()
    //{
    //    loadingPanel.SetActive(true);
    //    loadingText.text = "접속 중...";
    //    await ClientSingleton.Instance.StartClientAsync(joinCodeInputField.text);
    //    loadingPanel.SetActive(false);
    //}

    //public void PressBackBtn(GameObject panel)
    //{
    //    DoTransition(panel, mainMenuBase);
    //}
    //public void EndEditInputField()
    //{
    //    lobbyNameInputField.text = lobbyNameInputField.text.Trim();
    //}
}
