namespace HRMS.API.DTO
{
    public class ComplaintDto
    {
        public string Name { get; set; }
        public string Emp_Id { get; set; }
        public int PositionId { get; set; }
        public int BranchId { get; set; }
        public DateTime DateOfEvent { get; set; }
        public string Incident { get; set; }
        public string Remedy { get; set; }
    }
}
