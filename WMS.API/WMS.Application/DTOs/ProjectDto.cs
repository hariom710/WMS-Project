using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        [MaxLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Project name must be at least 3 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        public int? ClientId { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Active";
    }

    public class UpdateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        [MaxLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Project name must be at least 3 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        public int? ClientId { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Active";
    }
}
