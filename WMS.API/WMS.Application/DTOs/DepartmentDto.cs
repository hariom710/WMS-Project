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
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateDepartmentDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
