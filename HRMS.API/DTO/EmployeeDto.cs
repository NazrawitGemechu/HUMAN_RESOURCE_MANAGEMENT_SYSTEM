﻿namespace HRMS.API.DTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public string Email { get; set; }
        public IFormFile EmployeePhoto { get; set; }
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
        public string Degree { get; set; }
        public string Institute { get; set; }

        // Experience details
        public string CompanyName { get; set; }
        public string ExperiencePosition { get; set; }
        public DateTime ExperienceStartDate { get; set; }
        public DateTime ExperienceEndDate { get; set; }

        // Contact person details
        public string ContactPersonName { get; set; }
        public string Relationship { get; set; }
        public string ContactPhoneNo { get; set; }
        public string ContactRegion { get; set; }
        public string ContactWoreda { get; set; }
        public int ContactKebele { get; set; }
        public string? ContactHouseNo { get; set; }

        //Child details
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

}
