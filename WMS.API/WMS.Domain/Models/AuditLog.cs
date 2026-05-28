using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.API.Models // Matching your existing namespace
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }

        public string EntityName { get; set; } // Table Updated

        public int RecordId { get; set; } // Affected Row

        [MaxLength(20)]
        public string Action { get; set; } // Insert / Update / Delete

        public int CreatedBy { get; set; } // User who performed action

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}