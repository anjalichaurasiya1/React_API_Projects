using System;

namespace product_api.Models
{
    public class AttendenceModel
    {
        public DateTime AttendanceDate { get; set; }

        public DateTime? IN_Time { get; set; }

        public DateTime? OUT_Time { get; set; }

        public decimal WorkingHours { get; set; }

        public decimal OTHours { get; set; }

        public string? Status { get; set; }
         public AttendenceModel? Summary { get; set; }
        public List<AttendenceModel>? Attendance { get; set; }
    }
}