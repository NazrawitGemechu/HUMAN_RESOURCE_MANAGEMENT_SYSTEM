using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HRMS.API.Models
{
    public class InternalJob
    {
        [Key]
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public DateTime PostingDate { get; set; } = DateTime.Now;
        public ICollection<Employee> ShortlistedEmployees { get; set; }
        [JsonIgnore]
        public ICollection<EmployeeJobApplication> EmployeeApplications { get; set; }
    }
}
