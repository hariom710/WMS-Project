using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.API.Models;

namespace WMS.Domain.Models;

public class ProjectAllocation
{
    [Key]
    public int AllocationId { get; set; }

    [Required]
    public int EmpId { get; set; }
    [ForeignKey("EmpId")]
    public virtual Employee? Employee { get; set; }

    [Required]
    public int ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    [Required]
    public DateTime AssignedOn { get; set; }

    // Capgemini Tracking Columns
    public DateTime CreateDate { get; set; } = DateTime.Now;

    public string CreatedBy { get; set; } = "Admin"; // Will be updated by Controller

    public bool Status { get; set; } = true; // Active/Inactive (Bit)

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}