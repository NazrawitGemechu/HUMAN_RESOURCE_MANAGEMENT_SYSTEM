using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Leave
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public int LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }


    }
}
