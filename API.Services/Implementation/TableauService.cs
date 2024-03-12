using API.Data.Context;
using API.Data.Entities;
using Domain.Tableau;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class TableauService : ITableauService
    {
        private readonly DbSet<Employee> _employess;
        private readonly ITimeProviderService _timeProvider;

        public TableauService(ProjectContext context, ITimeProviderService timeProviderService)
            => (_employess, _timeProvider) = (context.Employees, timeProviderService);

        public async Task<EmployeePresentCollection> GetEmployeePresentsCollectionAsync()
        {
            List<Employee> employees = await _employess.Include(e => e.Stamps).ToListAsync();
            List<EmployeePresentModel> models = [];
            foreach (Employee employee in employees)
            {
                if (!employee.Stamps.Where(s => s.StampTime.Date == _timeProvider.GetUtcNow().Date).Any())
                {
                    models.Add( new EmployeePresentModel 
                    { 
                        EmployeeNumber= employee.Id, 
                        DisplayName= $"{employee.Name}, {employee.FirstName??string.Empty}", 
                        PresentState=PresentState.None 
                    });
                    continue;
                }
                Stamp lastStamp = employee.Stamps.OrderBy(e => e.StampTime).Last();
                models.Add(new EmployeePresentModel 
                { 
                    EmployeeNumber = employee.Id, 
                    DisplayName = $"{employee.Name}, {employee.FirstName ?? string.Empty}", 
                    PresentState = lastStamp.StampType == Domain.StampType.In ? PresentState.Present : PresentState.Absent }
                );
            }
            return new EmployeePresentCollection { Models = models };
        }
    }
}
