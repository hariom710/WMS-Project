using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Models;

public class Announcement : BaseEntity
{
    [Key]
    public int AnnouncementId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Message is required.")]
    [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
    [MinLength(10, ErrorMessage = "Message must be at least 10 characters.")]
    public string Message { get; set; }

    public int CreatedByEmployeeId { get; set; }

    [ForeignKey("CreatedByEmployeeId")]
    public virtual Employee? CreatedByEmployee { get; set; }

    public bool IsActive { get; set; } = true;
}
