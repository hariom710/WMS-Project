using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models;

public class Project : BaseEntity
{
    [Key]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Project name is required.")]
    [MaxLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
    [MinLength(3, ErrorMessage = "Project name must be at least 3 characters.")]
    public string ProjectName { get; set; }

    public int? ClientId { get; set; }

    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
}
