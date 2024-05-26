namespace HRMS.API.DTO
{
    public class EmployeeEvaluationDetailDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<EvaluationDetailDto> Evaluations { get; set; }
    }
}
