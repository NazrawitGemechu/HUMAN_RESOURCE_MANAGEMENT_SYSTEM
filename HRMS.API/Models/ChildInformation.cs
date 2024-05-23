using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class ChildInformation
    {
        [Key]
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
