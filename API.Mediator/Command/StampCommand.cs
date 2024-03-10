using Domain;
using Domain.Terminal;
using MediatR;

namespace API.Mediator.Command {
    public record StampCommand(uint EmployeeId, StampType StampType): IRequest<StampResponseModel>;
}
