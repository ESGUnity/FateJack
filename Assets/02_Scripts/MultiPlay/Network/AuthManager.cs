using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public enum AuthStateEnum
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Timeout,
    Error
}

public static class AuthManager
{
    public static AuthStateEnum AuthState { get; private set; } = AuthStateEnum.NotAuthenticated;

    public static async Task<AuthStateEnum> TryAuth(int maxTries = 5)
    {
        if (AuthState == AuthStateEnum.Authenticating) return AuthState;

        await AuthAnonymouslyAsync(maxTries);

        return AuthState;
    }

    static async Task AuthAnonymouslyAsync(int maxTries = 5)
    {
        AuthState = AuthStateEnum.Authenticating;

        int tries = 0;
        while (AuthState == AuthStateEnum.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized) // IsSignedIn는 로그인 여부, IsAuthorized는 토큰 유효 여부 검사
                {
                    AuthState = AuthStateEnum.Authenticated;
                    return;
                }

            }
            catch (AuthenticationException e) // 5회 이상 시도하지 않고 시도해서 안 될 오류라 판단하여 즉시 리턴한다.
            {
                Debug.LogException(e);
                AuthState = AuthStateEnum.Error;
                return;
            }
            catch (RequestFailedException e)
            {
                Debug.LogException(e);
                AuthState = AuthStateEnum.Error;
                return;
            }

            tries++;
            await Task.Delay(1000);
        }
        
        AuthState = AuthStateEnum.Timeout; // 반복문 탈출 시 타임아웃
    }
}
