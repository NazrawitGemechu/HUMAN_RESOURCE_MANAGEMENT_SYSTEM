using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class EmployeeJobApplication
    {
        [Key]
        public int Id { get; set; }
        public bool Shortlisted { get; set; }
        public string Status { get; set; } = "Pending";
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int InternalJobId { get; set; }
        public InternalJob InternalJob { get; set; }
    }
}
