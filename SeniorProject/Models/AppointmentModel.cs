using System;

namespace TestWebApplication.Models
{
    public class AppointmentModel
    {
        public int AppointmentID { get; set; }
        public int InstructorUserID { get; set; }
        public int StudentUserID { get; set; }
        public DateTime Time { get; set; }
        public bool isConfirmed { get; set; }

    }
}