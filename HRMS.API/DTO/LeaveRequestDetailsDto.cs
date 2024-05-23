namespace HRMS.API.DTO
{
    public class LeaveRequestDetailsDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveTypeName { get; set; }
        public int RequestedDays { get; set; }
        public int RemainingLeaveDays { get; set; }
    }
}
