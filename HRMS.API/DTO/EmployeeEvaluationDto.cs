namespace HRMS.API.DTO
{
    public class EmployeeEvaluationDto
    {
        public int EmployeeId { get; set; }
        public List<EvaluationDto> Evaluations { get; set; }
    }

}
