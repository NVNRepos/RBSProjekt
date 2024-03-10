using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Mediator.Request;
using API.Services;
using Domain.Authorization;
using MediatR;

namespace API.Mediator.Handler {
    public class LoginRequestHandler: IRequestHandler<LoginRequest, UserSession?> {

        private readonly IAuthorizationService _authorizationService;

        public LoginRequestHandler(IAuthorizationService authorizationService) 
            => _authorizationService = authorizationService;
        public async Task<UserSession?> Handle( LoginRequest request, CancellationToken cancellationToken ) {
            if(! await _authorizationService.CheckCredentials(request.LoginRequestModel)) 
                return null;
            string jwt = await _authorizationService.GenerateJwt(request.LoginRequestModel.UserName, request.LoginRequestModel.ClaimRole);
            string displayName = _authorizationService.GetClaimPrincipal(jwt).Claims.First( c => c.Type == ClaimTypes.UserData ).Value;

            return new UserSession(jwt, request.LoginRequestModel.UserName, request.LoginRequestModel.ClaimRole, displayName);
        }
    }
}
