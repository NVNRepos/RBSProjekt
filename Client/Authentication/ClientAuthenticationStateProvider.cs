using Blazored.SessionStorage;
using Client.Extensions;
using Client.Services;
using Domain.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Authentication
{

    /// <summary>
    /// <inheritdoc/>
    /// <para>For WASM authentication and authorizaton</para>
    /// <para>Sets a IIdentity (here ClaimIdentity) to check who the user are </para>
    /// </summary>
    public class ClientAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;
        public ClientAuthenticationStateProvider(ISessionStorageService sessionStorageService) => _sessionStorageService = sessionStorageService;
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var currentSession = await _sessionStorageService.ReadEncryptedItemAsync<UserSession>(ClientDefaults.USER_SESSION);
                var principal = IUserManager.BuildClaimsPrincipal(currentSession);
                return new AuthenticationState(principal);
            }
            catch
            {
                return new AuthenticationState(IUserManager.Anonymous);
            }
        }

        //NULL bei Logout
        public void UpdateAuthenticaionState(UserSession? userSession)
            => NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(IUserManager.BuildClaimsPrincipal(userSession))));
        

    }
}
