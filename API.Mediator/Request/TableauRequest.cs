using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Tableau;
using MediatR;

namespace API.Mediator.Request
{
    public record TableauRequest : IRequest<EmployeePresentCollection>;
}
