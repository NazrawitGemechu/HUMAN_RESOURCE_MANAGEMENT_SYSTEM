using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<Resignation> Resignations { get; set; }
    }
}
