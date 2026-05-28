using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Models;

public class Project
{
    [Key]
    public int ProjectId { get; set; }

    [Required]
    public string ProjectName { get; set; }

    // ---> NEW: Nullable ClientId to prevent Foreign Key crashes <---
    public int? ClientId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; } = "Active";
}