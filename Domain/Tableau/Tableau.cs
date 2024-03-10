using System.Collections;

namespace Domain.Tableau
{
    public enum PresentState
    {
        None,
        Present,
        Absent
    }

    public class EmployeePresentModel
    {
        public uint EmployeeNumber { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public PresentState PresentState { get; set; } = PresentState.None;
    }


    public class EmployeePresentCollection 
    {
         public IEnumerable<EmployeePresentModel> Models { get; set; } = new List<EmployeePresentModel>();
    }
}
