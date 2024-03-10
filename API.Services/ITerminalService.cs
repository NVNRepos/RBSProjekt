using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace API.Services {
    public interface ITerminalService {

        public Task<(bool,string?)> Stamp(uint employeeId, StampType type);

    }
}
