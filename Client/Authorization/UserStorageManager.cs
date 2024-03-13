using System.Net.Http.Json;
using Client.Extensions;
using Client.Services;
using Domain.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Authorization
{
    public abstract class UserStorageManager : IUserManager
    {
        

        private readonly ClientAuthenticationStateProvider _stateProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        private UserSession _userSessionCache = null!;
        private string _jwtCache = null!;


        protected UserStorageManager(AuthenticationStateProvider stateProvider, IHttpClientFactory httpClientFactory)
            => (_stateProvider,_httpClientFactory) = ((stateProvider as ClientAuthenticationStateProvider)!, httpClientFactory);

        public async ValueTask<string> GetJwtAsync()
        {
            if(_jwtCache is not null)
                return _jwtCache;
            _jwtCache = await GetJwtFallback();
            return _jwtCache;
        }

        public async ValueTask<UserSession> GetUserSessionAsync()
        {
            if (_userSessionCache is not null)
                return _userSessionCache;
            _userSessionCache = await GetUserSessionFallback();
            return _userSessionCache;
        }

        public async Task<bool> LoginAsync(LoginRequestModel loginRequest)
        {
            var result = await _httpClientFactory.CreateAPIClient()
                .PostAsJsonAsync($"{Domain.Routes.AUTHORIZATION}/{Domain.Routes.LOGIN}", loginRequest);

            if(!result.IsSuccessStatusCode)
                return false;

            UserSession session = (await result.Content.ReadFromJsonAsync<UserSession>()) ?? throw new InvalidDataException();
            await AfterLogin(session);
            _userSessionCache = session;
            _stateProvider.UpdateAuthenticaionState(_userSessionCache);
            return true;
        }

        public async Task LogoutAsync()
        {
            _userSessionCache = null!;
            _jwtCache = null!;
            await AfterLogout();
            _stateProvider.UpdateAuthenticaionState(null);
        }

        protected abstract Task AfterLogin(UserSession userSession);
        protected abstract Task AfterLogout();
        protected abstract Task<UserSession> GetUserSessionFallback();
        protected abstract Task<string> GetJwtFallback();
    }
}
