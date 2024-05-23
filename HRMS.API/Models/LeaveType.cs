using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int AllowedDays { get; set; }

        public ICollection<Leave> Leaves { get; set; }
    }
}
