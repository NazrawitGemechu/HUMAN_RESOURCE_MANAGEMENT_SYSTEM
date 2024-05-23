using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        public string GradeName { get; set; }
        public ICollection<Employee> Employees { get; set; }

    }
}
