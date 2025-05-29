using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using WebSocketSharp;

public class ApplicationManager : MonoBehaviour
{
    GameObject authCanvas;
    GameObject loginBtnsBase;
    GameObject loginStateBase;
    GameObject loginInProcessImage;
    TMP_Text loginInProcessText;
    TMP_Text loginStateText;
    TMP_InputField nickNameInputField;

    bool isAuthenticated;

    async void Start()
    {
        authCanvas = gameObject;

        Transform[] transformComponents = GetComponentsInChildren<RectTransform>(true);
        loginBtnsBase = Array.Find(transformComponents, c => c.gameObject.name.Equals("LoginBtnsBase")).gameObject;
        loginStateBase = Array.Find(transformComponents, c => c.gameObject.name.Equals("LoginStateBase")).gameObject;
        loginInProcessImage = Array.Find(transformComponents, c => c.gameObject.name.Equals("LoginInProcessImage")).gameObject;

        TMP_Text[] tMP_TextComponents = GetComponentsInChildren<TMP_Text>(true);
        loginInProcessText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("LoginInProcessText"));
        loginStateText = Array.Find(tMP_TextComponents, c => c.gameObject.name.Equals("LoginStateText"));

        TMP_InputField[] tMP_InputFieldComponents = GetComponentsInChildren<TMP_InputField>(true);
        nickNameInputField = Array.Find(tMP_InputFieldComponents, c => c.gameObject.name.Equals("NickNameInputField"));

        // ����Ƽ ���� �ʱ�ȭ
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
        {
            nickNameInputField.text = AuthenticationService.Instance.PlayerName.IndexOf('#') >= 0 ? AuthenticationService.Instance.PlayerName.Substring(0, AuthenticationService.Instance.PlayerName.IndexOf('#')) : AuthenticationService.Instance.PlayerName;
        }
    }

    void Update()
    {
        if (isAuthenticated)
        {
            InitLoginCanvas();
            authCanvas.SetActive(false);
        }
        else
        {
            if (!authCanvas.activeInHierarchy)
            {
                authCanvas.SetActive(true);
            }
        }
    }

    async Task ApplicationStart()
    {
        // �ʱ�ȭ �� ������ ���
        while (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await Task.Delay(1000);
        }

        // UI �κ�
        DisplayLoginInProcess();

        // �α��� �õ�
        AuthStateEnum authState = await AuthManager.TryAuth(); // �α��� ���� �õ�
        if (authState == AuthStateEnum.Authenticated)
        {
            DisplayLoginResult("�α��� ����!");

            HostSingleton.Instance.Init();
            ClientSingleton.Instance.Init();

            if (string.IsNullOrEmpty(nickNameInputField.text))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("�г����Է¾��ѻ��");
            }
            else
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(nickNameInputField.text);
            }

            await Task.Delay(1000);

            isAuthenticated = true;
        }
        else if (authState == AuthStateEnum.Error)
        {
            DisplayLoginResult("�� �� ���� ����!");

            await Task.Delay(1000);

            isAuthenticated = false;
            InitLoginCanvas();
        }
        else if (authState == AuthStateEnum.Timeout)
        {
            DisplayLoginResult("Ÿ�� �ƿ��Ǿ����ϴ�. �ٽ� �õ����ּ���.");

            await Task.Delay(1000);

            isAuthenticated = false;
            InitLoginCanvas();
        }
        else
        {
            DisplayLoginResult("�α����� �����߽��ϴ�. �ٽ� �õ����ּ���.");

            await Task.Delay(1000);

            isAuthenticated = false;
            InitLoginCanvas();
        }
    }
    public async void PressGuestLogin()
    {
        await ApplicationStart();
    }
    void DisplayLoginInProcess()
    {
        loginBtnsBase.SetActive(false);
        loginStateBase.SetActive(true);
        loginInProcessText.text = "�α��� ��...";
        loginInProcessText.gameObject.SetActive(true);
        loginInProcessImage.SetActive(true);
        loginStateText.text = "";
        loginStateText.gameObject.SetActive(false);
    }
    void DisplayLoginResult(string result)
    {
        loginInProcessText.gameObject.SetActive(false);
        loginInProcessImage.SetActive(false);
        loginStateText.text = result;
        loginStateText.gameObject.SetActive(true);
    }
    void InitLoginCanvas()
    {
        loginStateBase.SetActive(false);
        loginBtnsBase.SetActive(true);
    }
}
