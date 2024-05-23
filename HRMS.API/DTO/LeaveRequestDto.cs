namespace HRMS.API.DTO
{
    public class LeaveRequestDto
    {
        public string Emp_Id { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }
}
