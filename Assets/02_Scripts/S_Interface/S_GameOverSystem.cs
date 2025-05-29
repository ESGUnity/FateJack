using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_GameOverSystem : MonoBehaviour
{
    // ������Ʈ
    GameObject image_BlackBackground;
    GameObject panel_GameOverBase;

    // �̱���
    static S_GameOverSystem instance;
    public static S_GameOverSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);

        image_BlackBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BlackBackground")).gameObject;
        panel_GameOverBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_GameOverBase")).gameObject;

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
        panel_GameOverBase.SetActive(false);
    }
    public void AppearGameOverPanel() // �г� ����
    {
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.GameOver;

        // �г� ��ġ �ʱ�ȭ
        image_BlackBackground.SetActive(true);
        image_BlackBackground.GetComponent<Image>().DOFade(0.85f, 1f)
            .OnComplete(() => panel_GameOverBase.SetActive(true));
    }

    public void ClickBackToTitleBtn()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
