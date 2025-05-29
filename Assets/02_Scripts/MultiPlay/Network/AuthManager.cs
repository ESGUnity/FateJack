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

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized) // IsSignedIn�� �α��� ����, IsAuthorized�� ��ū ��ȿ ���� �˻�
                {
                    AuthState = AuthStateEnum.Authenticated;
                    return;
                }

            }
            catch (AuthenticationException e) // 5ȸ �̻� �õ����� �ʰ� �õ��ؼ� �� �� ������ �Ǵ��Ͽ� ��� �����Ѵ�.
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
        
        AuthState = AuthStateEnum.Timeout; // �ݺ��� Ż�� �� Ÿ�Ӿƿ�
    }
}
