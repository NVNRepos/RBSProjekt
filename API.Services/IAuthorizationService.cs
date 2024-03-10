using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Authorization;

namespace API.Services {
    public interface IAuthorizationService {

        public Task<bool> CheckCredentials(LoginRequestModel loginRequest );

        public Task<string> GenerateJwt(string claimName, ClaimRole claimRole);

        public ClaimsPrincipal GetClaimPrincipal(string jwt);
    }

}
