using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Terminal {
    public class StampResponseModel
    {
        public bool Success { get; set; } = false;

        public string? ErrorMessage { get; set; } = null;
    }

}
