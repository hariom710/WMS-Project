using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models;

public class Leave : BaseEntity
{
    [Key]
    public int LeaveId { get; set; }

    [Required(ErrorMessage = "Employee ID is required.")]
    public int EmpId { get; set; }

    [ForeignKey("EmpId")]
    public virtual Employee? Employee { get; set; }

    [Required(ErrorMessage = "Leave type is required.")]
    [MaxLength(20)]
    public string LeaveType { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
    [MinLength(10, ErrorMessage = "Reason must be at least 10 characters.")]
    public string Reason { get; set; }

    [Required(ErrorMessage = "From date is required.")]
    public DateTime FromDate { get; set; }

    [Required(ErrorMessage = "To date is required.")]
    public DateTime ToDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
}
