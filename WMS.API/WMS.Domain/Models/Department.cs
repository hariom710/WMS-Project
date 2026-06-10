using System.ComponentModel.DataAnnotations;

namespace WMS.Domain.Models
{
    public class Department : BaseEntity
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [MaxLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Department name must be at least 3 characters.")]
        public string DepartmentName { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
