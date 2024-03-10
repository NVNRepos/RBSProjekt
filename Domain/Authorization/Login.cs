using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Authorization {
    public class LoginRequestModel {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ClaimRole ClaimRole { get; set; }
    }

    public enum ClaimRole {
        Employee,
        User
    }

    public record UserSession(string Jwt, string User, ClaimRole ClaimRole, string DisplayName );

}
