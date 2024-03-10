using API.Mediator.Request;
using Domain.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Authorization
{

    [Route(Domain.Routes.AUTHORIZATION)]
    public class AuthorizationController : ControllerBase
    {
        //private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public AuthorizationController(IMediator mediator)
            => _mediator = mediator;

        [HttpPost(Domain.Routes.LOGIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserSession>> Login([FromBody] LoginRequestModel loginRequestModel)
        {
            UserSession? result = await _mediator.Send(new LoginRequest(loginRequestModel));
            return result is null ? NotFound() : Ok(result);
        }
    }
}
