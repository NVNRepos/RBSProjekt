using API.Mediator.Command;
using Domain;
using Domain.Terminal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Terminal
{

    [Route(Routes.TERMINAL)]
    public class TerminalController : ControllerBase
    {

        private readonly ILogger<TerminalController> _logger;
        private readonly IMediator _mediator;
        public TerminalController(ILogger<TerminalController> logger,
            IMediator mediator)
            => (_logger, _mediator) = (logger, mediator);

        [HttpPost(Routes.STAMP)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status208AlreadyReported)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Employee")]
        public Task<ActionResult<StampResponseModel>> StampNow([FromBody] StampType stampType)
        {
            //Controller only for Employee => EmployeeName Claim == EmployeeId
            string? name = User.Identity?.Name;
            if (uint.TryParse(name, out uint empId))
            {
                return StampNow(stampType, empId);
            }
            return Task.FromResult<ActionResult<StampResponseModel>>(NotFound(new StampResponseModel { Success = false, ErrorMessage = "Employee not found" }));
        }

        [HttpPost(Routes.STAMP + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status208AlreadyReported)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<StampResponseModel>> StampNow([FromBody] StampType stampType, uint id)
        {
            var result = await _mediator.Send(new StampCommand(id, stampType));
            if (result.Success)
                return Ok(result);
            return StatusCode(StatusCodes.Status208AlreadyReported, result);
        }

    }
}
