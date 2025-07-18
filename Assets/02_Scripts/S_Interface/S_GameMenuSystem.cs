using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_GameMenuSystem : MonoBehaviour
{
    [Header("컴포넌트")]
    GameObject image_BlackBackground;
    GameObject panel_GameMenuBase;
    GameObject image_SaveBtnBase;
    GameObject image_ForgiveBtnBase;
    GameObject image_ContinueBtnBase;



    [Header("씬 오브젝트")]
    [SerializeField] TMP_Dropdown dropdown_Resolution;
    [SerializeField] TMP_Dropdown dropdown_DisplayMode;
    [SerializeField] Slider slider_MasterVolume;
    [SerializeField] Slider slider_BGMVolume;
    [SerializeField] Slider slider_SFXVolume;
    [SerializeField] Slider slider_UIVolume;

    [Header("해상도 관련")]
    List<Resolution> resolutions = new List<Resolution>();
    int optimalResolutionIndex = 0;
    const string RESOLUTION_INDEX_KEY = "ResolutionIndex";

    [Header("창모드 관련")]
    List<string> displayModes = new List<string> { "전체화면", "창모드", "테두리 없는 창모드" };
    const string DISPLAY_MODE_INDEX_KEY = "DisplayModeIndex";

    // 싱글턴
    static S_GameMenuSystem instance;
    public static S_GameMenuSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        Slider[] sliders = GetComponentsInChildren<Slider>(true);

        image_BlackBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BlackBackground")).gameObject;
        panel_GameMenuBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_GameMenuBase")).gameObject;
        image_SaveBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_SaveBtnBase")).gameObject;
        image_ForgiveBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ForgiveBtnBase")).gameObject;
        image_ContinueBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ContinueBtnBase")).gameObject;

        // 싱글턴
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
        slider_MasterVolume.onValueChanged.AddListener(MasterVolumeValueChanged);
        slider_BGMVolume.onValueChanged.AddListener(BGMVolumeValueChanged);
        slider_SFXVolume.onValueChanged.AddListener(SFXVolumeValueChanged);
        slider_UIVolume.onValueChanged.AddListener(UIVolumeValueChanged);

        slider_MasterVolume.value = PlayerPrefs.GetFloat("MasterVolume");
        slider_BGMVolume.value = PlayerPrefs.GetFloat("BGMVolume");
        slider_SFXVolume.value = PlayerPrefs.GetFloat("SFXVolume");
        slider_UIVolume.value = PlayerPrefs.GetFloat("UIVolume");

        resolutions = new List<Resolution>
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1280, height = 800 },
        new Resolution { width = 1440, height = 900 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1680, height = 1050 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1920, height = 1200 },
        new Resolution { width = 2048, height = 1280 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 2560, height = 1600 },
        new Resolution { width = 2880, height = 1800 },
        new Resolution { width = 3480, height = 2160 },
    };

        int resolutionIndex = 0;
        int displayModeIndex = 1; // 창모드 기본

        bool hasSavedResolution = PlayerPrefs.HasKey(RESOLUTION_INDEX_KEY);
        bool hasSavedDisplayMode = PlayerPrefs.HasKey(DISPLAY_MODE_INDEX_KEY);

        if (hasSavedResolution && hasSavedDisplayMode)
        {
            // 저장된 값 불러오기
            resolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY);
            displayModeIndex = PlayerPrefs.GetInt(DISPLAY_MODE_INDEX_KEY);
        }
        else
        {
            // 처음 실행: 현재 해상도 기준 가장 가까운 걸 찾아서 설정
            int closestIndex = 0;
            int minDistance = int.MaxValue;

            int targetWidth = Screen.currentResolution.width;
            int targetHeight = Screen.currentResolution.height;

            for (int i = 0; i < resolutions.Count; i++)
            {
                int dw = resolutions[i].width - targetWidth;
                int dh = resolutions[i].height - targetHeight;
                int distance = dw * dw + dh * dh; // 유클리디안 거리의 제곱

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            resolutionIndex = closestIndex;
            // 처음 실행이므로 창모드 고정
            displayModeIndex = 1;
        }

        // 드롭다운 설정
        SetupResolutionDropdown(resolutionIndex);
        SetupDisplayModeDropdown(displayModeIndex);

        // 실제 적용
        ApplyResolution(resolutionIndex, displayModeIndex);

        dropdown_Resolution.onValueChanged.AddListener(OnResolutionDropdownChanged);
        dropdown_DisplayMode.onValueChanged.AddListener(OnDisplayModeDropdownChanged);
    }
    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "TitleScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (panel_GameMenuBase.activeInHierarchy || image_BlackBackground.activeInHierarchy)
                {
                    DisappearGameMenuPanel();
                }
                else
                {
                    AppearGameMenuPanel();
                }
            }
        }
        else if (currentScene.name == "SingleGameScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape) && S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.GameOver)
            {
                if (panel_GameMenuBase.activeInHierarchy || image_BlackBackground.activeInHierarchy)
                {
                    DisappearGameMenuPanel();
                }
                else
                {
                    AppearGameMenuPanel();
                }
            }
        }
    }

    public void AppearGameMenuPanel() // 패널 등장
    {
        panel_GameMenuBase.SetActive(true);
        image_BlackBackground.SetActive(true);
        image_BlackBackground.GetComponent<Image>().DOKill();
        image_BlackBackground.GetComponent<Image>().DOFade(0.85f, 0f);

        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "TitleScene")
        {
            image_SaveBtnBase.SetActive(true);
            image_ForgiveBtnBase.SetActive(false);
            image_ContinueBtnBase.SetActive(false);
        }
        else if (currentScene.name == "SingleGameScene")
        {
            image_SaveBtnBase.SetActive(false);
            image_ForgiveBtnBase.SetActive(true);
            image_ContinueBtnBase.SetActive(true);
        }

    }
    public void DisappearGameMenuPanel()
    {
        panel_GameMenuBase.SetActive(false);
        image_BlackBackground.SetActive(false);
        image_BlackBackground.GetComponent<Image>().DOFade(0f, 0f);

        image_SaveBtnBase.SetActive(false);
        image_ForgiveBtnBase.SetActive(false);
        image_ContinueBtnBase.SetActive(false);
    }

    #region 버튼 함수
    public void ClickForgiveBtn()
    {
        DisappearGameMenuPanel();
        S_GameOverSystem.Instance.AppearGameOverPanel();
    }
    public void ClickContinueBtn()
    {
        DisappearGameMenuPanel();
    }
    #endregion
    #region 해상도
    void SetupResolutionDropdown(int selectedIndex)
    {
        dropdown_Resolution.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Count; i++)
        {
            string label = $"{resolutions[i].width} x {resolutions[i].height}";
            if (i == selectedIndex) label += " *";
            options.Add(label);
        }
        dropdown_Resolution.AddOptions(options);
        dropdown_Resolution.value = selectedIndex;
        dropdown_Resolution.RefreshShownValue();
    }
    void SetupDisplayModeDropdown(int selectedIndex)
    {
        dropdown_DisplayMode.ClearOptions();
        dropdown_DisplayMode.AddOptions(displayModes);
        dropdown_DisplayMode.value = selectedIndex;
        dropdown_DisplayMode.RefreshShownValue();
    }
    void OnResolutionDropdownChanged(int newResIndex)
    {
        ApplyResolution(newResIndex, dropdown_DisplayMode.value);
    }
    void OnDisplayModeDropdownChanged(int newDisplayModeIndex)
    {
        ApplyResolution(dropdown_Resolution.value, newDisplayModeIndex);
    }
    void ApplyResolution(int resolutionIndex, int displayModeIndex)
    {
        var resolution = resolutions[resolutionIndex];

        FullScreenMode mode = FullScreenMode.Windowed;
        switch (displayModeIndex)
        {
            case 0: mode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: mode = FullScreenMode.Windowed; break;
            case 2: mode = FullScreenMode.FullScreenWindow; break;
        }

        Screen.SetResolution(resolution.width, resolution.height, mode);

        // 저장
        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, resolutionIndex);
        PlayerPrefs.SetInt(DISPLAY_MODE_INDEX_KEY, displayModeIndex);
        PlayerPrefs.Save();
    }
    #endregion
    #region 볼륨
    void MasterVolumeValueChanged(float value)
    {
        S_AudioManager.Instance.SetMasterVolume(value);
    }
    void BGMVolumeValueChanged(float value)
    {
        S_AudioManager.Instance.SetBGMVolume(value);
    }
    void SFXVolumeValueChanged(float value)
    {
        S_AudioManager.Instance.SetSFXVolume(value);
    }
    void UIVolumeValueChanged(float value)
    {
        S_AudioManager.Instance.SetUIVolume(value);
    }
    #endregion
}
