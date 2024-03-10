using API.Mediator.Request;
using Domain.Tableau;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AnwesenheitsMonitor
{
    [Route(Domain.Routes.TABLEAU)]
    [Authorize]
    public class TableuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TableuController(IMediator mediator)
            => _mediator = mediator;


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmployeePresentCollection>> Get()
        {
            try
            {
                var result = await _mediator.Send(new TableauRequest());
                int count = result.Models.Count();
                return !result.Models.Any() ? NoContent() : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
