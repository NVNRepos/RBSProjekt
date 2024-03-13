using Blazored.SessionStorage;
using Client.Extensions;
using Domain.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using static Client.ClientDefaults;

namespace Client.Authorization
{
    public class UserSessionStorageManager : UserStorageManager
    {
        private readonly ISessionStorageService _sessionStorageService;

        public UserSessionStorageManager(AuthenticationStateProvider stateProvider, IHttpClientFactory httpClientFactory,
            ISessionStorageService sessionStorageService)
            : base(stateProvider, httpClientFactory)
            => _sessionStorageService = sessionStorageService;

        protected override Task AfterLogin(UserSession userSession)
            => Task.WhenAll(_sessionStorageService.SaveItemEncryptedAsync(USER_SESSION, userSession),
                _sessionStorageService.SaveItemEncryptedAsync(USER_JWT, userSession.Jwt));

        protected override Task AfterLogout()
            => Task.WhenAll(_sessionStorageService.RemoveItemAsync(USER_SESSION).AsTask(),
                _sessionStorageService.RemoveItemAsync(USER_JWT).AsTask());

        protected override async Task<string> GetJwtFallback()
        {
            try
            {
                string? jwt = await _sessionStorageService.ReadEncryptedItemAsync<string>(USER_JWT);
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
                UserSession? session = await _sessionStorageService.ReadEncryptedItemAsync<UserSession>(USER_SESSION);
                return session ?? null!;
            }
            catch
            {
                return null!;
            }
        }
    }
}
