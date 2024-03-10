using Domain.Tableau;

namespace Client.Services
{
    public interface ITableauService
    {
        public Task<EmployeePresentCollection> GetEmployeePresentsAsync();
    }
}
