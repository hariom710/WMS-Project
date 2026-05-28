using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Models;

public class Announcement
{
    [Key]
    public int AnnouncementId { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Message { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public int CreatedBy { get; set; }
    public bool IsActive { get; set; } = true;
}