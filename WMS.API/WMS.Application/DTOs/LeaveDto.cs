namespace WMS.Application.DTOs
{
    public class LeaveDto
    {
        public int LeaveId { get; set; }
        public int EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime AppliedOn { get; set; }
    }

    public class CreateLeaveDto
    {
        public int? EmpId { get; set; }
        public string LeaveType { get; set; } = "Sick";
        public string Reason { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
