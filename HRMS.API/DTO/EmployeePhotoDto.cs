namespace HRMS.API.DTO
{
    public class EmployeePhotoDto
    {
        public int EmployeeId { get; set; }
        public IFormFile Photo { get; set; }
    }
}
