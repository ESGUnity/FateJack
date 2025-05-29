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

        // 유니티 서비스 초기화
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
        // 초기화 될 때까지 대기
        while (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await Task.Delay(1000);
        }

        // UI 부분
        DisplayLoginInProcess();

        // 로그인 시도
        AuthStateEnum authState = await AuthManager.TryAuth(); // 로그인 인증 시도
        if (authState == AuthStateEnum.Authenticated)
        {
            DisplayLoginResult("로그인 성공!");

            HostSingleton.Instance.Init();
            ClientSingleton.Instance.Init();

            if (string.IsNullOrEmpty(nickNameInputField.text))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("닉네임입력안한사람");
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
            DisplayLoginResult("알 수 없는 오류!");

            await Task.Delay(1000);

            isAuthenticated = false;
            InitLoginCanvas();
        }
        else if (authState == AuthStateEnum.Timeout)
        {
            DisplayLoginResult("타임 아웃되었습니다. 다시 시도해주세요.");

            await Task.Delay(1000);

            isAuthenticated = false;
            InitLoginCanvas();
        }
        else
        {
            DisplayLoginResult("로그인을 실패했습니다. 다시 시도해주세요.");

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
        loginInProcessText.text = "로그인 중...";
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
