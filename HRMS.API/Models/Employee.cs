using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    [Index(nameof(Emp_Id), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]

    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Emp_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Region { get; set; }
        public string Woreda { get; set; }
        public int Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string PhoneNo { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public int GradeId { get; set; }
        public virtual Grade Grade { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public int DegreeId { get; set; }
        public Degree Degree { get; set; }
        public DateTime HireDate { get; set; }
        public float Salary { get; set; }
        public string Roles { get; set; }
        public string? Status { get; set; } = "Active";
        public ICollection<Leave> Leaves { get; set; }
        public virtual List<ChildInformation> ChildInformations { get; set; } = new List<ChildInformation>();
        public virtual List<Experience> Experiences { get; set; } = new List<Experience>();
        public virtual List<Education> Educations { get; set; } = new List<Education>();
        public virtual List<ContactPerson> ContactPersons { get; set; } = new List<ContactPerson>();
        public ICollection<EmployeeJobApplication> AppliedJobs { get; set; }
        public ICollection<Promotion> Promotions { get; set; }
        public int? PreviousGradeId { get; set; }
        public virtual Grade PreviousGrade { get; set; }
        public virtual EmployeePhoto EmployeePhoto { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<EmployeeEvaluation> EmployeeEvaluations { get; set; }
        public ICollection<EmployeeFeedback> EmployeeFeedbacks { get; set; }
    }
}
