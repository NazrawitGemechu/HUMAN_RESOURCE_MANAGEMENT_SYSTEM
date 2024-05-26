namespace HRMS.API.DTO
{
    public class EvaluationListDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TotalRating { get; set; }
        public DateTime EvaluationDate { get; set; }
    }
}
