using System.ComponentModel.DataAnnotations.Schema;
using API.Data.Identity;

namespace API.Data.Entities {

    public class Employee {

        public uint Id { get; set; }

        public required string Name { get; set; }

        public string? FirstName { get; set; }

        public required string Password { get; set; }

        public User? User { get; set; }

        [NotMapped]
        public bool HasUser => User is not null;

        public Department? Department { get; set; }

        public ICollection<Stamp> Stamps { get; set; } = new List<Stamp>();
    }
}
