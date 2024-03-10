using System.Text.Json.Serialization;

namespace API.Data.Entities {
    public class Department {

        public uint Id { get; set; }

        public string? Name { get; set; }

        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
