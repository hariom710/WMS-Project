namespace WMS.Application.DTOs
{
    public class AuditLogDto
    {
        public int AuditId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Username { get; set; }
        public string? UserRole { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
