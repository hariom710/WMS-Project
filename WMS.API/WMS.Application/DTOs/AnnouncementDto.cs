using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs
{
    public class AnnouncementDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateAnnouncementDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters.")]
        public string Message { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateAnnouncementDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters.")]
        public string Message { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
