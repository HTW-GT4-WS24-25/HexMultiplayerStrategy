using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Networking.Client
{
    public static class AuthenticationWrapper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuthentication(int maxRetries = 5)
        {
            if (AuthState == AuthState.Authenticated)
                return AuthState;

            if(AuthState == AuthState.Authenticating)
            {
                Debug.LogWarning("Already authenticating!");
                await Authenticating();
                return AuthState;
            }

            await SignInAnonymouslyAsync(maxRetries);

            return AuthState;
        }

        private static async Task<AuthState> Authenticating()
        {
            while(AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
            {
                await Task.Delay(200);
            }

            return AuthState;
        }

        private static async Task SignInAnonymouslyAsync(int maxRetries)
        {
            AuthState = AuthState.Authenticating;
            int retries = 0;

            while (AuthState == AuthState.Authenticating && retries < maxRetries)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                catch (AuthenticationException authenticationException)
                {
                    Debug.LogError(authenticationException);
                    AuthState = AuthState.Error;
                }
                catch (RequestFailedException requestException)
                {
                    Debug.LogError(requestException);
                    AuthState = AuthState.Error;
                }

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }

                retries++;
                await Task.Delay(1000);
            }

            if(AuthState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player did not authenticate successfully after {retries} retries.");
                AuthState = AuthState.Timeout;
            }
        }
    }

    public enum AuthState
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Error,
        Timeout
    }
}