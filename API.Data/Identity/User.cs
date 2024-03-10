using System.ComponentModel.DataAnnotations.Schema;
using API.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data.Identity {
    public class User : IdentityUser {

        public uint? EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [NotMapped]
        public bool HasEmploye => Employee is not null;

    }
}
