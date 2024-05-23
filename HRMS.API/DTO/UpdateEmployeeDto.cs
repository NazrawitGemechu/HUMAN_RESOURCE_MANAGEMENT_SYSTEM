namespace HRMS.API.DTO
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Emp_Id { get; set; }
        public string Region { get; set; }
        public string Woreda { get; set; }
        public int Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string PhoneNo { get; set; }
        public int DepartmentId { get; set; }
        public int GradeId { get; set; }
        public int PositionId { get; set; }
        public int BranchId { get; set; }
        public int DegreeId { get; set; }
        public DateTime HireDate { get; set; }
        public float Salary { get; set; }
        public string Roles { get; set; }

        // Education details

        public List<EducationDto> Educations { get; set; } = new List<EducationDto>();

        // Experience details

        public List<ExperienceDto> Experiences { get; set; } = new List<ExperienceDto>();

        // Contact person details

        public List<ContactPersonDto> ContactPersons { get; set; } = new List<ContactPersonDto>();
        //Child detail
        public List<ChildDto> ChildInformations { get; set; } = new List<ChildDto>();

    }
}
