using Client.Services;
using Domain.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Authorization
{
    public abstract class ClientAuthenticationStateProvider : AuthenticationStateProvider
    {

        public abstract override Task<AuthenticationState> GetAuthenticationStateAsync();

        protected ClientAuthenticationStateProvider() { }
        //NULL bei Logout
        public void UpdateAuthenticaionState(UserSession? userSession)
            => NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(IUserManager.BuildClaimsPrincipal(userSession))));
    }
}
