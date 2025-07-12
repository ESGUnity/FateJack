using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// 클라이언트에만 뜨면 되기에 네트워크가 아닌 모노비헤이비어로 생성
public class InGameMenuManager : MonoBehaviour
{
    [SerializeField] GameObject inGameMenuCanvas;
    [SerializeField] GameObject optionPanel;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider bGMVolumeSlider;
    [SerializeField] Slider sFXVolumeSlider;
    [SerializeField] Slider uIVolumeSlider;

    public static InGameMenuManager LocalInstance { get; private set; }
    void Awake()
    {
        LocalInstance = this;
    }
    void Start()
    {
        Slider[] sliders = GetComponentsInChildren<Slider>(true);

        masterVolumeSlider = Array.Find(sliders, c => c.gameObject.name.Equals("MasterVolumeSlider"));
        bGMVolumeSlider = Array.Find(sliders, c => c.gameObject.name.Equals("BGMVolumeSlider"));
        sFXVolumeSlider = Array.Find(sliders, c => c.gameObject.name.Equals("SFXVolumeSlider"));
        uIVolumeSlider = Array.Find(sliders, c => c.gameObject.name.Equals("UIVolumeSlider"));

        masterVolumeSlider.onValueChanged.AddListener(MasterVolumeValueChanged);
        bGMVolumeSlider.onValueChanged.AddListener(BGMVolumeValueChanged);
        sFXVolumeSlider.onValueChanged.AddListener(SFXVolumeValueChanged);
        uIVolumeSlider.onValueChanged.AddListener(UIVolumeValueChanged);

        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        bGMVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        sFXVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        uIVolumeSlider.value = PlayerPrefs.GetFloat("UIVolume");
    }

    void Update()
    {
        PressEscape();
    }

    public void PressEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inGameMenuCanvas.activeInHierarchy)
            {
                inGameMenuCanvas.SetActive(false);
                optionPanel.SetActive(false);
            }
            else
            {
                inGameMenuCanvas.SetActive(true);
            }
        }
    }

    public void PressOption()
    {
        if (optionPanel.activeInHierarchy)
        {
            optionPanel.SetActive(false);
        }
        else
        {
            optionPanel.SetActive(true);
        }
    }
    public void PressQuit()
    {
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkManager.Singleton.Shutdown(); // 즉시 연결 끊기
        }

        Application.Quit();
    }
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
}
