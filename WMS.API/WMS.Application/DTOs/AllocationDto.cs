using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs
{
    public class AllocationDto
    {
        public int AllocationId { get; set; }
        public int EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public DateTime AssignedOn { get; set; }
        public string? CreatedBy { get; set; }
        public bool Status { get; set; }
    }

    public class CreateAllocationDto
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmpId { get; set; }

        [Required(ErrorMessage = "Project ID is required.")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Assignment date is required.")]
        public DateTime AssignedOn { get; set; } = DateTime.Now;
    }
}
