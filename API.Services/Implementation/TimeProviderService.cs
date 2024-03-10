using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services {
    public class TimeProviderService: ITimeProviderService {
        public DateTime GetLocalTimeNow()
            => DateTime.Now;

        public DateTime GetUtcNow() 
            => DateTime.UtcNow;
    }
}
