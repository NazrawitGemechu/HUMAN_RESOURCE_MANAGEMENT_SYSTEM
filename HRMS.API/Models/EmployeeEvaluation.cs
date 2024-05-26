namespace HRMS.API.Models
{
    public class EmployeeEvaluation
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int EvaluationFactorId { get; set; }
        public EvaluationFactor EvaluationFactor { get; set; }
        public int Rating { get; set; } 
        public DateTime EvaluationDate { get; set; }=DateTime.Now;
    }

}
