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

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
