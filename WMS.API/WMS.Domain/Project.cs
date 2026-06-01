using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models;

public class Project
{
    [Key]
    public int ProjectId { get; set; }

    [Required, MaxLength(100)]
    public string ProjectName { get; set; }

    public int? ClientId { get; set; }

    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Active";
}
