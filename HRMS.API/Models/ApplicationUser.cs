using Microsoft.AspNetCore.Identity;

namespace HRMS.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? pictureURL { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

    }

}
