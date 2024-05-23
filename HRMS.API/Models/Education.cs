using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Education
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public string Degree { get; set; }
        public string Institute { get; set; }

    }
}
