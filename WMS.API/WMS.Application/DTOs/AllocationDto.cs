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
        public string CreatedBy { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class CreateAllocationDto
    {
        public int EmpId { get; set; }
        public int ProjectId { get; set; }
        public DateTime AssignedOn { get; set; } = DateTime.Now;
    }
}
