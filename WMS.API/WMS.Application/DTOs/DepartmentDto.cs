using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CreateDepartmentDto
    {
        [Required(ErrorMessage = "Department name is required.")]
        [MaxLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Department name must be at least 3 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }

    public class UpdateDepartmentDto
    {
        [Required(ErrorMessage = "Department name is required.")]
        [MaxLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Department name must be at least 3 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
