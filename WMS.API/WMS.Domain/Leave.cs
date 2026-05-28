using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.API.Models;

namespace WMS.Domain.Models;

public class Leave
{
    [Key]
    public int LeaveId { get; set; }

    [Required]
    public int EmpId { get; set; }

    // Creates the Foreign Key relationship to Employee
    [ForeignKey("EmpId")]
    public virtual Employee? Employee { get; set; }

    [Required]
    public string LeaveType { get; set; } // Sick/Casual/Earned

    public string Reason { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    public string Status { get; set; } = "Pending";

    // Capgemini tracking requirements
    public DateTime AppliedOn { get; set; } = DateTime.Now;

    public int? ApprovedBy { get; set; } // Nullable, as it is pending

    public DateTime? ApprovedOn { get; set; } // Nullable
}