using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_DemoSystem : MonoBehaviour
{
    // 컴포넌트
    GameObject image_BlackBackground;
    GameObject panel_DemoBase;

    // 싱글턴
    static S_DemoSystem instance;
    public static S_DemoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        image_BlackBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BlackBackground")).gameObject;
        panel_DemoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_DemoBase")).gameObject;

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
    void InitPos()
    {
        image_BlackBackground.GetComponent<Image>().DOFade(0, 0);
        image_BlackBackground.SetActive(false);
        panel_DemoBase.SetActive(false);
    }
    public void AppearDemoPanel() // 패널 등장
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.GameOver;

        // 패널 위치 초기화
        image_BlackBackground.SetActive(true);
        panel_DemoBase.SetActive(true);
        image_BlackBackground.GetComponent<Image>().DOFade(0.95f, 0.5f);
    }

    public void ClickBackToTitleBtn()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
