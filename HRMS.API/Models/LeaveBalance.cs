using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class LeaveBalance
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        public int NumberOfDays { get; set; }

    }
}
