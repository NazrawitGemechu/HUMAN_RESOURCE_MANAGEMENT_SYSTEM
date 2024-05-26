namespace HRMS.API.DTO
{
    public class EmployeeOwnEvaluationDetailDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<EvaluationDetailDto> Evaluations { get; set; }
        public List<FeedbackDetailDto> Feedbacks { get; set; }
    }
}
