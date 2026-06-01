namespace WMS.Application.DTOs
{
    public class AnnouncementDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateAnnouncementDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateAnnouncementDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
