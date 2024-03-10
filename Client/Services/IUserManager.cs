using System.Security.Claims;
using Domain.Authorization;

namespace Client.Services
{
    public interface IUserManager
    {
        private static readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public static ClaimsPrincipal Anonymous => _anonymous;
        public Task<bool> LoginAsync(LoginRequestModel loginRequest);

        public Task LogoutAsync();

        public ValueTask<UserSession> GetUserSessionAsync();

        public ValueTask<string> GetJwtAsync();

        public static ClaimsPrincipal BuildClaimsPrincipal(UserSession? userSession)
        {
            if (userSession is null)
                return Anonymous;
            var claimPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Name, userSession.User),
                        new Claim(ClaimTypes.Role, userSession.ClaimRole.ToString()),
                        new Claim(ClaimTypes.UserData, userSession.DisplayName)
                    ]
                    , ClientDefaults.AUTH));
            return claimPrincipal;
        }
    }
}
