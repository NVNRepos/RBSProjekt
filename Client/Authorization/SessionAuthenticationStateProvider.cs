using Blazored.SessionStorage;
using Client.Extensions;
using Client.Services;
using Domain.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Authorization
{
    public class SessionAuthenticationStateProvider : ClientAuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;
        public SessionAuthenticationStateProvider(ISessionStorageService sessionStorageService) : base()
             => _sessionStorageService = sessionStorageService;

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

    }
}
