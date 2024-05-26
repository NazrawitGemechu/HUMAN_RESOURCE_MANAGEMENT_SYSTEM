namespace HRMS.API.Models
{
    public class EmployeeFeedback
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string Feedback { get; set; }
        public DateTime FeedbackDate { get; set; }
    }

}
