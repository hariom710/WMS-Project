namespace WMS.Application.DTOs
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public int EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public float? TotalHours { get; set; }
        public string WorkMode { get; set; } = "Office";
        public DateTime AttendanceDate { get; set; }
    }

    public class CheckInDto
    {
        public int? EmpId { get; set; }
        public string WorkMode { get; set; } = "Office";
    }
}
