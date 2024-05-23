namespace HRMS.API.DTO
{
    public class ContactPersonDto
    {
        public string ContactPersonName { get; set; }
        public string Relationship { get; set; }
        public string ContactPhoneNo { get; set; }
        public string ContactRegion { get; set; }
        public string ContactWoreda { get; set; }
        public int ContactKebele { get; set; }
        public string? ContactHouseNo { get; set; }
    }
}
