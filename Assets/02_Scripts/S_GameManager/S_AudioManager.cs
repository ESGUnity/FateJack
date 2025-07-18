using UnityEngine;

public enum BGMEnum
{
    Single = 0,
}
public enum SFXEnum
{
    Dialog = 0, HitCard = 1, CardHovering = 2, CardActivated = 3, Harm = 4,
}
public enum UIEnum
{
    UI_Hovering = 0, UI_Click = 1, 
}

public class S_AudioManager : MonoBehaviour
{
    float masterVolume = 1f;

    [Header("BGM")]
    public AudioClip[] BGMClips;
    float bGMVolume = 0.2f;
    AudioSource bGMPlayer;

    [Header("SFX")]
    public AudioClip[] SFXClips;
    float sFXVolume = 0.2f;
    AudioSource[] sFXPlayers;
    int sFXChannels = 30;
    int sFXchannelIndex;

    [Header("UI")]
    public AudioClip[] UIClips;
    float uIVolume = 0.2f;
    AudioSource[] uIPlayers;
    int uIChannels = 10;
    int uIchannelIndex;

    // 간이 싱글턴
    static S_AudioManager instance;
    public static S_AudioManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        // 마스터 볼륨 가져오기
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.Save();
        }
        // 브금 볼륨 가져오기
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            bGMVolume = PlayerPrefs.GetFloat("BGMVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("BGMVolume", bGMVolume);
            PlayerPrefs.Save();
        }
        // SFX 볼륨 가져오기
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("SFXVolume", sFXVolume);
            PlayerPrefs.Save();
        }
        // UI 볼륨 가져오기
        if (PlayerPrefs.HasKey("UIVolume"))
        {
            uIVolume = PlayerPrefs.GetFloat("UIVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("UIVolume", uIVolume);
            PlayerPrefs.Save();
        }

        // 브금 초기화
        GameObject bGMGo = new GameObject("BGMPlayer");
        bGMGo.transform.parent = transform;
        bGMPlayer = bGMGo.AddComponent<AudioSource>();
        bGMPlayer.playOnAwake = false;
        bGMPlayer.loop = true;
        bGMPlayer.volume = bGMVolume * masterVolume;

        // SFX 초기화
        GameObject sFXGo = new GameObject("SFXPlayer");
        sFXGo.transform.parent = transform;
        sFXPlayers = new AudioSource[sFXChannels];
        for (int i = 0; i < sFXChannels; i++)
        {
            sFXPlayers[i] = sFXGo.AddComponent<AudioSource>();
            sFXPlayers[i].playOnAwake = false;
            sFXPlayers[i].loop = false;
            sFXPlayers[i].volume = sFXVolume * masterVolume;
        }

        // SFX 초기화
        GameObject uIGo = new GameObject("UIPlayer");
        uIGo.transform.parent = transform;
        uIPlayers = new AudioSource[uIChannels];
        for (int i = 0; i < uIChannels; i++)
        {
            uIPlayers[i] = uIGo.AddComponent<AudioSource>();
            uIPlayers[i].playOnAwake = false;
            uIPlayers[i].loop = false;
            uIPlayers[i].volume = uIVolume * masterVolume;
        }
    }

    public void PlayBGM(BGMEnum bGM) // 브금 재생 메서드
    {
        if (bGMPlayer.isPlaying)
        {
            //bGMPlayer.Stop();
            return;
        }

        //int randomIndex = 0;
        //if (bGM == BGMEnum.InGame)
        //{
        //    randomIndex = UnityEngine.Random.Range(0, 3);
        //}

        bGMPlayer.clip = BGMClips[(int)bGM]; // + randomIndex
        bGMPlayer.Play();
    }
    public void StopBGM()
    {
        bGMPlayer.Stop();
    }
    public void PlaySFX(SFXEnum sFX) // SFX 재생 메서드
    {
        for (int i = 0; i < sFXChannels; i++)
        {
            int loopIndex = (i + sFXchannelIndex) % sFXChannels;

            if (sFXPlayers[loopIndex].isPlaying) // 플레이 중이라면 다른 채널로 넘어가기
            {
                continue;
            }
            else
            {
                // 소리 재생
                sFXchannelIndex = loopIndex;
                sFXPlayers[loopIndex].clip = SFXClips[(int)sFX];
                sFXPlayers[loopIndex].Play();
                break;
            }
        }
    }
    public void PlayUI(UIEnum uI) // UI 사운드 재생 메서드
    {
        for (int i = 0; i < uIChannels; i++)
        {
            int loopIndex = (i + uIchannelIndex) % uIChannels;

            if (uIPlayers[loopIndex].isPlaying) // 플레이 중이라면 다른 채널로 넘어가기
            {
                continue;
            }
            else
            {
                uIchannelIndex = loopIndex;
                uIPlayers[loopIndex].clip = UIClips[(int)uI];
                uIPlayers[loopIndex].Play();
                break;
            }
        }
    }

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);

        masterVolume = volume;
        SetBGMVolume(bGMVolume);
        SetSFXVolume(sFXVolume);
        SetUIVolume(uIVolume);
    }
    public void SetBGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("BGMVolume", volume);

        bGMVolume = volume;
        bGMPlayer.volume = bGMVolume * masterVolume;
    }
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);

        sFXVolume = volume;
        for (int i = 0; i < sFXChannels; i++)
        {
            sFXPlayers[i].volume = sFXVolume * masterVolume;
        }
    }
    public void SetUIVolume(float volume)
    {
        PlayerPrefs.SetFloat("UIVolume", volume);

        uIVolume = volume;
        for (int i = 0; i < uIChannels; i++)
        {
            uIPlayers[i].volume = uIVolume * masterVolume;
        }
    }
}
