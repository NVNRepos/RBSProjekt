using Domain;

namespace API.Data.Entities {
    public class Stamp {

        public Guid Id { get; set; }
        public DateTime StampTime { get; set; }

        public StampType StampType { get; set; }

        public required Employee Employee { get; set; }
    }

    
}
