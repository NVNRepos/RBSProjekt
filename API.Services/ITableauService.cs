using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Tableau;

namespace API.Services
{
    public interface ITableauService
    {
        public Task<EmployeePresentCollection> GetEmployeePresentsCollectionAsync();
    }
}
