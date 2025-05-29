using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_GameMenuSystem : MonoBehaviour
{
    // ������Ʈ
    GameObject image_BlackBackground;
    GameObject panel_GameMenuBase;

    // �̱���
    static S_GameMenuSystem instance;
    public static S_GameMenuSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        image_BlackBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BlackBackground")).gameObject;
        panel_GameMenuBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_GameMenuBase")).gameObject;

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

    void Update()
    {
        if (S_GameFlowManager.Instance.GameFlowState != S_GameFlowStateEnum.GameOver && Input.GetKeyDown(KeyCode.Escape))
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
    void InitPos()
    {
        image_BlackBackground.GetComponent<Image>().DOFade(0, 0);
        image_BlackBackground.SetActive(false);
        panel_GameMenuBase.SetActive(false);
    }
    public void AppearGameMenuPanel() // �г� ����
    {
        // �г� ��ġ �ʱ�ȭ
        panel_GameMenuBase.SetActive(true);
        image_BlackBackground.SetActive(true);
        image_BlackBackground.GetComponent<Image>().DOFade(0.85f, 0f);
    }
    public void DisappearGameMenuPanel()
    {
        // �г� ��ġ �ʱ�ȭ
        panel_GameMenuBase.SetActive(false);
        image_BlackBackground.SetActive(false);
        image_BlackBackground.GetComponent<Image>().DOFade(0f, 0f);
    }
    public void ClickForgiveBtn()
    {
        S_GameOverSystem.Instance.AppearGameOverPanel();
    }
    public void ClickContinueBtn()
    {
        DisappearGameMenuPanel();
    }
}
