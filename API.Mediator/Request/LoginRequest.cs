using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Services;
using Domain.Authorization;
using MediatR;

namespace API.Mediator.Request {
    public record LoginRequest( LoginRequestModel LoginRequestModel ): IRequest<UserSession?>;
}
