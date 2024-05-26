namespace HRMS.API.DTO
{
    public class CreateEmployeeEvaluationDto
    {
        public int EmployeeId { get; set; }
        public int EvaluationFactorId { get; set; }
        public int Rating { get; set; }
    }

}
