using Blazored.LocalStorage;
using Client.Extensions;
using Domain.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using static Client.ClientDefaults;

namespace Client.Authentication
{
    public class UserLocalStorageManager : UserStorageManager
    {
        private readonly ILocalStorageService _localStorageService;

        public UserLocalStorageManager(AuthenticationStateProvider stateProvider, IHttpClientFactory httpClientFactory,
            ILocalStorageService localStorageService)
            : base(stateProvider, httpClientFactory)
            => _localStorageService = localStorageService;

        protected override Task AfterLogin(UserSession userSession)
            => Task.WhenAll(_localStorageService.SaveItemEncryptedAsync(USER_SESSION, userSession),
                _localStorageService.SaveItemEncryptedAsync(USER_JWT, userSession.Jwt));

        protected override Task AfterLogout()
            => Task.WhenAll(_localStorageService.RemoveItemAsync(USER_SESSION).AsTask(),
                    _localStorageService.RemoveItemAsync(USER_JWT).AsTask());

        protected override async Task<string> GetJwtFallback()
        {
            try
            {
                var jwt = await _localStorageService.ReadEncryptedItemAsync<string>(USER_JWT);
                return jwt ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        protected override async Task<UserSession> GetUserSessionFallback()
        {
            try
            {
                var userSession = await _localStorageService.ReadEncryptedItemAsync<UserSession>(USER_SESSION);
                return userSession ?? null!;
            }
            catch
            {
                return null!;
            }
        }
    }
}
