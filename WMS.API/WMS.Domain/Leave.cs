using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models;

public class Leave
{
    [Key]
    public int LeaveId { get; set; }

    [Required]
    public int EmpId { get; set; }

    [ForeignKey("EmpId")]
    public virtual Employee? Employee { get; set; }

    [Required, MaxLength(20)]
    public string LeaveType { get; set; } // Sick/Casual/Earned

    [Required, MaxLength(500)]
    public string Reason { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public DateTime AppliedOn { get; set; } = DateTime.Now;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
}
