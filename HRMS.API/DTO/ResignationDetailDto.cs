namespace HRMS.API.DTO
{
    public class ResignationDetailDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public DateTime EmployeeHireDate { get; set; }
        public DateTime SeparationDate { get; set; }
        public string Reason { get; set; }
        public string Satisfaction { get; set; }
        public string EmployeeRelationship { get; set; }
        public string Recommendation { get; set; }
        public string Comment { get; set; }

    }
}
