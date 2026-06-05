using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }

        [Required, MaxLength(100)]
        public string EntityName { get; set; }

        public int RecordId { get; set; }

        [Required, MaxLength(20)]
        public string Action { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(50)]
        public string? UserRole { get; set; }

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
