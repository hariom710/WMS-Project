using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models;

public class ProjectAllocation : BaseEntity
{
    [Key]
    public int AllocationId { get; set; }

    [Required(ErrorMessage = "Employee ID is required.")]
    public int EmpId { get; set; }
    [ForeignKey("EmpId")]
    public virtual Employee? Employee { get; set; }

    [Required(ErrorMessage = "Project ID is required.")]
    public int ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    [Required(ErrorMessage = "Assignment date is required.")]
    public DateTime AssignedOn { get; set; }

    public bool Status { get; set; } = true;

    [MaxLength(200)]
    public string? Note { get; set; }
}
