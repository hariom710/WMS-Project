using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Employee ID is required.")]
        public int? EmpId { get; set; }

        [Required(ErrorMessage = "Leave type is required.")]
        public string LeaveType { get; set; } = "Sick";

        [Required(ErrorMessage = "Reason is required.")]
        [MinLength(10, ErrorMessage = "Reason must be at least 10 characters.")]
        [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "From date is required.")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To date is required.")]
        public DateTime ToDate { get; set; }
    }
}
