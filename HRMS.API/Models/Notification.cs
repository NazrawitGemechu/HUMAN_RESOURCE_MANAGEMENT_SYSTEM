﻿namespace HRMS.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
