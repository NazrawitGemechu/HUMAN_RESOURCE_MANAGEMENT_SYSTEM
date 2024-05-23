using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class ContactPerson
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string PhoneNo { get; set; }
        public string Region { get; set; }
        public string Woreda { get; set; }
        public int Kebele { get; set; }
        public string? HouseNo { get; set; }

    }
}
