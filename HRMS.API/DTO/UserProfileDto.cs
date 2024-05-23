namespace HRMS.API.DTO
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PictureURL { get; set; }
        public byte[] PictureData { get; set; }
    }
}
