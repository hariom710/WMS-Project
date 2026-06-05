using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Models
{
    public abstract class BaseEntity
    {
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        [MaxLength(100)]
        public string? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
