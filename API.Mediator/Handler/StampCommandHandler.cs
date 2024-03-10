using API.Mediator.Command;
using API.Services;
using Domain.Terminal;
using MediatR;

namespace API.Mediator.Handler
{
    public class StampCommandHandler : IRequestHandler<StampCommand, StampResponseModel>
    {

        private readonly ITerminalService _terminalService;

        public StampCommandHandler(ITerminalService terminalService)
            => _terminalService = terminalService;

        public async Task<StampResponseModel> Handle(StampCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var (success, message) = await _terminalService.Stamp(request.EmployeeId, request.StampType);
                return new StampResponseModel { Success = success, ErrorMessage = message };
            }
            catch (Exception ex)
            {
                return new StampResponseModel { Success = false, ErrorMessage = ex.Message };
            }
        }
    }
}
