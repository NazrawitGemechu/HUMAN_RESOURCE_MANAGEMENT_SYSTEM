using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Compliant
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public DateTime DateOfEvent { get; set; }
        public string Incident { get; set; }
        public string Remedy { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.Now;
        public string Status { get; set; }
    }
}
