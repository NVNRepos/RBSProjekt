using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Data.Context;
using API.Data.Entities;
using API.Data.Identity;
using Domain.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class AuthorizationService : IAuthorizationService
    {

        private readonly IConfiguration _connfiguration;
        private readonly DbSet<Employee> _employees;
        private readonly UserManager<User> _userManager;

        private SecurityKey JwtSecurityKey
            => new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_connfiguration["JWT:Secret"] ??
                throw new InvalidOperationException("Missing JWT:Secret in appsettings.json")));

        private TokenValidationParameters TokenValidationParameters
            => new TokenValidationParameters
            {
                ValidIssuer = _connfiguration["JWT:Issuer"],
                ValidAudience = _connfiguration["JWT:Audience"],
                IssuerSigningKey = JwtSecurityKey,
                ValidateLifetime = false
            };

        public AuthorizationService(IConfiguration connfiguration, ProjectContext projectContext, UserManager<User> userManager)
            => (_connfiguration, _employees, _userManager) =
            (connfiguration, projectContext.Employees, userManager);


        private string WriteToken(JwtSecurityToken token)
            => new JwtSecurityTokenHandler().WriteToken(token);


        public ClaimsPrincipal GetClaimPrincipal(string token)
            => new JwtSecurityTokenHandler().ValidateToken(token, TokenValidationParameters, out _);

        public async Task<TokenValidationResult> ValidateAsync(string token)
            => await new JwtSecurityTokenHandler().ValidateTokenAsync(token, TokenValidationParameters);

        // Uid == UserName or Uid == EmployeeNumber
        public async Task<string> GenerateJwt(string uid, ClaimRole claimRole)
        {
            string displayName = uid;
            if (claimRole == ClaimRole.Employee)
            {
                if (uint.TryParse(uid, out uint emplNr))
                {
                    var employee = await _employees.FindAsync(emplNr);
                    displayName = $"Employee Nr {emplNr}, {employee!.Name}";
                    if (employee!.FirstName is not null)
                        displayName += $" {employee!.FirstName}";
                }
            }

            List<Claim> authClaims = [
                new Claim( ClaimTypes.Name, uid ),
                new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim( ClaimTypes.Role, claimRole.ToString() ),
                new Claim( ClaimTypes.UserData,displayName ) //TODO: better solution
                ];

            var token = new JwtSecurityToken(
                issuer: _connfiguration["JWT:Issuer"],
                audience: _connfiguration["JWT:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddDays(10).Date,
                signingCredentials: new SigningCredentials(JwtSecurityKey, SecurityAlgorithms.HmacSha256)
                );
            return WriteToken(token);
        }

        public async Task<bool> CheckCredentials(LoginRequestModel loginRequest)
            => loginRequest.ClaimRole switch
            {
                ClaimRole.User => await CheckUser(loginRequest),
                ClaimRole.Employee => await CheckEmployee(loginRequest),
                _ => throw new System.Diagnostics.UnreachableException()
            };

        private async Task<bool> CheckUser(LoginRequestModel loginRequest)
        {
            User? user = await _userManager.FindByNameAsync(loginRequest.UserName);
            return user is not null && (await _userManager.CheckPasswordAsync(user, loginRequest.Password));
        }

        private async Task<bool> CheckEmployee(LoginRequestModel loginRequest)
        {
            if (uint.TryParse(loginRequest.UserName, out uint userId))
            {
                Employee? employee = await _employees.FindAsync(userId);
                return employee is not null && employee.Password == loginRequest.Password;
            }
            return false;
        }
    }


}
