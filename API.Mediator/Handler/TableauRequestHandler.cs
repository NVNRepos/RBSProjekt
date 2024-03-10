using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Mediator.Request;
using API.Services;
using Domain.Tableau;
using MediatR;

namespace API.Mediator.Handler
{
    public class TableauRequesthandler : IRequestHandler<TableauRequest, EmployeePresentCollection>
    {
        private readonly ITableauService _tableauService;

        public TableauRequesthandler(ITableauService tableauService) 
            => _tableauService = tableauService;

        public Task<EmployeePresentCollection> Handle(TableauRequest request, CancellationToken cancellationToken)
            => _tableauService.GetEmployeePresentsCollectionAsync();
    }
}
