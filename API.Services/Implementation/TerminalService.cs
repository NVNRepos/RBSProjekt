using API.Data.Context;
using API.Data.Entities;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace API.Services {
    public class TerminalService: ITerminalService {

        private readonly ProjectContext _context;
        private readonly ITimeProviderService _timeProvider;
        public TerminalService( ProjectContext context, ITimeProviderService timeProvider )
            => (_context, _timeProvider) = (context, timeProvider);

        public async Task<(bool,string?)> Stamp( uint employeeId, StampType type ) {
            Employee? employee = await _context.Employees.Include( e => e.Stamps ).FirstOrDefaultAsync( e => e.Id == employeeId );
            if( employee == null )
                return (false, "Mitarbeiter nicht gefunden");

            IEnumerable<Stamp> employeeStamps = employee.Stamps.Where( s => s.StampTime.Date == _timeProvider.GetUtcNow().Date ).OrderBy(s => s.StampTime).ToList(); // today

            if( !employeeStamps.Any() && type == StampType.Out )
                return (false, "Mitarbeiter hat noch nicht gebucht");

            if( employeeStamps.LastOrDefault()?.StampType == type)
                return (false, $"Mitarbeiter kann {type} stempeln");

            Stamp stamp = new () {
                StampType = type,
                StampTime = _timeProvider.GetUtcNow(),
                Employee = employee
            };

            await _context.AddAsync( stamp );

            await _context.SaveChangesAsync();

            return (true, null);
            
        }
    }
}
