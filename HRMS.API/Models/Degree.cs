using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Degree
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
