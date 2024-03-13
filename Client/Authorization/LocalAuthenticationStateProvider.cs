using Blazored.LocalStorage;
using Client.Services;
using Client.Extensions;
using Domain.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Authorization
{
    public class LocalAuthenticationStateProvider : ClientAuthenticationStateProvider
    {

        private readonly ILocalStorageService _localStorageService;

        public LocalAuthenticationStateProvider(ILocalStorageService localStorageService) : base() 
            => _localStorageService = localStorageService;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var currentSession = await _localStorageService.ReadEncryptedItemAsync<UserSession>(ClientDefaults.USER_SESSION);
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
