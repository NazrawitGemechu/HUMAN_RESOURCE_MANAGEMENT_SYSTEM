using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
