using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Resignation
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
        public DateTime EmployeeHireDate { get; set; }
        public DateTime SeparationDate { get; set; }
        public string Reason { get; set; }
        public string Satisfaction { get; set; }
        public string EmployeeRelationship { get; set; }
        public string Recommendation { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }

    }
}
