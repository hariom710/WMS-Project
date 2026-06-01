using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models;

public class Announcement
{
    [Key]
    public int AnnouncementId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; }

    [Required, MaxLength(4000)]
    public string Message { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual Employee? CreatedByEmployee { get; set; }

    public bool IsActive { get; set; } = true;
}
