using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_DemoSystem : MonoBehaviour
{
    // ������Ʈ
    GameObject image_BlackBackground;
    GameObject panel_DemoBase;

    // �̱���
    static S_DemoSystem instance;
    public static S_DemoSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        image_BlackBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BlackBackground")).gameObject;
        panel_DemoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_DemoBase")).gameObject;

        // �̱���
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
    public void AppearDemoPanel() // �г� ����
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.GameOver;

        // �г� ��ġ �ʱ�ȭ
        image_BlackBackground.SetActive(true);
        panel_DemoBase.SetActive(true);
        image_BlackBackground.GetComponent<Image>().DOFade(0.95f, 0.5f);
    }

    public void ClickBackToTitleBtn()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
