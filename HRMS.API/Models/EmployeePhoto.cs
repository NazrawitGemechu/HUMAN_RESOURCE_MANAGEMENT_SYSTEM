using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class EmployeePhoto
    {
        [Key]
        public int Id { get; set; }
        public string? pictureURL { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
