using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int PreviousPositionId { get; set; }
        public virtual Position PreviousPosition { get; set; }
        public int NewPositionId { get; set; }
        public virtual Position NewPosition { get; set; }
        public int? NewGradeId { get; set; }
        public Grade NewGrade { get; set; }
        public DateTime PromotionDate { get; set; } = DateTime.Now;
        public string Reason { get; set; }
        public float NewSalary { get; set; }
    }
}
