using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LobbyTypeEnum
{
    None = 0,
    Public = 1,
    Private = 2
}

public class MainMenuManager : MonoBehaviour
{
    GameObject image_TutorialBtnBase;
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
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        image_TutorialBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_TutorialBtnBase")).gameObject;
        text_HighTrial = Array.Find(transforms, c => c.gameObject.name.Equals("Text_HighTrial")).gameObject.GetComponent<TMP_Text>();

        // 튜토리얼 여부
        if (PlayerPrefs.HasKey("TutorialCompleted"))
        {
            image_TutorialBtnBase.SetActive(true);
        }
        else
        {
            image_TutorialBtnBase.SetActive(false);
        }

        // 최고 시련
        text_HighTrial.text = $"등반한 최고 시련 : {PlayerPrefs.GetInt("HighTrial"), 0}";

        // 브금 틀기
        S_AudioManager.Instance.PlayBGM(BGMEnum.Single);
    }

    public void PressTutorialBtn()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();

        S_LoadingSceneManager.LoadScene("SingleGameScene");
    }
    public void PressSingleGameBtn()
    {
        if (PlayerPrefs.HasKey("TutorialCompleted"))
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.Save();
        }

        S_LoadingSceneManager.LoadScene("SingleGameScene");
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
    //void DoTransition(GameObject prev, GameObject next)
    //{
    //    transitionPanel.SetActive(true);

    //    Sequence sequence = DOTween.Sequence();
    //    sequence.Append
    //        (
    //            transitionPanel.GetComponent<Image>()
    //            .DOColor(new Color(0f, 0f, 0f, 1f), transitionDuration).SetEase(Ease.OutQuart))
    //            .AppendCallback(() => prev.SetActive(false))
    //            .AppendInterval(0.1f)
    //            .AppendCallback(() => next.SetActive(true))
    //            .Append(transitionPanel.GetComponent<Image>()
    //            .DOColor(new Color(0f, 0f, 0f, 0f), transitionDuration).SetEase(Ease.OutQuart)
    //            .OnComplete(() => transitionPanel.SetActive(false))
    //        );
    //}
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
